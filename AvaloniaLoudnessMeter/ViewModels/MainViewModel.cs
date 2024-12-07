using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using AvaloniaLoudnessMeter.DataModels;
using AvaloniaLoudnessMeter.Services;
using ReactiveUI;

namespace AvaloniaLoudnessMeter.ViewModels;

public class MainViewModel : ViewModelBase
{
    #region Private Members

    private readonly IAudioInterfaceService mAudioInterfaceService;

    #endregion

    #region Public Properties

    private bool _channelConfigurationListIsOpen;

    public bool ChannelConfigurationListIsOpen
    {
        get => _channelConfigurationListIsOpen;
        set { this.RaiseAndSetIfChanged(ref _channelConfigurationListIsOpen, value); }
    }

    public string BoldTitle { get; set; } = "AVALONIA";

    public string RegularTitle { get; set; } = "LOUDNESS METER";


    private ObservableCollection<ChannelConfigurationItem> _channelConfigurations;
    public ObservableCollection<ChannelConfigurationItem> ChannelConfigurations
    {
        get => _channelConfigurations;
        set
        {
            this.RaiseAndSetIfChanged(ref _channelConfigurations, value);
        }
    }

    private ChannelConfigurationItem? _selectedChannelConfiguration;

    public ChannelConfigurationItem? SelectedChannelConfiguration
    {
        get => _selectedChannelConfiguration;
        set => this.RaiseAndSetIfChanged(ref _selectedChannelConfiguration, value);
    }

    public string ChannelConfigurationButtonText => _selectedChannelConfiguration?.ShortText ?? "Select a channel";

    #endregion

    #region Public Commands

    public ReactiveCommand<ChannelConfigurationItem, Unit> ChannelConfigurationItemPressedCommand { get; }

    public void ChannelConfigurationButtonPressed()
    {
        ChannelConfigurationListIsOpen ^= true;
    }

    public void ChannelConfigurationItemPressed(ChannelConfigurationItem item = null)
    {
        // Update the selected item
        SelectedChannelConfiguration = item;

        // Close the menu
        ChannelConfigurationListIsOpen ^= true;
    }

    public async Task LoadSettingsAsync()
    {
        var channelConfigurations = await mAudioInterfaceService.GetChannelConfigurationsAsync();

        ChannelConfigurations = new ObservableCollection<ChannelConfigurationItem>(channelConfigurations);
    }

    #endregion

    #region Constructor

    /// <summary>
    ///     Default constructor
    /// </summary>
    /// <param name="audioInterfaceService">The audio interface service</param>
    public MainViewModel(IAudioInterfaceService audioInterfaceService)
    {
        mAudioInterfaceService = audioInterfaceService;

        ChannelConfigurationItemPressedCommand =
            ReactiveCommand.Create<ChannelConfigurationItem>(ChannelConfigurationItemPressed);
    }

    /// <summary>
    ///     Design-time constructor
    /// </summary>
    public MainViewModel()
    {
        mAudioInterfaceService = new DummyAudioInterfaceService();

        ChannelConfigurationItemPressedCommand =
            ReactiveCommand.Create<ChannelConfigurationItem>(ChannelConfigurationItemPressed);
    }

    #endregion
}