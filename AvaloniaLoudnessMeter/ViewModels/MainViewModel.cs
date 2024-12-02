using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

namespace AvaloniaLoudnessMeter.ViewModels;

public class MainViewModel : ViewModelBase
{
    public string BoldTitle { get; set; } = "AVALONIA";

    public string RegularTitle { get; set; } = "LOUDNESS METER";
    public bool _ChannelConfigurationListIsOpen = false;

    public bool ChannelConfigurationListIsOpen
    {
        get => _ChannelConfigurationListIsOpen;
        set => this.RaiseAndSetIfChanged(ref _ChannelConfigurationListIsOpen, value);
    }
    
    public void ChannelConfigurationButtonPressed() => ChannelConfigurationListIsOpen ^= true;
}