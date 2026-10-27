using CalculatorApp.Platforms.Android.HueWheel;
using CalculatorApp.SettingsPage.Animations;
using CalculatorApp.SettingsPage.Appearance;

namespace CalculatorApp.SettingsPage.Colors;

public partial class ColorSettingsPage
{
    public ColorSettingsPage(AppearanceSettings appearanceSettings,
        AnimationSettings animationSettings, HueWheelDialog hueWheelDialog) : base(animationSettings)
    {
        InitializeComponent();
        BindingContext = appearanceSettings;
        HexColorSetting.HueWheelDialog = hueWheelDialog;
    }
}
