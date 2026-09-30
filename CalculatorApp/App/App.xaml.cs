using CalculatorApp.SettingsPage.App;
using CalculatorApp.SettingsPage.Appearance;
using CalculatorApp.SettingsPage.Typography;
using CalculatorApp.Shell;

namespace CalculatorApp.App;

public partial class App
{
    private readonly AppShell _appShell;

    public App(AppSettings appSettings,
        AppearanceSettings appearanceSettings,
        TypographySettings typographySettings)
    {
        InitializeComponent();
        appearanceSettings.Initialize(Resources);
        appSettings.Initialize();
        typographySettings.Initialize();
        _appShell = new AppShell(appearanceSettings);
    }

    protected override Window CreateWindow(IActivationState? activationState) => new(_appShell);
}
