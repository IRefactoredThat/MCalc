using CalculatorApp.Platforms.Android.ColorWheel;
using CalculatorApp.SettingsPage.Animations;
using CalculatorApp.SettingsPage.Appearance;

namespace CalculatorApp.SettingsPage.Colors;

public sealed class LightColorRoleSettingsPage(
    AppearanceSettings appearanceSettings,
    AnimationSettings animationSettings,
    ColorWheelDialog colorWheelDialog)
    : ColorRoleSettingsPage(appearanceSettings, animationSettings, colorWheelDialog,
        appearanceSettings.LightColorRoles, false);
