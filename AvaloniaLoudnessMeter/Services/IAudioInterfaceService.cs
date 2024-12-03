using System.Collections.Generic;
using System.Threading.Tasks;
using AvaloniaLoudnessMeter.DataModels;

namespace AvaloniaLoudnessMeter.Services;

public interface IAudioInterfaceService
{
    /// <summary>
    ///     Fetch the channel configurations
    /// </summary>
    /// <returns></returns>
    Task<List<ChannelConfigurationItem>> GetChannelConfigurationsAsync();
}