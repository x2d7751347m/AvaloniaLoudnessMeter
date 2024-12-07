using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AvaloniaLoudnessMeter.DataModels;

namespace AvaloniaLoudnessMeter.Services;

public interface IAudioCaptureService
{
    /// <summary>
    ///     Fetch the channel configurations
    /// </summary>
    /// <returns></returns>
    Task<List<ChannelConfigurationItem>> GetChannelConfigurationsAsync();

    /// <summary>
    /// Start capturing audio
    /// </summary>
    void Start();
    
    /// <summary>
    /// Stop capturing audio
    /// </summary>
    void Stop();
    
    /// <summary>
    /// A callback for when the next chunk of audio data is available
    /// </summary>
    public event Action<AudioChunkData> AudioChunkAvailable;
}