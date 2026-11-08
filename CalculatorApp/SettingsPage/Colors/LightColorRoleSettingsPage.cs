using CalculatorApp.Platforms.Android.ColorWheel;
using CalculatorApp.SettingsPage.Animations;
using CalculatorApp.SettingsPage.Appearance;

namespace CalculatorApp.SettingsPage.Colors;

public sealed class LightColorRoleSettingsPage : ColorRoleSettingsPage
{
    public LightColorRoleSettingsPage(AppearanceSettings appearanceSettings,
        AnimationSettings animationSettings, ColorWheelDialog colorWheelDialog)
        : base(appearanceSettings, animationSettings, colorWheelDialog,
            appearanceSettings.LightColorRoles)
    {
        Header = "Light palette";
    }
}
