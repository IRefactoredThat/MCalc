using CalculatorApp.SettingsPage.Animations;

namespace CalculatorApp.SettingsPage.Backup;

public partial class BackupSettingsPage
{
    public BackupSettingsPage(AnimationSettings animationSettings,
        BackupViewModel backupViewModel)
        : base(animationSettings)
    {
        InitializeComponent();
        BindingContext = backupViewModel;
    }
}

