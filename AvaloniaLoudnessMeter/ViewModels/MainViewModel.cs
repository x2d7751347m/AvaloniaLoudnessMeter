using ReactiveUI;

namespace AvaloniaLoudnessMeter.ViewModels;

public class MainViewModel : ViewModelBase
{
    public bool _ChannelConfigurationListIsOpen;
    public string BoldTitle { get; set; } = "AVALONIA";

    public string RegularTitle { get; set; } = "LOUDNESS METER";

    public bool ChannelConfigurationListIsOpen
    {
        get => _ChannelConfigurationListIsOpen;
        set => this.RaiseAndSetIfChanged(ref _ChannelConfigurationListIsOpen, value);
    }

    public void ChannelConfigurationButtonPressed()
    {
        ChannelConfigurationListIsOpen ^= true;
    }
}