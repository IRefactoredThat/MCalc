using CalculatorApp.CalculatorPage.MainGrid.Cells;

namespace CalculatorApp.CalculatorPage.MainGrid.Parts;

public enum PartChangedAction { Add, Remove, Update, Clear }

public class PartChangedEventArgs(
    CellRange affectedCell,
    PartChangedAction action,
    GridViewModelPart? affectedPart = null) : EventArgs
{
    public CellRange AffectedCell { get; } = affectedCell;
    public PartChangedAction Action { get; } = action;
    public GridViewModelPart? AffectedPart { get; } = affectedPart;
}
