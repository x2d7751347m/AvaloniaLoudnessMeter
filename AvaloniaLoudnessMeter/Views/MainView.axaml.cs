using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using AvaloniaLoudnessMeter.Services;
using AvaloniaLoudnessMeter.ViewModels;
using ManagedBass;

namespace AvaloniaLoudnessMeter.Views;

public partial class MainView : UserControl
{
    #region Constructor

    public MainView()
    {
        InitializeComponent();

        mSizingTimer = new Timer(t => { Dispatcher.UIThread.InvokeAsync(() => { UpdateSizes(); }); });

        mChannelConfigButton =
            this.FindControl<Control>("ChannelConfigurationButton") ??
            throw new Exception("Could not find control 'mChannelConfigButton'");
        mChannelConfigPopup =
            this.FindControl<Control>("ChannelConfigurationPopup") ??
            throw new Exception("Could not find control 'mChannelConfigPopup'");
        mMainGrid =
            this.FindControl<Control>("MainGrid") ?? throw new Exception("Could not find control 'mMainGrid'");
        mVolumeContainer =
            this.FindControl<Control>("VolumeContainer") ??
            throw new Exception("Could not find control 'mVolumeContainer'");
    }

    #endregion

    private void UpdateSizes()
    {
        ((MainViewModel)DataContext).VolumeContainerSize = mVolumeContainer.Bounds.Height;
    }

    protected override async void OnLoaded(RoutedEventArgs routedEventArgs)
    {
        await ((MainViewModel)DataContext).LoadSettingsAsync();

        Task.Run(async () =>
        {
            // Output all devices, then select one
            foreach (var device in RecordingDevice.Enumerate()) Console.WriteLine($"{device?.Index}: {device?.Name}");

            var outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MBass");
            Directory.CreateDirectory(outputPath);

            var filePath = Path.Combine(outputPath, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".wav");
            using var writer =
                new WaveFileWriter(
                    new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read),
                    new WaveFormat());

            using var mCaptureDevice = new AudioCaptureService(0);

            mCaptureDevice.DataAvailable += (buffer, length) =>
            {
                writer.Write(buffer, length);

                Console.WriteLine(BitConverter.ToString(buffer));
            };

            mCaptureDevice.Start();

            await Task.Delay(3000);

            mCaptureDevice.Stop();

            await Task.Delay(100);
        });

        base.OnLoaded(routedEventArgs);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        mSizingTimer.Change(100, int.MaxValue);

        Dispatcher.UIThread.InvokeAsync(() =>
        {
            var position = mChannelConfigButton.TranslatePoint(new Point(), mMainGrid) ??
                           throw new Exception("Cannot get TranslatedPoint from Configuration Button");

            // Set margin of popup so it appears bottom left of button
            mChannelConfigPopup.Margin = new Thickness(
                position.X,
                0,
                0,
                mMainGrid.Bounds.Height - position.Y - mChannelConfigButton.Bounds.Height);
        });
    }

    private void InputElement_OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        ((MainViewModel)DataContext).ChannelConfigurationButtonPressed();
    }

    #region Private Members

    private readonly Control mChannelConfigPopup;
    private readonly Control mChannelConfigButton;
    private readonly Control mMainGrid;
    private readonly Control mVolumeContainer;

    /// <summary>
    ///     The timeout timer to detect when auto-sizing has finished firing
    /// </summary>
    private readonly Timer mSizingTimer;

    #endregion
}