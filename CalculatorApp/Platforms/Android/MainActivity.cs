using Android.App;
using Android.Content.PM;

namespace CalculatorApp;

[Activity(Theme = "@style/Maui.SplashTheme",
    MainLauncher = true, 
    LaunchMode = LaunchMode.SingleTop, 
    ConfigurationChanges = ConfigChanges.ScreenSize 
    | ConfigChanges.Orientation | ConfigChanges.UiMode 
    | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize 
    | ConfigChanges.Density, ScreenOrientation = ScreenOrientation.Landscape)]
public class MainActivity : MauiAppCompatActivity
{
}