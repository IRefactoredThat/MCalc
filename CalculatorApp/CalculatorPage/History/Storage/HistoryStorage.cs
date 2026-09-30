using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using CalculatorApp.CalculatorPage.History.Converters;

namespace CalculatorApp.CalculatorPage.History.Storage;

[JsonSerializable(typeof(HistoryItem))]
[JsonSerializable(typeof(ObservableCollection<HistoryItem>))]
internal sealed partial class HistoryJsonContext : JsonSerializerContext { }

public class HistoryStorage(TokenConverter tokenConverter, NumberTokenConverter numberTokenConverter)
    : IHistoryStorage
{
    private readonly HistoryJsonContext _context = new(new JsonSerializerOptions
    {
        Converters = { tokenConverter, numberTokenConverter },
    });
    private static readonly string HistoryPath = Path.Combine(FileSystem.AppDataDirectory, "history.json");

    public ObservableCollection<HistoryItem> GetItems()
    {
        if (!File.Exists(HistoryPath))
        {
            return [];
        }

        try
        {
            using var stream = File.OpenRead(HistoryPath);
            return JsonSerializer.Deserialize(stream, _context.ObservableCollectionHistoryItem)!;
        }
        catch (JsonException)
        {
            File.Delete(HistoryPath);
            return [];
        }
    }

    public async Task SaveItems(ObservableCollection<HistoryItem> items)
    {
        var json = JsonSerializer.Serialize(items, _context.ObservableCollectionHistoryItem);
        await File.WriteAllTextAsync(HistoryPath, json);
    }
}
