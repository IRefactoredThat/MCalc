using System.Text.Json;
using CalculatorApp.CalculatorPage.Buttons;
using CalculatorApp.CalculatorPage.MainGrid.Parts;
using CalculatorApp.SettingsPage.Backup;
using CalculatorApp.SettingsPage.Formatting;
using CalculatorApp.SettingsPage.Layout;

namespace CalculatorApp.CalculatorPage.MainGrid.Models;

public class CustomModelStorage : IStorage<CustomGridViewModel>
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
            return JsonSerializer.Deserialize<CustomGridViewModel>(
                modelText, _context.CustomGridViewModel) ?? new CustomGridViewModel();
        }
        catch (JsonException)
        {
            Delete();
            return new CustomGridViewModel();
        }
    }

    public async Task SaveAsync(CustomGridViewModel model)
    {
        var customModel = JsonSerializer.Serialize<CustomGridViewModel>(model,
            _context.CustomGridViewModel);
        await File.WriteAllTextAsync(CustomModelPath, customModel);
    }

    public void Save(CustomGridViewModel model)
    {
        var customModel = JsonSerializer.Serialize<CustomGridViewModel>(model,
            _context.CustomGridViewModel);
        File.WriteAllText(CustomModelPath, customModel);
    }

    public void Delete() => File.Delete(CustomModelPath);

    public CustomGridViewModel? Import(string content)
    {
        try
        {
            return JsonSerializer.Deserialize<CustomGridViewModel>(content,
                _context.CustomGridViewModel);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public string Export() =>
        JsonSerializer.Serialize(File.ReadAllText(CustomModelPath),
            _context.CustomGridViewModel);
}
