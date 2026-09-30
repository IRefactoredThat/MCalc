using System.Text.Json;
using System.Text.Json.Serialization;
using CalculatorApp.CalculatorPage.Buttons;
using CalculatorApp.CalculatorPage.MainGrid.Cells;
using CalculatorApp.CalculatorPage.MainGrid.Parts;
using CalculatorApp.SettingsPage.Formatting;
using CalculatorApp.SettingsPage.Layout;

namespace CalculatorApp.CalculatorPage.MainGrid.Models;

[JsonSerializable(typeof(CustomGridViewModel))]
internal sealed partial class CustomGridViewModelJsonContext : JsonSerializerContext { }

public class CustomGridViewModel : IGridViewModel
{
    private static readonly string CustomModelPath = Path.Combine(
        FileSystem.AppDataDirectory, "custom_model.json");

    private readonly Dictionary<CellRange, GridViewModelPart> _parts = [];

    private readonly CustomGridViewModelJsonContext _context;

    [JsonIgnore]
    public IBaseButtonViewModel[] Pool { get; }

    public CustomGridViewModel(FormattingSettings formattingSettings)
    {
        Pool = LayoutExtensions.Create(formattingSettings);
        _context = new CustomGridViewModelJsonContext(new JsonSerializerOptions
        {
            Converters = { new GridPartJsonConverter(Pool) },
        });

        if (!File.Exists(CustomModelPath))
        {
            return;
        }

        try
        {
            var customModel = File.ReadAllText(CustomModelPath);
            var model = JsonSerializer.Deserialize<CustomGridViewModel>(customModel,
                _context.CustomGridViewModel);
            if (model is null)
            {
                return;
            }

            _unitWidth = model.UnitWidth;
            _unitHeight = model.UnitHeight;
            foreach (var (cell, part) in model.Parts)
            {
                _parts[cell] = part;
            }
        }
        catch (JsonException)
        {
            File.Delete(CustomModelPath);
        }
    }

    [JsonConstructor]
    public CustomGridViewModel(
        IReadOnlyDictionary<CellRange, GridViewModelPart> parts,
        int unitWidth, int unitHeight)
    {
        _unitWidth = unitWidth;
        _unitHeight = unitHeight;
        foreach (var (cell, part) in parts)
        {
            _parts[cell] = part;
        }

        Pool = null!;
        _context = null!;
    }

    public event EventHandler<PartChangedEventArgs>? PartChanged;

    private int _unitWidth;
    private int _unitHeight;

    public int UnitWidth
    {
        get => _unitWidth;
        set
        {
            _unitWidth = value;
            Save();
        }
    }

    public int UnitHeight
    {
        get => _unitHeight;
        set
        {
            _unitHeight = value;
            Save();
        }
    }

    public IReadOnlyDictionary<CellRange, GridViewModelPart> Parts => _parts;

    public bool Occupied(int row, int column) =>
        _parts.Keys.Any(cell => cell.Contains(row, column));

    public bool OverlapsWithExistingCells(CellRange cell)
    {
        return _parts.Keys.
            Any(existing => existing.Row < cell.Row + cell.RowSpan &&
                            existing.Row + existing.RowSpan > cell.Row &&
                            existing.Column < cell.Column + cell.ColumnSpan &&
                            existing.Column + existing.ColumnSpan > cell.Column);
    }

    public void Add(CellRange cell, GridViewModelPart part)
    {
        if (cell.Row + cell.RowSpan > UnitHeight ||
            cell.Column + cell.ColumnSpan > UnitWidth ||
            OverlapsWithExistingCells(cell))
        {
            return;
        }

        _parts[cell] = part;
        PartChanged?.Invoke(this, new PartChangedEventArgs(cell,
            PartChangedAction.Add, part));
        Save();
    }

    public void Update(CellRange cell, GridViewModelPart part)
    {
        _parts[cell] = part;
        PartChanged?.Invoke(this, new PartChangedEventArgs(cell,
            PartChangedAction.Update, part));
        Save();
    }

    public void Remove(CellRange cell)
    {
        _parts.Remove(cell);
        PartChanged?.Invoke(this, new PartChangedEventArgs(cell,
            PartChangedAction.Remove));
        Save();
    }

    public void Clear()
    {
        _parts.Clear();
        PartChanged?.Invoke(this, new PartChangedEventArgs(default,
            PartChangedAction.Clear));
        File.Delete(CustomModelPath);
    }

    private void Save()
    {
        var customModel = JsonSerializer.Serialize(this,
            _context.CustomGridViewModel);
        File.WriteAllText(CustomModelPath, customModel);
    }
}
