using CalculatorApp.Platforms.Android.ColorWheel;
using CalculatorApp.SettingsPage.Animations;
using CalculatorApp.SettingsPage.Appearance;

namespace CalculatorApp.SettingsPage.Colors;

public partial class ColorRoleSettingsPage
{
    public ColorWheelDialog ColorWheelDialog { get; }

    public ColorRoleSettingsPage(AppearanceSettings appearanceSettings,
        AnimationSettings animationSettings, ColorWheelDialog colorWheelDialog) : base(animationSettings)
    {
        ColorWheelDialog = colorWheelDialog;
        InitializeComponent();
        BindingContext = appearanceSettings;
    }
}
