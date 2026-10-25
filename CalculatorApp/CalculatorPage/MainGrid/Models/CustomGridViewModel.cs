using System.Text.Json.Serialization;
using CalculatorApp.CalculatorPage.MainGrid.Cells;
using CalculatorApp.CalculatorPage.MainGrid.Parts;

namespace CalculatorApp.CalculatorPage.MainGrid.Models;

[JsonSerializable(typeof(CustomGridViewModel))]
internal sealed partial class CustomGridViewModelJsonContext : JsonSerializerContext { }

public class CustomGridViewModel : IGridViewModel
{
    private readonly Dictionary<CellRange, GridViewModelPart> _parts = [];

    public CustomGridViewModel() { }

    [JsonConstructor]
    public CustomGridViewModel(
        IReadOnlyDictionary<CellRange, GridViewModelPart> parts,
        int unitWidth, int unitHeight)
    {
        UnitWidth = unitWidth;
        UnitHeight = unitHeight;
        foreach (var (cell, part) in parts)
        {
            _parts[cell] = part;
        }
    }

    public event EventHandler<PartChangedEventArgs>? PartChanged;

    public int UnitWidth { get; set; }
    public int UnitHeight { get; set; }

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
    }

    public void Update(CellRange cell, GridViewModelPart part)
    {
        _parts[cell] = part;
        PartChanged?.Invoke(this, new PartChangedEventArgs(cell,
            PartChangedAction.Update, part));
    }

    public void Remove(CellRange cell)
    {
        _parts.Remove(cell);
        PartChanged?.Invoke(this, new PartChangedEventArgs(cell,
            PartChangedAction.Remove));
    }

    public void Clear()
    {
        _parts.Clear();
        PartChanged?.Invoke(this, new PartChangedEventArgs(default,
            PartChangedAction.Clear));
    }

    // Clear button should only be explicitly added by user
    public void Update(bool includeClearButton) { }
}
