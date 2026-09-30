using CalculatorApp.CalculatorPage.Input;
using CalculatorApp.Platforms.Android.Handlers;
using CalculatorApp.Platforms.Android.Shell;
using CalculatorApp.SettingsPage.Typography;
using CalculatorApp.SharedElements.BottomSheet;
using CalculatorApp.SharedElements.ContentButton;
using CalculatorApp.SharedElements.StepSlider;
using CalculatorApp.Shell;

namespace CalculatorApp.App;

public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder ConfigureHandlers(this MauiAppBuilder builder) =>
        builder.ConfigureMauiHandlers(handlers =>
        {
            handlers.AddHandler<CalculatorInput, CalculatorInputHandler>()
                .AddHandler<StepSlider, StepSliderHandler>()
                .AddHandler<BottomSheet, BottomSheetHandler>()
                .AddHandler<ContentButton, ContentButtonHandler>()
                .AddHandler<AppShell, AppShellRender>();
        });

    public static MauiAppBuilder RegisterFonts(this MauiAppBuilder builder) =>
        builder.ConfigureFonts(fontCollection =>
        {
            foreach (var alias in Enum.GetNames<CustomFonts>())
            {
                fontCollection.AddFont($"{alias}.ttf", alias);
            }

            fontCollection.AddFont($"{TypographySettings.Icons}.ttf",
                TypographySettings.Icons);
            fontCollection.AddFont($"{TypographySettings.IconsFilled}.ttf",
                TypographySettings.IconsFilled);
        });
}
