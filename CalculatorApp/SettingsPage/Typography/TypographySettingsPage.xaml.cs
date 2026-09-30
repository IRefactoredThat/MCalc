using CalculatorApp.SettingsPage.Animations;

namespace CalculatorApp.SettingsPage.Typography;

public partial class TypographySettingsPage
{
    public TypographySettingsPage(TypographySettings typographySettings,
        AnimationSettings animationSettings) : base(animationSettings)
    {
        InitializeComponent();
        BindingContext = typographySettings;
    }
}