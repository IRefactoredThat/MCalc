using CalculatorApp.CalculatorPage.History;
using CalculatorApp.CalculatorPage.History.Converters;
using CalculatorApp.CalculatorPage.History.Storage;
using CalculatorApp.CalculatorPage.Input;
using CalculatorApp.CalculatorPage.MainGrid.Models;
using CalculatorApp.CalculatorPage.Output;
using CalculatorApp.CalculatorPage.Page;
using CalculatorApp.Platforms.Android.Extensions;
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

namespace CalculatorApp.App;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services) =>
        services.AddSingleton<AppSettings>()
            .AddSingleton<Tokens>()
            .AddSingleton<Calculation>()
            .AddSingleton<TokenConverter>()
            .AddSingleton<NumberTokenConverter>()
            .AddSingleton<IHistoryStorage, HistoryStorage>();

    public static IServiceCollection AddSettings(this IServiceCollection services) =>
        services.AddSingleton<SystemScheme>()
            .AddSingleton<AppearanceSettings>()
            .AddSingleton<TypographySettings>()
            .AddSingleton<TextExtensions>()
            .AddSingleton<FormattingSettings>()
            .AddSingleton<HistorySettings>()
            .AddSingleton<AnimationSettings>()
            .AddSingleton<LayoutSettings>();

    public static IServiceCollection AddViewModels(this IServiceCollection services) =>
        services.AddTransient<CalculatorOutputViewModel>()
            .AddTransient<CalculatorInputViewModel>()
            .AddTransient<HistoryViewModel>()
            .AddTransient<CalculatorPageViewModel>()
            .AddTransient<MixedGridViewModel>()
            .AddTransient<SimpleGridViewModel>()
            .AddTransient<CustomGridViewModel>()
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
            .AddTransient<AppSettingsPage>();
}
