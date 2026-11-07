using CalculatorApp.SettingsPage.Animations;

namespace CalculatorApp.SettingsPage.CustomLayout;

public partial class CustomLayoutSettingsPage
{
    public CustomLayoutSettingsPage(CustomLayoutViewModel customLayoutViewModel,
        AnimationSettings animationSettings) : base(animationSettings)
    {
        InitializeComponent();
        BindingContext = customLayoutViewModel;
    }
}
