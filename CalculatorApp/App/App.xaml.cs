using CalculatorApp.SettingsPage.App;
using CalculatorApp.SettingsPage.Appearance;
using CalculatorApp.SettingsPage.Backup;
using CalculatorApp.SettingsPage.Typography;
using CalculatorApp.Shell;

namespace CalculatorApp.App;

public partial class App
{
    private readonly AppShell _appShell;

    public App(AppSettings appSettings,
        AppearanceSettings appearanceSettings,
        TypographySettings typographySettings,
        BackupSettings backupSettings,
        IReadOnlyList<IReload> reloadableSettings)
    {
        InitializeComponent();

        appSettings.Initialize();
        appearanceSettings.Initialize(Resources);
        typographySettings.Initialize(Resources);

        foreach (var reloadableSetting in reloadableSettings)
        {
            reloadableSetting.Reload();
        }
        backupSettings.Reload();

        _appShell = new AppShell(appearanceSettings);
    }

    protected override Window CreateWindow(IActivationState?
        activationState) => new(_appShell);
}
