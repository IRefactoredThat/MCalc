using CalculatorApp.Platforms.Android.ColorWheel;
using CalculatorApp.SettingsPage.Animations;
using CalculatorApp.SettingsPage.Appearance;

namespace CalculatorApp.SettingsPage.Colors;

public partial class ColorRoleSettingsPage
{
    public ColorWheelDialog ColorWheelDialog { get; }
    public IReadOnlyList<ColorRole> ColorRoles { get; }

    public ColorRoleSettingsPage(AppearanceSettings appearanceSettings,
        AnimationSettings animationSettings, ColorWheelDialog colorWheelDialog)
        : this(appearanceSettings, animationSettings, colorWheelDialog,
            appearanceSettings.DarkColorRoles)
    {
    }

    protected ColorRoleSettingsPage(AppearanceSettings appearanceSettings,
        AnimationSettings animationSettings, ColorWheelDialog colorWheelDialog,
        IReadOnlyList<ColorRole> colorRoles) : base(animationSettings)
    {
        ColorWheelDialog = colorWheelDialog;
        ColorRoles = colorRoles;
        InitializeComponent();
        BindingContext = appearanceSettings;
        Header = "Dark palette";
    }
}
