using CalculatorApp.CalculatorPage.MainGrid.Cells;
using CalculatorApp.CalculatorPage.MainGrid.Parts;

namespace CalculatorApp.CalculatorPage.MainGrid.Models;

public interface IGridViewModel
{
    int UnitHeight { get; }
    int UnitWidth { get; }
    IReadOnlyDictionary<CellRange, GridViewModelPart> Parts { get; }
}