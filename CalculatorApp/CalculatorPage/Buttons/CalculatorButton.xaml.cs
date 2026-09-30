using CalculatorApp.SettingsPage.Animations;
using CalculatorApp.SettingsPage.Layout;
using CalculatorApp.SettingsPage.Typography;
using CalculatorApp.SharedElements.ContentButton;

namespace CalculatorApp.CalculatorPage.Buttons;

public partial class CalculatorButton
{
    private const double MaxFontFactor = 0.6;

    public CalculatorButton() => InitializeComponent();

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(
            nameof(Text),
            typeof(string),
            typeof(ContentButton),
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: OnTextChanged);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    private static async void OnTextChanged(BindableObject bindable,
        object? oldValue, object? newValue)
    {
        if (bindable is not CalculatorButton button || newValue is not string text)
        {
            return;
        }

        if (oldValue is null || button.TransitionAnimation is TransitionAnimation.None)
        {
            button.Label.Text = text;
            button.UpdateFontSize();
        }
        else if (button.TransitionAnimation is TransitionAnimation.Fade)
        {
            await button.Label.FadeToAsync(0, length: 200, easing: Easing.CubicIn);
            button.Label.Text = text;
            button.UpdateFontSize();
            await button.Label.FadeToAsync(1, length: 200, easing: Easing.CubicOut);
        }
    }

    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(
            nameof(TextColor),
            typeof(Color),
            typeof(ContentButton),
            propertyChanged: OnTextColorChanged);

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    private static void OnTextColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not CalculatorButton button || newValue is not Color color)
        {
            return;
        }

        button.Label.TextColor = color;
    }

    public static readonly BindableProperty TransitionAnimationProperty =
        BindableProperty.Create(
            nameof(TransitionAnimation),
            typeof(TransitionAnimation),
            typeof(ContentButton),
            defaultValue: TransitionAnimation.None);

    public TransitionAnimation TransitionAnimation
    {
        get => (TransitionAnimation)GetValue(TransitionAnimationProperty);
        set => SetValue(TransitionAnimationProperty, value);
    }

    public static readonly BindableProperty ButtonShapeProperty =
        BindableProperty.Create(
            nameof(ButtonShape),
            typeof(ButtonShape),
            typeof(ContentButton),
            defaultValue: ButtonShape.Circle,
            propertyChanged: OnButtonShapeChanged);

    public ButtonShape ButtonShape
    {
        get => (ButtonShape)GetValue(ButtonShapeProperty);
        set => SetValue(ButtonShapeProperty, value);
    }

    private static void OnButtonShapeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not CalculatorButton button)
        {
            return;
        }

        button.UpdateCornerRadius();
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        UpdateCornerRadius();
        UpdateFontSize();
    }

    private void UpdateFontSize()
    {
        var scale = Label.Text.Length < 2 ? 1 :
            Math.Sqrt(2) / Math.Sqrt(Label.Text.Length);
        var baseSize = Math.Min(Width, Height) * MaxFontFactor;
        Label.FontSize = Math.Max(
            TypographySettings.GetCurrentSize(
                nameof(TypographySettings.ButtonMinFontSize),
                TypographySettings.ButtonMinFontSize),
            baseSize * scale);
    }

    private void UpdateCornerRadius()
    {
        CornerRadius = ButtonShape switch
        {
            ButtonShape.Circle => new CornerRadius(Math.Min(Width, Height) / 2),
            ButtonShape.Square => 0,
            _ => throw new ArgumentOutOfRangeException(nameof(ButtonShape),
                "Invalid button shape")
        };
    }
}
