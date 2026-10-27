using CalculatorApp.Platforms.Android.HueWheel;
using CalculatorApp.SettingsPage.Appearance;

namespace CalculatorApp.SettingsPage.Colors.ColorWheelSetting;

public partial class ColorWheelSetting
{
    public ColorWheelSetting()
    {
        InitializeComponent();
        Clicked += OnClicked;
    }

    public static readonly BindableProperty HueWheelDialogProperty =
        BindableProperty.Create(
            nameof(HueWheelDialog),
            typeof(HueWheelDialog),
            typeof(ColorWheelSetting));

    public HueWheelDialog HueWheelDialog
    {
        get => (HueWheelDialog)GetValue(HueWheelDialogProperty);
        set => SetValue(HueWheelDialogProperty, value);
    }

    private void OnClicked(object? sender, EventArgs e)
    {
        HueWheelDialog.ShowColorPicker(Color, selected => { Color = selected; });
    }

    public static readonly BindableProperty IconProperty =
        BindableProperty.Create(
            nameof(Icon),
            typeof(string),
            typeof(ColorWheelSetting));

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly BindableProperty NameProperty =
        BindableProperty.Create(
            nameof(Name),
            typeof(string),
            typeof(ColorWheelSetting));

    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    public static readonly BindableProperty DescriptionProperty =
        BindableProperty.Create(
            nameof(Description),
            typeof(string),
            typeof(ColorWheelSetting));

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public static readonly BindableProperty ColorProperty =
        BindableProperty.Create(
            nameof(Color),
            typeof(Color),
            typeof(ColorWheelSetting),
            defaultValue: AppearanceSettings.FallbackSeed,
            defaultBindingMode: BindingMode.TwoWay);

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }
}
