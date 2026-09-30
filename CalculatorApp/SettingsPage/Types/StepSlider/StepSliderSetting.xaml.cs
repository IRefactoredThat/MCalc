namespace CalculatorApp.SettingsPage.Types.StepSlider;

public partial class StepSliderSetting
{
    public StepSliderSetting() => InitializeComponent();

    public static readonly BindableProperty MaximumProperty =
        BindableProperty.Create(
            nameof(Maximum),
            typeof(double),
            typeof(StepSliderSetting));

    public double Maximum
    {
        get => (double)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public static readonly BindableProperty MinimumProperty =
        BindableProperty.Create(
            nameof(Minimum),
            typeof(double),
            typeof(StepSliderSetting));

    public double Minimum
    {
        get => (double)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(
            nameof(Value),
            typeof(double),
            typeof(StepSliderSetting),
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: OnValueTextDependencyChanged);

    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly BindableProperty IsPercentProperty =
        BindableProperty.Create(
            nameof(IsPercent),
            typeof(bool),
            typeof(StepSliderSetting),
            false,
            propertyChanged: OnValueTextDependencyChanged);

    public bool IsPercent
    {
        get => (bool)GetValue(IsPercentProperty);
        set => SetValue(IsPercentProperty, value);
    }

    public string ValueText =>
        IsPercent ? $"{Value * 100:0.##}%" : $"{Value:0.##}";

    private static void OnValueTextDependencyChanged(
        BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not StepSliderSetting sliderSetting)
        {
            return;
        }

        sliderSetting.OnPropertyChanged(nameof(ValueText));
    }

    public static readonly BindableProperty ResetValueProperty =
        BindableProperty.Create(
            nameof(ResetValue),
            typeof(double),
            typeof(StepSliderSetting));

    public double ResetValue
    {
        get => (double)GetValue(ResetValueProperty);
        set => SetValue(ResetValueProperty, value);
    }

    public static readonly BindableProperty StepProperty =
        BindableProperty.Create(
            nameof(Step),
            typeof(double),
            typeof(StepSliderSetting),
            1d);

    public double Step
    {
        get => (double)GetValue(StepProperty);
        set => SetValue(StepProperty, value);
    }

    public static readonly BindableProperty IconProperty =
        BindableProperty.Create(
            nameof(Icon),
            typeof(string),
            typeof(StepSliderSetting));

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly BindableProperty NameProperty =
        BindableProperty.Create(
            nameof(Name),
            typeof(string),
            typeof(StepSliderSetting));

    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    public static readonly BindableProperty DescriptionProperty =
        BindableProperty.Create(
            nameof(Description),
            typeof(string),
            typeof(StepSliderSetting));

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }
}