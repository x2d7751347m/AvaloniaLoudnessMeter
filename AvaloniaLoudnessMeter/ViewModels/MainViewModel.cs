using System.Reactive;
using System.Reactive.Linq;

namespace AvaloniaLoudnessMeter.ViewModels;

public class MainViewModel : ViewModelBase
{
    public string BoldTitle { get; set; } = "AVALONIA";

    public string RegularTitle { get; set; } = "LOUDNESS METER";
    public bool ChannelConfigurationListIsOpen { get; set; } = false;
    
    public void ChannelConfigurationButtonPressed() => ChannelConfigurationListIsOpen ^= true;
}