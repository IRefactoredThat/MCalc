namespace CalculatorApp.SharedElements.ContentButton;

public class ScaleBehavior : Behavior<ContentButton>
{
    public static readonly BindableProperty IsEnabledProperty =
        BindableProperty.Create(
            nameof(IsEnabled),
            typeof(bool),
            typeof(ScaleBehavior),
            defaultValue: true);

    public bool IsEnabled
    {
        get => (bool)GetValue(IsEnabledProperty);
        set => SetValue(IsEnabledProperty, value);
    }

    public static readonly BindableProperty PressedScaleProperty =
        BindableProperty.Create(
            nameof(PressedScale),
            typeof(double),
            typeof(ScaleBehavior),
            defaultValue: 0.9d);

    public double PressedScale
    {
        get => (double)GetValue(PressedScaleProperty);
        set => SetValue(PressedScaleProperty, value);
    }

    public static readonly BindableProperty PressedLengthProperty =
        BindableProperty.Create(
            nameof(PressedLength),
            typeof(uint),
            typeof(ScaleBehavior),
            defaultValue: 120u);

    public uint PressedLength
    {
        get => (uint)GetValue(PressedLengthProperty);
        set => SetValue(PressedLengthProperty, value);
    }

    public static readonly BindableProperty PressedEasingProperty =
        BindableProperty.Create(
            nameof(PressedEasing),
            typeof(Easing),
            typeof(ScaleBehavior));

    public Easing? PressedEasing
    {
        get => (Easing?)GetValue(PressedEasingProperty);
        set => SetValue(PressedEasingProperty, value);
    }

    public static readonly BindableProperty ReleasedScaleProperty =
        BindableProperty.Create(
            nameof(ReleasedScale),
            typeof(double),
            typeof(ScaleBehavior),
            defaultValue: 1d);

    public double ReleasedScale
    {
        get => (double)GetValue(ReleasedScaleProperty);
        set => SetValue(ReleasedScaleProperty, value);
    }

    public static readonly BindableProperty ReleasedLengthProperty =
        BindableProperty.Create(
            nameof(ReleasedLength),
            typeof(uint),
            typeof(ScaleBehavior),
            defaultValue: 160u);

    public uint ReleasedLength
    {
        get => (uint)GetValue(ReleasedLengthProperty);
        set => SetValue(ReleasedLengthProperty, value);
    }

    public static readonly BindableProperty ReleasedEasingProperty =
        BindableProperty.Create(
            nameof(ReleasedEasing),
            typeof(Easing),
            typeof(ScaleBehavior));

    public Easing? ReleasedEasing
    {
        get => (Easing?)GetValue(ReleasedEasingProperty);
        set => SetValue(ReleasedEasingProperty, value);
    }

    protected override void OnAttachedTo(ContentButton button)
    {
        base.OnAttachedTo(button);
        button.Pressed += OnButtonPressed;
        button.Released += OnButtonReleased;
    }

    protected override void OnDetachingFrom(ContentButton button)
    {
        button.Pressed -= OnButtonPressed;
        button.Released -= OnButtonReleased;
        base.OnDetachingFrom(button);
    }

    private async void OnButtonPressed(object? sender, EventArgs e)
    {
        if (!IsEnabled || sender is not ContentButton button)
        {
            return;
        }

        await button.ScaleToAsync(PressedScale, PressedLength, PressedEasing);
    }

    private async void OnButtonReleased(object? sender, EventArgs e)
    {
        if (!IsEnabled || sender is not ContentButton button)
        {
            return;
        }

        await button.ScaleToAsync(ReleasedScale, ReleasedLength, ReleasedEasing);
    }
}