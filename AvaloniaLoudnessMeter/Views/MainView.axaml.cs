using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using AvaloniaLoudnessMeter.ViewModels;

namespace AvaloniaLoudnessMeter.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();

        mChannelConfigButton =
            this.FindControl<Control>("ChannelConfigurationButton") ??
            throw new Exception("Could not find control 'mChannelConfigButton'");
        mChannelConfigPopup =
            this.FindControl<Control>("ChannelConfigurationPopup") ??
            throw new Exception("Could not find control 'mChannelConfigPopup'");
        mMainGrid =
            this.FindControl<Control>("MainGrid") ?? throw new Exception("Could not find control 'mMainGrid'");
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        var position = mChannelConfigButton.TranslatePoint(new Point(), mMainGrid) ??
                       throw new Exception("Cannot get TranslatedPoint from Configuration Button");
        
        // Set margin of popup so it appears bottom left of button
        try
        {
            mChannelConfigPopup.Margin = new Thickness(
                position.X,
                0,
                0,
                mMainGrid.Bounds.Height - position.Y - mChannelConfigButton.Bounds.Height);
        }
        catch (Exception e)
        {
            // ignored
        }
    }

    private void InputElement_OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        ((MainViewModel)DataContext).ChannelConfigurationButtonPressed();
    }

    #region Public Members

    private readonly Control mChannelConfigPopup;
    private readonly Control mChannelConfigButton;
    private readonly Control mMainGrid;

    #endregion
}