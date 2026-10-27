using CalculatorApp.Platforms.Android.ColorWheel;
using CalculatorApp.Platforms.Android.HueWheel;
using CalculatorApp.SettingsPage.Animations;
using CalculatorApp.SettingsPage.Appearance;

namespace CalculatorApp.SettingsPage.Colors;

public partial class ColorSettingsPage
{
    public ColorSettingsPage(AppearanceSettings appearanceSettings,
        AnimationSettings animationSettings, ColorWheelDialog colorWheelDialog) : base(animationSettings)
    {
        InitializeComponent();
        BindingContext = appearanceSettings;
        HexColorSetting.ColorWheelDialog = colorWheelDialog;
    }
}
