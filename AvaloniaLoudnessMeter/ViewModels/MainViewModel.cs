using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Threading;
using AvaloniaLoudnessMeter.DataModels;
using AvaloniaLoudnessMeter.Services;
using DynamicData;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using ReactiveUI;
using SkiaSharp;

namespace AvaloniaLoudnessMeter.ViewModels;

public class MainViewModel : ViewModelBase
{
    private void initialize()
    {
        MainChartValues.AddRange(Enumerable.Range(0, 200).Select(f => new ObservableValue(0)));
        
        Series = new ISeries[]
        {
            new LineSeries<ObservableValue>
            {
                Values = MainChartValues,
                GeometrySize = 0,
                GeometryStroke = null,
                Fill = new SolidColorPaint(new SKColor(63, 77, 99)),
                Stroke = new SolidColorPaint(new SKColor(120, 152, 203)) { StrokeThickness = 3 }
            }
        };
    }

    /// <summary>
    ///     Starts capturing audio from the specified device
    /// </summary>
    /// <param name="deviceId">The device ID</param>
    private void StartCapture(int deviceId)
    {
        // Initialize capturing on specific device
        _mAudioCaptureService.InitCapture(deviceId);

        // Listen out for chunks of information
        _mAudioCaptureService.AudioChunkAvailable += audioChunkData =>
        {
            ProcessAudioChunk(audioChunkData);
        };

        // Start capturing
        _mAudioCaptureService.Start();
    }

    private void ProcessAudioChunk(AudioChunkData audioChunkData)
    {
        // Counter between 0-1-2
        mUpdateCounter = (mUpdateCounter + 1) % 3;

        // Every time counter is at 0...
        if (mUpdateCounter == 0)
        {
            ShortTermLoudness = $"{Math.Max(-60, audioChunkData.ShortTermLUFS):0.0} LUFS";
            IntegratedLoudness = $"{Math.Max(-60, audioChunkData.IntegratedLUFS):0.0} LUFS";
            LoudnessRange = $"{Math.Max(-60, audioChunkData.LoudnessRange):0.0} LU";
            RealtimeDynamics = $"{Math.Max(-60, audioChunkData.RealtimeDynamics):0.0} LU";
            AverageDynamics = $"{Math.Max(-60, audioChunkData.AverageRealtimeDynamics):0.0} LU";
            MomentaryMaxLoudness = $"{Math.Max(-60, audioChunkData.MomentaryMaxLUFS):0.0} LUFS";
            ShortTermMaxLoudness = $"{Math.Max(-60, audioChunkData.ShortTermMaxLUFS):0.0} LUFS";
            TruePeakMax = $"{Math.Max(-60, audioChunkData.TruePeakMax):0.0} dB";
            
            Dispatcher.UIThread.Invoke(() =>
            {
                MainChartValues.RemoveAt(0);
                MainChartValues.Add(new (Math.Max(0,60+audioChunkData.ShortTermLUFS)));
            });
        }

        // Set volume bar height
        VolumeBarMaskHeight = Math.Min(_volumeBarHeight, _volumeBarHeight / 60 * -audioChunkData.Loudness);

        // Set Volume Arrow height
        VolumePercentPosition = Math.Min(_volumeContainerHeight,
            _volumeContainerHeight / 60 * -audioChunkData.ShortTermLUFS);
    }

    #region Private Members

    // The audio capture service
    private readonly IAudioCaptureService _mAudioCaptureService;

    /// <summary>
    ///     A slow tick counter to update the text slower than the graphs and bars
    /// </summary>
    private int mUpdateCounter;

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

    private double _volumeContainerHeight;

    public double VolumeContainerHeight
    {
        get => _volumeContainerHeight;
        set => this.RaiseAndSetIfChanged(ref _volumeContainerHeight, value);
    }

    private double _volumeBarHeight;

    public double VolumeBarHeight
    {
        get => _volumeBarHeight;
        set => this.RaiseAndSetIfChanged(ref _volumeBarHeight, value);
    }

    private double _volumeBarMaskHeight;

    public double VolumeBarMaskHeight
    {
        get => _volumeBarMaskHeight;
        set => this.RaiseAndSetIfChanged(ref _volumeBarMaskHeight, value);
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

    public ObservableCollection<ObservableValue> MainChartValues = new ObservableCollection<ObservableValue>();
    
    public ISeries[] Series { get; set; }

    public List<Axis> YAxis { get; set; } = new()
    {
        new Axis
        {
            MinStep = 10,
            ForceStepToMin = true,
            MinLimit = 0,
            MaxLimit = 60,
            Labeler = val => (Math.Min(60, Math.Max(0, val)) - 60).ToString(),
            IsVisible = false
            // IsInverted = true
        }
    };

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
    ///     Do initial loading of data and settings up services
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
}