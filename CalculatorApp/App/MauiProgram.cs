using CommunityToolkit.Maui;

namespace CalculatorApp.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp
            .CreateBuilder()
            .UseMauiCommunityToolkit()
            .RegisterFonts()
            .UseMauiApp<App>()
            .ConfigureHandlers();

        builder.Services
            .AddPersistence()
            .AddSettings()
            .AddViewModels()
            .AddPages();

        return builder.Build();
    }
}