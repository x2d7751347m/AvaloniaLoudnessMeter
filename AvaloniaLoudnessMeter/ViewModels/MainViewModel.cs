using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Threading;
using AvaloniaLoudnessMeter.DataModels;
using AvaloniaLoudnessMeter.Services;
using ReactiveUI;

namespace AvaloniaLoudnessMeter.ViewModels;

public class MainViewModel : ViewModelBase
{
    #region Private Members

    // The audio capture service
    private IAudioCaptureService _mAudioCaptureService;

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
    
    private string _shortTermLoudness = "0 LUFS";
    
    private string _integratedLoudness = "0 LUFS";
    
    private string _loudnessRange = "0 LU";
    
    private string _realtimeDynamics = "0 LU";
    
    private string _averageDynamics = "0 LU";
    
    private string _momentaryMaxLoudness = "0 LUFS";
    
    private string _shortTermMaxLoudness = "0 LUFS";
    
    private string _truePeakMax = "0 dB";

    public string ShortTermLoudness
    {
        get => _shortTermLoudness;
        set => this.RaiseAndSetIfChanged(ref _shortTermLoudness, value);
    }

    public string IntegratedLoudness
    {
        get => _integratedLoudness;
        set => this.RaiseAndSetIfChanged(ref _integratedLoudness, value);
    }

    public string LoudnessRange
    {
        get => _loudnessRange;
        set => this.RaiseAndSetIfChanged(ref _loudnessRange, value);
    }

    public string RealtimeDynamics
    {
        get => _realtimeDynamics;
        set => this.RaiseAndSetIfChanged(ref _realtimeDynamics, value);
    }

    public string AverageDynamics
    {
        get => _averageDynamics;
        set => this.RaiseAndSetIfChanged(ref _averageDynamics, value);
    }

    public string MomentaryMaxLoudness
    {
        get => _momentaryMaxLoudness;
        set => this.RaiseAndSetIfChanged(ref _momentaryMaxLoudness, value);
    }

    public string ShortTermMaxLoudness
    {
        get => _shortTermMaxLoudness;
        set => this.RaiseAndSetIfChanged(ref _shortTermMaxLoudness, value);
    }

    public string TruePeakMax
    {
        get => _truePeakMax;
        set => this.RaiseAndSetIfChanged(ref _truePeakMax, value);
    }

    private double _volumePercentPosition;

    public double VolumePercentPosition
    {
        get => _volumePercentPosition;
        set => this.RaiseAndSetIfChanged(ref _volumePercentPosition, value);
    }

    private double _volumeContainerSize;

    public double VolumeContainerSize
    {
        get => _volumeContainerSize;
        set => this.RaiseAndSetIfChanged(ref _volumeContainerSize, value);
    }


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

    /// <summary>
    /// Do initial loading of data and settings up services
    /// </summary>
    public async Task LoadCommand()
    {
        var channelConfigurations = await _mAudioCaptureService.GetChannelConfigurationsAsync();

        ChannelConfigurations = new ObservableCollection<ChannelConfigurationItem>(channelConfigurations);

        StartCapture(3);
    }

    #endregion

    #region Constructor

    /// <summary>
    ///     Default constructor
    /// </summary>
    /// <param name="audioCaptureService">The audio interface service</param>
    public MainViewModel(IAudioCaptureService audioCaptureService)
    {
        _mAudioCaptureService = audioCaptureService;

        ChannelConfigurationItemPressedCommand =
            ReactiveCommand.Create<ChannelConfigurationItem>(ChannelConfigurationItemPressed);

        initialize();
    }

    /// <summary>
    ///     Design-time constructor
    /// </summary>
    public MainViewModel()
    {
        _mAudioCaptureService = new BassAudioCaptureService();

        ChannelConfigurationItemPressedCommand =
            ReactiveCommand.Create<ChannelConfigurationItem>(ChannelConfigurationItemPressed);

        initialize();
    }
    
    #endregion

    private void initialize()
    {
        // Temp code to move volume position
        var tick = 0;
        var input = 0.0;

        var tempTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1 / 60.0)
        };

        tempTimer.Tick += (s, e) =>
        {
            tick++;

            // Slow down ticks
            input = tick / 20f;

            // Scale value
            var scale = _volumeContainerSize / 2f;

            VolumePercentPosition = (Math.Sin(input) + 1) * scale;
        };

        tempTimer.Start();
    }
    
    /// <summary>
    /// Starts capturing audio from the specified device
    /// </summary>
    /// <param name="deviceId">The device ID</param>
    private void StartCapture(int deviceId)
    {
        _mAudioCaptureService = new BassAudioCaptureService(deviceId);
        
        // Listen out for chunks of information
        _mAudioCaptureService.AudioChunkAvailable += audioChunkData =>
        {
            ShortTermLoudness = $"{audioChunkData.ShortTermLUFS:0.0} LUFS";
            IntegratedLoudness = $"{audioChunkData.IntegratedLUFS:0.0} LUFS";
            LoudnessRange = $"{audioChunkData.LoudnessRange:0.0} LU";
            RealtimeDynamics = $"{audioChunkData.RealtimeDynamics:0.0} LU";
            AverageDynamics = $"{audioChunkData.AverageRealtimeDynamics:0.0} LU";
            MomentaryMaxLoudness = $"{audioChunkData.MomentaryMaxLUFS:0.0} LUFS";
            ShortTermMaxLoudness = $"{audioChunkData.ShortTermMaxLUFS:0.0} LUFS";
            TruePeakMax = $"{audioChunkData.TruePeakMax:0.0} dB";
        };
        
        // Start capturing
        _mAudioCaptureService.Start();
    }
}