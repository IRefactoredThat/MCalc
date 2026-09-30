using CalculatorApp.SettingsPage.Animations;

namespace CalculatorApp.SettingsPage.App;

public partial class AppSettingsPage
{
    public AppSettingsPage(AnimationSettings animationSettings,
        AppSettings exceptionHandler) : base(animationSettings)
    {
        InitializeComponent();
        BindingContext = exceptionHandler;
    }
}