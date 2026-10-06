using CalculatorApp.SettingsPage.Animations;

namespace CalculatorApp.SettingsPage.Backup;

public partial class BackupSettingsPage
{
    public BackupSettingsPage(AnimationSettings animationSettings, BackupSettings backupSettings)
        : base(animationSettings)
    {
        InitializeComponent();
        BindingContext = backupSettings;
    }
}

