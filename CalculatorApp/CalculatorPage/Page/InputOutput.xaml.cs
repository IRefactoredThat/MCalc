using CalculatorApp.CalculatorPage.Input;
using CalculatorApp.Platforms.Android.Extensions;
using CalculatorApp.SettingsPage.Animations;
using CalculatorApp.SettingsPage.Typography;

namespace CalculatorApp.CalculatorPage.Page;

public interface IInputOutput
{
    Task AnimateResultPreTransition(AnimationSettings animationSettings);
    Task AnimateResultPostTransition(AnimationSettings animationSettings);
}

public partial class InputOutput : IInputOutput
{
    private Func<double> _getBestFontScale = null!;
    private Rect _start, _end;

    public InputOutput()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        InputField.InputEntry.Focus();
        _start = GetRelativeBounds(OutputField);
        _end = GetRelativeBounds(InputField);
        Loaded -= OnLoaded;
    }

    public void Initialize(TypographySettings typographySettings, TextExtensions textExtensions)
    {
        if (typographySettings.DynamicTextScaling)
        {
            InputField.InputEntry.Behaviors.Add(new AutoFontSizeBehavior(textExtensions));
            _getBestFontScale = () => textExtensions.BestFontSize(Overlay.Text, Overlay.FontFamily,
                InputField.Width * TypographySettings.InputWidthFactor) / Overlay.FontSize;
        }
        else
        {
            _getBestFontScale = () => InputField.InputEntry.FontSize / Overlay.FontSize;
        }
    }

    private Rect GetRelativeBounds(VisualElement element)
    {
        var x = element.X;
        var y = element.Y;

        var parent = element.Parent as VisualElement;

        while (parent != null && parent != this)
        {
            x += parent.X;
            y += parent.Y;
            parent = parent.Parent as VisualElement;
        }

        return new Rect(x, y, element.Width, element.Height);
    }

    public async Task AnimateResultPreTransition(AnimationSettings animationSettings)
    {
        if (animationSettings.OutputToInputAnimation == OutputToInputAnimation.None)
        {
            OutputField.OutputLabel.Text = string.Empty;
        }
        else if (animationSettings.OutputToInputAnimation is OutputToInputAnimation.Morph)
        {
            // Copy text
            Overlay.Text = OutputField.OutputLabel.Text;
            Overlay.FontSize = OutputField.OutputLabel.FontSize;
            OutputField.OutputLabel.Text = string.Empty;

            // Reset state
            Overlay.Scale = 1;
            Overlay.TextColor = OutputField.OutputLabel.TextColor;
            Overlay.IsVisible = true;
            Overlay.TranslationY = _start.Bottom - _end.Bottom;
            Overlay.TranslationX = _start.Right - _end.Right;
            InputField.InputEntry.Unfocus();

            // Animate
            await Task.WhenAll(
                Overlay.TranslateToAsync(0, 0, length: 350, easing: Easing.SinOut),
                Overlay.ScaleToAsync(_getBestFontScale(), length: 350, easing: Easing.SinOut),
                Overlay.TextColorTo(InputField.InputEntry.TextColor, length: 350, easing: Easing.Linear),
                InputField.FadeToAsync(0, length: 300, easing: Easing.Linear));
        }
        else if (animationSettings.OutputToInputAnimation is OutputToInputAnimation.Push)
        {
            Overlay.Text = OutputField.OutputLabel.Text;
            Overlay.FontSize = OutputField.OutputLabel.FontSize;
            OutputField.OutputLabel.Text = string.Empty;

            // Reset state
            Overlay.Scale = 1;
            Overlay.TextColor = OutputField.OutputLabel.TextColor;
            Overlay.IsVisible = true;
            Overlay.TranslationY = _start.Bottom - _end.Bottom;
            Overlay.TranslationX = _start.Right - _end.Right;
            InputField.InputEntry.Unfocus();

            // Animate
            await Task.WhenAll(
                Overlay.TranslateToAsync(0, 0, length: 350, easing: Easing.SinOut),
                Overlay.ScaleToAsync(_getBestFontScale(), length: 350, easing: Easing.SinOut),
                Overlay.TextColorTo(InputField.InputEntry.TextColor, length: 350, easing: Easing.Linear),
                InputField.TranslateToAsync(0, -InputField.Height, length: 300, easing: Easing.Linear));
        }
    }

    public Task AnimateResultPostTransition(AnimationSettings animationSettings)
    {
        if (animationSettings.OutputToInputAnimation is OutputToInputAnimation.Morph)
        {
            InputField.Opacity = 1;
            Overlay.IsVisible = false;
            InputField.InputEntry.Focus();
        }
        else if (animationSettings.OutputToInputAnimation is OutputToInputAnimation.Push)
        {
            InputField.TranslationY = 0;
            InputField.InputEntry.Text = ((CalculatorInputViewModel)InputField.BindingContext).Text;
            Overlay.IsVisible = false;
            InputField.InputEntry.Focus();
        }
        return Task.CompletedTask;
    }
}