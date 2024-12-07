using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
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
        set => this.RaiseAndSetIfChanged(ref _channelConfigurationListIsOpen, value);
    }

    public string BoldTitle { get; set; } = "AVALONIA";

    public string RegularTitle { get; set; } = "LOUDNESS METER";

    private double _volumePercentPosition;

    public double VolumePercentPosition { get => _volumePercentPosition; set => this.RaiseAndSetIfChanged(ref _volumePercentPosition, value); }

    private double _volumeContainerSize;

    public double VolumeContainerSize { get => _volumeContainerSize; set => this.RaiseAndSetIfChanged(ref _volumeContainerSize, value); }


    private ObservableCollection<ChannelConfigurationItem> _channelConfigurations;

    public ObservableCollection<ChannelConfigurationItem> ChannelConfigurations
    {
        get => _channelConfigurations;
        set => this.RaiseAndSetIfChanged(ref _channelConfigurations, value);
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
        
        initialize();
    }

    /// <summary>
    ///     Design-time constructor
    /// </summary>
    public MainViewModel()
    {
        mAudioInterfaceService = new DummyAudioInterfaceService();

        ChannelConfigurationItemPressedCommand =
            ReactiveCommand.Create<ChannelConfigurationItem>(ChannelConfigurationItemPressed);
        
        initialize();
    }

    private void initialize()
    {
        // Temp code to move volume position
        var tick = 0;
        var input = 0.0;

        var tempTimer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromSeconds(1/60.0)
        };

        tempTimer.Tick += (s, e) =>
        {
            tick++;
            
            // Slow down ticks
            input = tick / 20f;
            
            // Scale value
            var scale = _volumeContainerSize/2f;
            
            VolumePercentPosition = (Math.Sin(input) + 1) * scale;
        };
        
        tempTimer.Start();
    }

    #endregion
}