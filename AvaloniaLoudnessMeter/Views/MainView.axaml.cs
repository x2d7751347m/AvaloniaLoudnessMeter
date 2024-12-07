using System;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using AvaloniaLoudnessMeter.ViewModels;

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
        mVolumeBar =
            this.FindControl<Control>("VolumeBar") ??
            throw new Exception("Could not find control 'mVolumeBar'");
    }

    #endregion

    /// <summary>
    ///     Updates the application window/control sizes dynamically
    /// </summary>
    private void UpdateSizes()
    {
        mViewModel.VolumeContainerHeight = mVolumeContainer.Bounds.Height;
        mViewModel.VolumeBarHeight = mVolumeBar.Bounds.Height;
    }

    /// <summary>
    ///     Run on-load initialization code
    /// </summary>
    /// <param name="routedEventArgs"></param>
    protected override async void OnLoaded(RoutedEventArgs routedEventArgs)
    {
        await mViewModel.LoadCommand();

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
        mViewModel.ChannelConfigurationButtonPressed();
    }

    #region Private Members

    /// <summary>
    ///     The main view of this view
    /// </summary>
    private MainViewModel mViewModel => (MainViewModel)DataContext;

    private readonly Control mChannelConfigPopup;
    private readonly Control mChannelConfigButton;
    private readonly Control mMainGrid;
    private readonly Control mVolumeContainer;
    private readonly Control mVolumeBar;

    /// <summary>
    ///     The timeout timer to detect when auto-sizing has finished firing
    /// </summary>
    private readonly Timer mSizingTimer;

    #endregion
}