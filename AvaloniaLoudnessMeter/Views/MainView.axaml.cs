using System;
using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using ReactiveUI;

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
            // Console.WriteLine(e);
            // throw;
        }
    }

    #region Private Members

    private readonly Control mChannelConfigPopup;
    private readonly Control mChannelConfigButton;
    private readonly Control mMainGrid;

    #endregion
}