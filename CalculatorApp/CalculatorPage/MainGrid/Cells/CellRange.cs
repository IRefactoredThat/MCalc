using System.Text.Json.Serialization;

namespace CalculatorApp.CalculatorPage.MainGrid.Cells;

[JsonConverter(typeof(CellRangeJsonConverter))]
public readonly record struct CellRange(int Row, int Column, int RowSpan, int ColumnSpan)
{
    public bool Contains(int row, int column) =>
        row >= Row && row < Row + RowSpan &&
        column >= Column && column < Column + ColumnSpan;

    public bool IsAdjacentTo(CellRange other)
    {
        var right = Column + ColumnSpan;
        var bottom = Row + RowSpan;

        var otherRight = other.Column + other.ColumnSpan;
        var otherBottom = other.Row + other.RowSpan;

        var verticalTouch =
            (right == other.Column || otherRight == Column) &&
            Row < otherBottom &&
            bottom > other.Row;

        var horizontalTouch =
            (bottom == other.Row || otherBottom == Row) &&
            Column < otherRight &&
            right > other.Column;

        return verticalTouch || horizontalTouch;
    }
}