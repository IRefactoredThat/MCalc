using CalculatorApp.SettingsPage.Animations;

namespace CalculatorApp.SettingsPage.Layout;

public partial class LayoutSettingsPage
{
    public LayoutSettingsPage(LayoutSettings layoutSettings,
        AnimationSettings animationSettings) : base(animationSettings)
    {
        InitializeComponent();
        BindingContext = layoutSettings;
    }
}
