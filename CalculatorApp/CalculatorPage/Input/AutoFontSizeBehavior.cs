using CalculatorApp.Platforms.Android.Extensions;
using CalculatorApp.SettingsPage.Typography;

namespace CalculatorApp.CalculatorPage.Input;

public class AutoFontSizeBehavior(TextExtensions textExtensions) : Behavior<Entry>
{
    private Entry _entry = null!;
    private double _availableWidth;

    protected override void OnAttachedTo(Entry bindable)
    {
        base.OnAttachedTo(bindable);

        _entry = bindable;
        _entry.Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        _availableWidth = (_entry.Parent as VisualElement)!.Width *
                          TypographySettings.InputWidthFactor;
        _entry.TextChanged += OnTextChanged;
        _entry.SizeChanged += OnSizeChanged;
        UpdateFontSize();
        _entry.Loaded -= OnLoaded;
    }

    protected override void OnDetachingFrom(Entry bindable)
    {
        _entry.TextChanged -= OnTextChanged;
        _entry.SizeChanged -= OnSizeChanged;
        base.OnDetachingFrom(bindable);
    }

    private void OnTextChanged(object? sender, EventArgs e) => UpdateFontSize();

    private void OnSizeChanged(object? sender, EventArgs e) => UpdateFontSize();

    private void UpdateFontSize()
    {
        _entry.FontSize = textExtensions
            .BestFontSize(_entry.Text, _entry.FontFamily, _availableWidth);
    }
}
