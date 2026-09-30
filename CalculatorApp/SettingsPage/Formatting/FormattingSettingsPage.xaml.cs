using CalculatorApp.SettingsPage.Animations;

namespace CalculatorApp.SettingsPage.Formatting;

public partial class FormattingSettingsPage
{
    public FormattingSettingsPage(FormattingSettings formattingSettings,
        AnimationSettings animationSettings) : base(animationSettings)
    {
        InitializeComponent();
        BindingContext = formattingSettings;
    }
}