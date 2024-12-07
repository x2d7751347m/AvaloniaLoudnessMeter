namespace AvaloniaLoudnessMeter.DataModels;

/// <summary>
///     Information about a channel configuration
/// </summary>
public class ChannelConfigurationItem
{
    public ChannelConfigurationItem(string group, string text, string shortText)
    {
        Group = group;
        Text = text;
        ShortText = shortText;
    }

    public string Group { get; set; }
    public string Text { get; set; }
    public string ShortText { get; set; }
}