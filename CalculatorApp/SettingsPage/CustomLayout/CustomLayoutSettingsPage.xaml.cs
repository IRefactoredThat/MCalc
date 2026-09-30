using CalculatorApp.SettingsPage.Animations;
using CalculatorApp.SettingsPage.Layout;

namespace CalculatorApp.SettingsPage.CustomLayout;

public partial class CustomLayoutSettingsPage
{
    public LayoutSettings LayoutSettings { get; }

    public CustomLayoutSettingsPage(LayoutSettings layoutSettings,
        AnimationSettings animationSettings) : base(animationSettings)
    {
        InitializeComponent();
        LayoutSettings = layoutSettings;
        BindingContext = layoutSettings;
    }
}
