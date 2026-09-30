using CalculatorApp.SettingsPage.Animations;
using CalculatorApp.SettingsPage.Typography;

namespace CalculatorApp.SettingsPage.FontSize;

public partial class FontSizeSettingsPage
{
    public FontSizeSettingsPage(TypographySettings typographySettings,
        AnimationSettings animationSettings) : base(animationSettings)
    {
        InitializeComponent();
        BindingContext = typographySettings;
    }
}