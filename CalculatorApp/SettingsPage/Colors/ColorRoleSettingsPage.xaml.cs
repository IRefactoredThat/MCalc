using CalculatorApp.Platforms.Android.ColorWheel;
using CalculatorApp.SettingsPage.Animations;
using CalculatorApp.SettingsPage.Appearance;

namespace CalculatorApp.SettingsPage.Colors;

public partial class ColorRoleSettingsPage
{
    public ColorWheelDialog ColorWheelDialog { get; }
    public IReadOnlyList<ColorRole> ColorRoles { get; }

    public static readonly BindableProperty PresetProperty =
        BindableProperty.Create(
            nameof(Preset),
            typeof(ColorPreset),
            typeof(ColorRoleSettingsPage),
            defaultValue: ColorPreset.Custom,
            defaultBindingMode: BindingMode.TwoWay);

    public ColorPreset Preset
    {
        get => (ColorPreset)GetValue(PresetProperty);
        set => SetValue(PresetProperty, value);
    }

    public ColorRoleSettingsPage(AppearanceSettings appearanceSettings,
        AnimationSettings animationSettings, ColorWheelDialog colorWheelDialog)
        : this(appearanceSettings, animationSettings, colorWheelDialog,
            appearanceSettings.DarkColorRoles, true) { }

    protected ColorRoleSettingsPage(AppearanceSettings appearanceSettings,
        AnimationSettings animationSettings, ColorWheelDialog colorWheelDialog,
        IReadOnlyList<ColorRole> colorRoles, bool isDarkPalette)
        : base(animationSettings)
    {
        ColorWheelDialog = colorWheelDialog;
        ColorRoles = colorRoles;
        Header = isDarkPalette ? "Dark palette" : "Light palette";
        InitializeComponent();
        BindingContext = appearanceSettings;
        this.SetBinding(PresetProperty, isDarkPalette
            ? nameof(AppearanceSettings.DarkColorPreset)
            : nameof(AppearanceSettings.LightColorPreset));
    }
}
