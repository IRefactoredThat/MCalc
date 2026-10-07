using System.Text.Json;
using CalculatorApp.CalculatorPage.Buttons;
using CalculatorApp.CalculatorPage.MainGrid.Parts;
using CalculatorApp.SettingsPage.Formatting;
using CalculatorApp.SettingsPage.Layout;

namespace CalculatorApp.CalculatorPage.MainGrid.Models;

public class CustomModelStorage
{
    private static readonly string CustomModelPath = Path.Combine(
        FileSystem.AppDataDirectory, "custom_model.json");
    private readonly CustomGridViewModelJsonContext _context;

    public IBaseButtonViewModel[] Pool { get; }

    public CustomModelStorage(FormattingSettings formattingSettings)
    {
        Pool = LayoutExtensions.Create(formattingSettings);
        _context = new CustomGridViewModelJsonContext(new JsonSerializerOptions
        {
            Converters = { new GridPartJsonConverter(Pool) },
        });
    }

    public CustomGridViewModel Get()
    {
        if (!File.Exists(CustomModelPath))
        {
            return new CustomGridViewModel();
        }

        try
        {
            var modelText = File.ReadAllText(CustomModelPath);
            Console.WriteLine(modelText);
            return JsonSerializer.Deserialize<CustomGridViewModel>(
                modelText, _context.CustomGridViewModel) ?? new CustomGridViewModel();
        }
        catch (JsonException)
        {
            Delete();
            return new CustomGridViewModel();
        }
    }

    public void Save(CustomGridViewModel model)
    {
        var customModel = JsonSerializer.Serialize<CustomGridViewModel>(model,
            _context.CustomGridViewModel);
        File.WriteAllText(CustomModelPath, customModel);
    }

    public void Delete() => File.Delete(CustomModelPath);
}
