using CalculatorApp.Platforms.Android.Alerts;
using CalculatorApp.SettingsPage.Animations;

namespace CalculatorApp.SettingsPage.App;

public partial class AppSettingsPage
{
    public AppSettingsPage(AnimationSettings animationSettings,
        AppSettings exceptionHandler,
        ExceptionLogDialog exceptionLogDialog) : base(animationSettings)
    {
        InitializeComponent();
        BindingContext = exceptionHandler;
        ExceptionLogView.ExceptionLogDialog = exceptionLogDialog;
    }
}