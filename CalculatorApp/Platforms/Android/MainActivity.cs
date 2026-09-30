using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using AndroidX.Activity;
using AndroidX.Core.View;
using CalculatorApp.SettingsPage.Appearance;
using Microsoft.Maui.Platform;

namespace CalculatorApp.Platforms.Android;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize
                           // For future version
                           //| ConfigChanges.Orientation
                           | ConfigChanges.UiMode
                           | ConfigChanges.ScreenLayout
                           | ConfigChanges.SmallestScreenSize)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        EdgeToEdge.Enable(this);
    }

    protected override void OnPostResume()
    {
        base.OnPostResume();

        if (OperatingSystem.IsAndroidVersionAtLeast(29))
        {
            Window!.NavigationBarContrastEnforced = false;
        }
    }

    public override void OnConfigurationChanged(Configuration newConfig)
    {
        base.OnConfigurationChanged(newConfig);
        var nightMask = newConfig.UiMode & UiMode.NightMask;
        var isLight = nightMask == UiMode.NightNo;

        if (Window is not { DecorView: { } decorView } window)
        {
            return;
        }

        var insets = WindowCompat.GetInsetsController(window, decorView);
        if (insets is null)
        {
            return;
        }

        insets.AppearanceLightStatusBars = isLight;
        insets.AppearanceLightNavigationBars = isLight;

        if (!OperatingSystem.IsAndroidVersionAtLeast(29))
        {
            var color = Microsoft.Maui.Controls.Application.Current?.Resources[
                nameof(MaterialScheme.SurfaceBright)] as Color;
            window.SetNavigationBarColor(color?.ToPlatform() ?? Colors.Transparent.ToPlatform());
        }
    }
}
