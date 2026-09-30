using CalculatorApp.SettingsPage.Animations;

namespace CalculatorApp.SettingsPage.Appearance;

public partial class AppearanceSettingsPage
{
    public AppearanceSettingsPage(AppearanceSettings appearanceSettings,
        AnimationSettings animationSettings) : base(animationSettings)
    {
        InitializeComponent();
        BindingContext = appearanceSettings;
    }
}