using Avalonia;
using Avalonia.Controls.Primitives;

namespace AvaloniaLoudnessMeter.Styles;

public class LargeLabelControl : TemplatedControl
{
    public static readonly StyledProperty<string> LargeTextProperty =
        AvaloniaProperty.Register<LargeLabelControl, string>(
            nameof(LargeText), "LARGE TEXT");

    public static readonly StyledProperty<string> SmallTextProperty =
        AvaloniaProperty.Register<LargeLabelControl, string>(
            nameof(SmallText), "SMALL TEXT");

    public string LargeText
    {
        get => GetValue(LargeTextProperty);
        set => SetValue(LargeTextProperty, value);
    }

    public string SmallText
    {
        get => GetValue(SmallTextProperty);
        set => SetValue(SmallTextProperty, value);
    }
}