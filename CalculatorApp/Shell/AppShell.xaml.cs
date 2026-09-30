using CalculatorApp.SettingsPage.Animations;
using CalculatorApp.SettingsPage.App;
using CalculatorApp.SettingsPage.Appearance;
using CalculatorApp.SettingsPage.Colors;
using CalculatorApp.SettingsPage.CustomLayout;
using CalculatorApp.SettingsPage.FontSize;
using CalculatorApp.SettingsPage.Formatting;
using CalculatorApp.SettingsPage.History;
using CalculatorApp.SettingsPage.Layout;
using CalculatorApp.SettingsPage.Typography;

namespace CalculatorApp.Shell;

public partial class AppShell
{
    public AppShell(AppearanceSettings appearanceSettings)
    {
        InitializeComponent();
        BindingContext = appearanceSettings;

        Routing.RegisterRoute(nameof(AppearanceSettingsPage), typeof(AppearanceSettingsPage));
        Routing.RegisterRoute(nameof(ColorSettingsPage), typeof(ColorSettingsPage));
        Routing.RegisterRoute(nameof(FormattingSettingsPage), typeof(FormattingSettingsPage));
        Routing.RegisterRoute(nameof(FontSizeSettingsPage), typeof(FontSizeSettingsPage));
        Routing.RegisterRoute(nameof(TypographySettingsPage), typeof(TypographySettingsPage));
        Routing.RegisterRoute(nameof(HistorySettingsPage), typeof(HistorySettingsPage));
        Routing.RegisterRoute(nameof(AnimationSettingsPage), typeof(AnimationSettingsPage));
        Routing.RegisterRoute(nameof(LayoutSettingsPage), typeof(LayoutSettingsPage));
        Routing.RegisterRoute(nameof(CustomLayoutSettingsPage), typeof(CustomLayoutSettingsPage));
        Routing.RegisterRoute(nameof(AppSettingsPage), typeof(AppSettingsPage));

        Navigated += appearanceSettings.OnNavigated;
    }
}
