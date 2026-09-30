using CalculatorApp.SettingsPage.Animations;

namespace CalculatorApp.SettingsPage.History;

public partial class HistorySettingsPage
{
    public HistorySettingsPage(HistorySettings historySettings,
        AnimationSettings animationSettings) : base(animationSettings)
    {
        InitializeComponent();
        BindingContext = historySettings;
    }
}