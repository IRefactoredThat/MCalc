using CalculatorApp.SettingsPage.Animations;

namespace CalculatorApp.SettingsPage.All;

using Shell = Microsoft.Maui.Controls.Shell;

public partial class AllSettingsPage
{
    public AllSettingsPage(AnimationSettings animationSettings) : base(animationSettings)
    {
        InitializeComponent();
    }

    protected override bool OnBackButtonPressed()
    {
        Shell.Current.CurrentItem = Shell.Current.Items[0];
        return true;
    }
}
