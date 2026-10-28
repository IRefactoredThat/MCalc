using Calculator.ExpressionParsing;
using CalculatorApp.CalculatorPage.History;
using CalculatorApp.CalculatorPage.History.Converters;
using CalculatorApp.CalculatorPage.History.Storage;
using CalculatorApp.CalculatorPage.Input;
using CalculatorApp.CalculatorPage.MainGrid.Models;
using CalculatorApp.CalculatorPage.Output;
using CalculatorApp.CalculatorPage.Page;
using CalculatorApp.Platforms.Android.Alerts;
using CalculatorApp.Platforms.Android.ColorWheel;
using CalculatorApp.Platforms.Android.Extensions;
using CalculatorApp.SettingsPage.Animations;
using CalculatorApp.SettingsPage.App;
using CalculatorApp.SettingsPage.Appearance;
using CalculatorApp.SettingsPage.Backup;
using CalculatorApp.SettingsPage.Base;
using CalculatorApp.SettingsPage.Colors;
using CalculatorApp.SettingsPage.CustomLayout;
using CalculatorApp.SettingsPage.FontSize;
using CalculatorApp.SettingsPage.Formatting;
using CalculatorApp.SettingsPage.History;
using CalculatorApp.SettingsPage.Layout;
using CalculatorApp.SettingsPage.Typography;
using UserMessageDisplayer = CalculatorApp.CalculatorPage.Page.UserMessageDisplayer;

namespace CalculatorApp.App;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services) =>
        services.AddSingleton<Tokens>()
            .AddSingleton<Calculation>()
            .AddSingleton<TokenConverter>()
            .AddSingleton<NumberTokenConverter>()
            .AddSingleton<HistoryStorage>()
            .AddSingleton<SystemScheme>()
            .AddSingleton<ClearHistoryDialog>()
            .AddSingleton<ColorWheelDialog>()
            .AddSingleton<ExceptionLogDialog>()
            .AddSingleton<CustomModelStorage>()
            .AddSingleton<AppearanceSchemeStorage>()
            .AddSingleton<IPreferences>(sp =>
                sp.GetRequiredService<SettingsPreferences>())
            .AddSingleton<SettingsPreferences>();

    public static IServiceCollection AddSettings(this IServiceCollection services) =>
        services.AddSingleton<AppearanceSettings>()
            .AddSingleton<AppSettings>()
            .AddSingleton<TypographySettings>()
            .AddSingleton<TextExtensions>()
            .AddSingleton<FormattingSettings>()
            .AddSingleton<HistorySettings>()
            .AddSingleton<AnimationSettings>()
            .AddSingleton<LayoutSettings>()
            .AddSingleton<BackupSettings>()
            .AddSingleton<IReadOnlyList<IReload>>(sp =>
            [
                sp.GetRequiredService<AppearanceSettings>(),
                sp.GetRequiredService<TypographySettings>(),
                sp.GetRequiredService<FormattingSettings>(),
                sp.GetRequiredService<HistorySettings>(),
                sp.GetRequiredService<AnimationSettings>(),
                sp.GetRequiredService<LayoutSettings>(),
                sp.GetRequiredService<BackupSettings>()
            ]);

    public static IServiceCollection AddViewModels(this IServiceCollection services) =>
        services.AddTransient<CalculatorOutputViewModel>()
            .AddTransient<CalculatorInputViewModel>()
            .AddTransient<HistoryViewModel>()
            .AddTransient<CalculatorPageViewModel>()
            .AddSingleton<MixedGridViewModel>()
            .AddSingleton<SimpleGridViewModel>()
            .AddTransient<CustomLayoutViewModel>()
            .AddTransient<BackupViewModel>()
            .AddTransient<UserMessageDisplayer>();

    public static IServiceCollection AddPages(this IServiceCollection services) =>
        services.AddTransient<CalculatorGridPage>()
            .AddTransient<ColorSettingsPage>()
            .AddTransient<AppearanceSettingsPage>()
            .AddTransient<FontSizeSettingsPage>()
            .AddTransient<TypographySettingsPage>()
            .AddTransient<HistorySettingsPage>()
            .AddTransient<LayoutSettingsPage>()
            .AddTransient<CustomLayoutSettingsPage>()
            .AddTransient<AppSettingsPage>()
            .AddTransient<BackupSettingsPage>();
}
