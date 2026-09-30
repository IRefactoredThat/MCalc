namespace CalculatorApp.SettingsPage.Animations;

public partial class AnimationSettingsPage
{
    public AnimationSettingsPage(AnimationSettings animationSettings) : base(animationSettings)
    {
        InitializeComponent();
        BindingContext = animationSettings;
    }
}