using CalculatorApp.SettingsPage.Typography;

namespace CalculatorApp.SettingsPage.CustomLayout.LayoutBuilder;

internal sealed class GridDrawable(SpreadsheetGrid owner) : IDrawable
{
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (owner.TotalRows <= 0 || owner.TotalColumns <= 0)
        {
            DrawGridPicker(canvas, dirtyRect);
        }
        else
        {
            DrawSpreadsheet(canvas, dirtyRect);
        }
    }

    private void DrawGridPicker(ICanvas canvas, RectF dirtyRect)
    {
        var width = dirtyRect.Width;
        var height = dirtyRect.Height;

        // Background

        canvas.FillColor = owner.PreviewBackgroundColor;
        canvas.FillRectangle(dirtyRect);

        var cellWidth = width / owner.MaxColumns;
        var cellHeight = height / owner.MaxRows;

        // Active selection

        var selectionX = owner.PreviewCell.Column * cellWidth;
        var selectionY = owner.PreviewCell.Row * cellHeight;

        var selectionWidth = owner.PreviewCell.ColumnSpan * cellWidth;
        var selectionHeight = owner.PreviewCell.RowSpan * cellHeight;

        canvas.FillColor = owner.PreviewSelectionBackgroundColor;
        canvas.FillRectangle(selectionX, selectionY, selectionWidth, selectionHeight);

        canvas.StrokeColor = owner.PreviewSelectionBordersColor;
        canvas.StrokeSize = 2;

        canvas.DrawRectangle(selectionX, selectionY, selectionWidth, selectionHeight);

        // Background borders

        canvas.StrokeColor = owner.PreviewBordersColor;
        canvas.StrokeSize = 1.5f;

        for (var column = 0; column <= owner.MaxColumns; column++)
        {
            var x = column * cellWidth;
            canvas.DrawLine(x, 0, x, height);
        }

        for (var row = 0; row <= owner.MaxRows; row++)
        {
            var y = row * cellHeight;
            canvas.DrawLine(0, y, width, y);
        }

        // Label

        var text = $"{owner.PreviewCell.ColumnSpan} × {owner.PreviewCell.RowSpan}";

        canvas.FontColor = owner.PreviewTextColor;
        canvas.FontSize = (float)TypographySettings.GetCurrentSize(
            nameof(TypographySettings.GridDrawableSize), TypographySettings.GridDrawableSize);

        canvas.DrawString(text, selectionX, selectionY,
            selectionWidth, selectionHeight,
            HorizontalAlignment.Center,
            VerticalAlignment.Center);
    }

    private void DrawSpreadsheet(ICanvas canvas, RectF dirtyRect)
    {
        var width = dirtyRect.Width;
        var height = dirtyRect.Height;

        var cellWidth = width / owner.TotalColumns;
        var cellHeight = height / owner.TotalRows;

        // Background

        canvas.FillColor = owner.BackgroundColor;
        canvas.FillRectangle(dirtyRect);

        // Occupied areas

        DrawOccupiedAreas(canvas, cellWidth, cellHeight);

        // Selection

        if (owner.SelectedCell is { RowSpan: > 0, ColumnSpan: > 0 })
        {
            var selectionX = owner.SelectedCell.Column * cellWidth;
            var selectionY = owner.SelectedCell.Row * cellHeight;

            var selectionWidth = owner.SelectedCell.ColumnSpan * cellWidth;
            var selectionHeight = owner.SelectedCell.RowSpan * cellHeight;

            canvas.FillColor = owner.SelectionBackgroundColor;
            canvas.FillRectangle(
                selectionX,
                selectionY,
                selectionWidth,
                selectionHeight);

            canvas.StrokeColor = owner.SelectionBordersColor;
            canvas.StrokeSize = 2;

            canvas.DrawRectangle(
                selectionX,
                selectionY,
                selectionWidth,
                selectionHeight);
        }

        // Grid

        canvas.StrokeColor = owner.BordersColor;
        canvas.StrokeSize = 1;

        for (var column = 0; column <= owner.TotalColumns; column++)
        {
            var x = column * cellWidth;
            canvas.DrawLine(x, 0, x, height);
        }

        for (var row = 0; row <= owner.TotalRows; row++)
        {
            var y = row * cellHeight;
            canvas.DrawLine(0, y, width, y);
        }
    }

    private void DrawOccupiedAreas(ICanvas canvas, float cellWidth, float cellHeight)
    {
        foreach (var area in owner.GridViewModel.Parts.Keys)
        {
            var x = area.Column * cellWidth;
            var y = area.Row * cellHeight;

            var width = area.ColumnSpan * cellWidth;
            var height = area.RowSpan * cellHeight;

            canvas.FillColor = owner.GetAreaColor(area);
            canvas.FillRectangle(x, y, width, height);
        }
    }
}