using CalculatorApp.SettingsPage.Appearance;
using CalculatorApp.SettingsPage.Typography;

namespace CalculatorApp.CalculatorPage.Page;

public class UserMessageDisplayer(
    AppearanceSettings appearanceSettings,
    TypographySettings typographySettings)
{
    private const int MessageVisibilityDuration = 2500;

    public void ShowMessage(string message, Action? onConfirmed)
    {
        Platforms.Android.Alerts.UserMessageDisplayer.ShowAndroidMessage(
            message,
            appearanceSettings.GetColorByName(nameof(MaterialScheme.SurfaceBright)),
            appearanceSettings.GetColorByName(nameof(MaterialScheme.OnSurface)),
            onConfirmed,
            appearanceSettings.GetColorByName(nameof(MaterialScheme.Primary)),
            typographySettings.AppFont,
            MessageVisibilityDuration);
    }
}
