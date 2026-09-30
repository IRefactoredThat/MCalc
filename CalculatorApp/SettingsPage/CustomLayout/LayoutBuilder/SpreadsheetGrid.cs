using CalculatorApp.CalculatorPage.MainGrid.Cells;
using CalculatorApp.CalculatorPage.MainGrid.Models;
using CalculatorApp.CalculatorPage.MainGrid.Parts;

namespace CalculatorApp.SettingsPage.CustomLayout.LayoutBuilder;

using Colors = Microsoft.Maui.Graphics.Colors;

public class SpreadsheetGrid : GraphicsView
{
    public static readonly BindableProperty PreviewBackgroundColorProperty =
        BindableProperty.Create(
            nameof(PreviewBackgroundColor),
            typeof(Color),
            typeof(SpreadsheetGrid),
            defaultValue: Colors.Transparent,
            propertyChanged: OnPreviewBackgroundColorChanged);

    public Color PreviewBackgroundColor
    {
        get => (Color)GetValue(PreviewBackgroundColorProperty);
        set => SetValue(PreviewBackgroundColorProperty, value);
    }

    public static readonly BindableProperty PreviewBordersColorProperty =
        BindableProperty.Create(
            nameof(PreviewBordersColor),
            typeof(Color),
            typeof(SpreadsheetGrid),
            defaultValue: Colors.Transparent,
            propertyChanged: OnPreviewBordersColorChanged);

    public Color PreviewBordersColor
    {
        get => (Color)GetValue(PreviewBordersColorProperty);
        set => SetValue(PreviewBordersColorProperty, value);
    }

    public static readonly BindableProperty PreviewSelectionBackgroundColorProperty =
        BindableProperty.Create(
            nameof(PreviewSelectionBackgroundColor),
            typeof(Color),
            typeof(SpreadsheetGrid),
            defaultValue: Colors.Transparent,
            propertyChanged: OnPreviewSelectionBackgroundColorChanged);

    public Color PreviewSelectionBackgroundColor
    {
        get => (Color)GetValue(PreviewSelectionBackgroundColorProperty);
        set => SetValue(PreviewSelectionBackgroundColorProperty, value);
    }

    public static readonly BindableProperty PreviewSelectionBordersColorProperty =
        BindableProperty.Create(
            nameof(PreviewSelectionBordersColor),
            typeof(Color),
            typeof(SpreadsheetGrid),
            defaultValue: Colors.Transparent,
            propertyChanged: OnPreviewSelectionBordersColorChanged);

    public Color PreviewSelectionBordersColor
    {
        get => (Color)GetValue(PreviewSelectionBordersColorProperty);
        set => SetValue(PreviewSelectionBordersColorProperty, value);
    }


    public static readonly BindableProperty PreviewTextColorProperty =
        BindableProperty.Create(
            nameof(PreviewTextColor),
            typeof(Color),
            typeof(SpreadsheetGrid),
            defaultValue: Colors.Transparent,
            propertyChanged: OnPreviewTextColorChanged);

    public Color PreviewTextColor
    {
        get => (Color)GetValue(PreviewTextColorProperty);
        set => SetValue(PreviewTextColorProperty, value);
    }


    public static readonly BindableProperty SelectionBackgroundColorProperty =
        BindableProperty.Create(
            nameof(SelectionBackgroundColor),
            typeof(Color),
            typeof(SpreadsheetGrid),
            defaultValue: Colors.Transparent,
            propertyChanged: OnSelectionBackgroundColorChanged);

    public Color SelectionBackgroundColor
    {
        get => (Color)GetValue(SelectionBackgroundColorProperty);
        set => SetValue(SelectionBackgroundColorProperty, value);
    }

    public static readonly BindableProperty SelectionBordersColorProperty =
        BindableProperty.Create(
            nameof(SelectionBordersColor),
            typeof(Color),
            typeof(SpreadsheetGrid),
            defaultValue: Colors.Transparent,
            propertyChanged: OnSelectionBordersColorChanged);

    public Color SelectionBordersColor
    {
        get => (Color)GetValue(SelectionBordersColorProperty);
        set => SetValue(SelectionBordersColorProperty, value);
    }

    public static readonly BindableProperty BordersColorProperty =
        BindableProperty.Create(
            nameof(BordersColor),
            typeof(Color),
            typeof(SpreadsheetGrid),
            defaultValue: Colors.Transparent,
            propertyChanged: OnBordersColorChanged);

    public Color BordersColor
    {
        get => (Color)GetValue(BordersColorProperty);
        set => SetValue(BordersColorProperty, value);
    }

    public static readonly BindableProperty OccupiedColor1Property =
        BindableProperty.Create(
            nameof(OccupiedColor1),
            typeof(Color),
            typeof(SpreadsheetGrid),
            Colors.Transparent,
            propertyChanged: OnOccupiedColor1Changed);

    public Color OccupiedColor1
    {
        get => (Color)GetValue(OccupiedColor1Property);
        set => SetValue(OccupiedColor1Property, value);
    }

    public static readonly BindableProperty OccupiedColor2Property =
        BindableProperty.Create(
            nameof(OccupiedColor2),
            typeof(Color),
            typeof(SpreadsheetGrid),
            Colors.Transparent,
            propertyChanged: OnOccupiedColor2Changed);

    public Color OccupiedColor2
    {
        get => (Color)GetValue(OccupiedColor2Property);
        set => SetValue(OccupiedColor2Property, value);
    }

    public static readonly BindableProperty OccupiedColor3Property =
        BindableProperty.Create(
            nameof(OccupiedColor3),
            typeof(Color),
            typeof(SpreadsheetGrid),
            Colors.Transparent,
            propertyChanged: OnOccupiedColor3Changed);

    public Color OccupiedColor3
    {
        get => (Color)GetValue(OccupiedColor3Property);
        set => SetValue(OccupiedColor3Property, value);
    }

    public static readonly BindableProperty OccupiedColor4Property =
        BindableProperty.Create(
            nameof(OccupiedColor4),
            typeof(Color),
            typeof(SpreadsheetGrid),
            Colors.Transparent,
            propertyChanged: OnOccupiedColor4Changed);

    public Color OccupiedColor4
    {
        get => (Color)GetValue(OccupiedColor4Property);
        set => SetValue(OccupiedColor4Property, value);
    }

    public int MaxRows
    {
        get => (int)GetValue(MaxRowsProperty);
        set => SetValue(MaxRowsProperty, value);
    }

    public static readonly BindableProperty MaxRowsProperty =
        BindableProperty.Create(
            nameof(MaxRows),
            typeof(int),
            typeof(SpreadsheetGrid),
            propertyChanged: OnMaxRowsChanged);

    public static readonly BindableProperty MaxColumnsProperty =
        BindableProperty.Create(
            nameof(MaxColumns),
            typeof(int),
            typeof(SpreadsheetGrid),
            propertyChanged: OnMaxColumnsChanged);

    public int MaxColumns
    {
        get => (int)GetValue(MaxColumnsProperty);
        set => SetValue(MaxColumnsProperty, value);
    }

    public static readonly BindableProperty TotalRowsProperty =
        BindableProperty.Create(
            nameof(TotalRows),
            typeof(int),
            typeof(SpreadsheetGrid),
            defaultValue: 0,
            defaultBindingMode: BindingMode.TwoWay);

    public int TotalRows
    {
        get => (int)GetValue(TotalRowsProperty);
        set => SetValue(TotalRowsProperty, value);
    }

    public static readonly BindableProperty TotalColumnsProperty =
        BindableProperty.Create(
            nameof(TotalColumns),
            typeof(int),
            typeof(SpreadsheetGrid),
            defaultValue: 0,
            defaultBindingMode: BindingMode.TwoWay);

    public int TotalColumns
    {
        get => (int)GetValue(TotalColumnsProperty);
        set => SetValue(TotalColumnsProperty, value);
    }

    public static readonly BindableProperty GridDefinedProperty =
        BindableProperty.Create(
            nameof(GridDefined),
            typeof(bool),
            typeof(SpreadsheetGrid),
            defaultValue: false,
            propertyChanged: OnGridDefinedChanged);

    public bool GridDefined
    {
        get => (bool)GetValue(GridDefinedProperty);
        set => SetValue(GridDefinedProperty, value);
    }

    public static readonly BindableProperty SelectedCellProperty =
        BindableProperty.Create(
            nameof(SelectedCell),
            typeof(CellRange),
            typeof(SpreadsheetGrid),
            default(CellRange),
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: OnSelectedCellChanged);

    public CellRange SelectedCell
    {
        get => (CellRange)GetValue(SelectedCellProperty);
        set => SetValue(SelectedCellProperty, value);
    }

    public static readonly BindableProperty GridViewModelProperty =
        BindableProperty.Create(
            nameof(GridViewModelProperty),
            typeof(CustomGridViewModel),
            typeof(SpreadsheetGrid),
            propertyChanged: OnGridViewModelChanged);

    public CustomGridViewModel GridViewModel
    {
        get => (CustomGridViewModel)GetValue(GridViewModelProperty);
        set => SetValue(GridViewModelProperty, value);
    }

    private static void OnGridViewModelChanged(BindableObject bindable,
        object? oldValue, object newValue)
    {
        if (bindable is not SpreadsheetGrid grid)
        {
            return;
        }

        if (oldValue is CustomGridViewModel oldModel)
        {
            oldModel.PartChanged -= grid.OnPartChanged;
        }

        if (newValue is CustomGridViewModel newModel)
        {
            grid.RecolorAll();
            newModel.PartChanged += grid.OnPartChanged;
            grid.Invalidate();
        }
    }

    protected override void OnHandlerChanged()
    {
        if (Handler is null)
        {
            GridViewModel?.PartChanged -= OnPartChanged;
        }
        base.OnHandlerChanged();
    }

    private void OnPartChanged(object? sender, PartChangedEventArgs e)
    {
        switch (e.Action)
        {
            case PartChangedAction.Clear: _areaColors.Clear(); break;
            case PartChangedAction.Add: ColorAddedCell(e.AffectedCell); break;
            case PartChangedAction.Remove: _areaColors.Remove(e.AffectedCell); break;
            case PartChangedAction.Update: return;
        }
        Invalidate();
    }

    private void ColorAddedCell(CellRange cell)
    {
        var used = GridViewModel.Parts.Keys
            .Where(other => other != cell && other.IsAdjacentTo(cell))
            .Select(other => _areaColors.GetValueOrDefault(other))
            .ToHashSet();

        var color = 0;
        while (used.Contains(color))
        {
            color++;
        }

        if (color < 4)
        {
            _areaColors[cell] = color;
            return;
        }

        RecolorAll();
    }

    private void RecolorAll()
    {
        var keys = GridViewModel.Parts.Keys.ToArray();

        var neighbors = keys.ToDictionary(cell => cell, _ => new List<CellRange>());

        for (var i = 0; i < keys.Length; i++)
        {
            for (var j = i + 1; j < keys.Length; j++)
            {
                var a = keys[i];
                var b = keys[j];

                if (a.IsAdjacentTo(b))
                {
                    neighbors[a].Add(b);
                    neighbors[b].Add(a);
                }
            }
        }

        _areaColors.Clear();

        var uncolored = new HashSet<CellRange>(keys);

        while (uncolored.Count > 0)
        {
            var area = uncolored
                .OrderByDescending(cell =>
                    neighbors[cell]
                        .Where(_areaColors.ContainsKey)
                        .Select(n => _areaColors[n])
                        .Distinct()
                        .Count())
                .ThenByDescending(cell => neighbors[cell].Count)
                .First();

            var usedColors = neighbors[area]
                .Where(_areaColors.ContainsKey)
                .Select(n => _areaColors[n])
                .ToHashSet();

            var color = 0;
            while (usedColors.Contains(color)) color++;

            _areaColors[area] = color;
            uncolored.Remove(area);
        }
    }

    private int _startRow, _startColumn;
    private int _currentRow, _currentColumn;

    private int _previewStartRow, _previewStartColumn;
    private int _previewCurrentRow, _previewCurrentColumn;

    private readonly Dictionary<CellRange, int> _areaColors = [];
    internal CellRange PreviewCell { get; private set; }

    internal Color GetAreaColor(CellRange area)
    {
        var index = _areaColors.GetValueOrDefault(area, 0);
        return index switch
        {
            0 => OccupiedColor1,
            1 => OccupiedColor2,
            2 => OccupiedColor3,
            3 => OccupiedColor4,
            _ => throw new NotSupportedException("Algorithm expects 4 colors")
        };
    }

    public SpreadsheetGrid()
    {
        Drawable = new GridDrawable(this);

        StartInteraction += OnStartInteraction;
        DragInteraction += OnDragInteraction;
        EndInteraction += OnEndInteraction;
    }

    private void OnStartInteraction(object? sender, TouchEventArgs e)
    {
        var point = e.Touches[0];

        if (GridDefined)
        {
            _startRow = GetRow(point.Y);
            _startColumn = GetColumn(point.X);

            _currentRow = _startRow;
            _currentColumn = _startColumn;

            UpdateSelection();
        }
        else
        {
            _previewStartRow = GetPreviewRow(point.Y);
            _previewStartColumn = GetPreviewColumn(point.X);

            _previewCurrentRow = _previewStartRow;
            _previewCurrentColumn = _previewStartColumn;

            UpdatePreviewSelection();
        }
    }

    private void OnDragInteraction(object? sender, TouchEventArgs e)
    {
        var point = e.Touches[0];

        if (GridDefined)
        {
            _currentRow = GetRow(point.Y);
            _currentColumn = GetColumn(point.X);

            UpdateSelection();
        }
        else
        {
            _previewCurrentColumn = GetPreviewColumn(point.X);
            _previewCurrentRow = GetPreviewRow(point.Y);

            UpdatePreviewSelection();
        }
    }

    private void OnEndInteraction(object? sender, TouchEventArgs e)
    {
        var point = e.Touches[0];

        if (GridDefined)
        {
            _currentColumn = GetColumn(point.X);
            _currentRow = GetRow(point.Y);

            UpdateSelection();
        }
        else
        {
            _previewCurrentColumn = GetPreviewColumn(point.X);
            _previewCurrentRow = GetPreviewRow(point.Y);

            UpdatePreviewSelection();
            CreateGridFromPreview();
        }
    }

    private void CreateGridFromPreview()
    {
        TotalRows = PreviewCell.RowSpan;
        TotalColumns = PreviewCell.ColumnSpan;

        SelectedCell = default;
        PreviewCell = default;

        Invalidate();
    }

    private int GetColumn(float x)
    {
        var cellWidth = (float)Width / TotalColumns;
        return Math.Clamp((int)(x / cellWidth), 0, TotalColumns - 1);
    }

    private int GetRow(float y)
    {
        var cellHeight = (float)Height / TotalRows;
        return Math.Clamp((int)(y / cellHeight), 0, TotalRows - 1);
    }

    private int GetPreviewColumn(float x)
    {
        var cellWidth = (float)Width / MaxColumns;
        return Math.Clamp((int)(x / cellWidth), 0, MaxColumns - 1);
    }

    private int GetPreviewRow(float y)
    {
        var cellHeight = (float)Height / MaxRows;
        return Math.Clamp((int)(y / cellHeight), 0, MaxRows - 1);
    }

    private void UpdatePreviewSelection()
    {
        var left = Math.Min(_previewStartColumn, _previewCurrentColumn);
        var top = Math.Min(_previewStartRow, _previewCurrentRow);
        var right = Math.Max(_previewStartColumn, _previewCurrentColumn);
        var bottom = Math.Max(_previewStartRow, _previewCurrentRow);
        var newCell = new CellRange(top, left, bottom - top + 1, right - left + 1);

        if (newCell == PreviewCell)
        {
            return;
        }
        PreviewCell = newCell;

        Invalidate();
    }

    private void UpdateSelection()
    {
        // Taps landing on an existing part snap to it.
        if (GridViewModel.Occupied(_currentRow, _currentColumn))
        {
            TrySelectOccupied(_currentRow, _currentColumn);
            return;
        }

        var left = Math.Min(_startColumn, _currentColumn);
        var top = Math.Min(_startRow, _currentRow);
        var right = Math.Max(_startColumn, _currentColumn);
        var bottom = Math.Max(_startRow, _currentRow);
        var newSelection = new CellRange(top, left, bottom - top + 1, right - left + 1);

        if (newSelection == SelectedCell)
        {
            return;
        }

        // Final selection must never overlap existing areas.
        if (GridViewModel.OverlapsWithExistingCells(newSelection))
        {
            return;
        }

        SelectedCell = newSelection;
        Invalidate();
    }

    public void TrySelectOccupied(int row, int column)
    {
        if (!GridViewModel.Occupied(row, column))
        {
            return;
        }

        var part = GridViewModel.Parts.Keys
            .First(cell => cell.Contains(row, column));

        if (part == SelectedCell)
        {
            return;
        }

        SelectedCell = part;
        Invalidate();
    }

    public void ClearGrid()
    {
        TotalRows = 0;
        TotalColumns = 0;

        SelectedCell = default;
        PreviewCell = default;

        Invalidate();
    }

    private static void RedrawGrid(BindableObject bindable) =>
        ((SpreadsheetGrid)bindable).Invalidate();

    private static void OnPreviewBackgroundColorChanged(BindableObject bindable,
        object oldValue, object newValue) => RedrawGrid(bindable);
    private static void OnPreviewBordersColorChanged(BindableObject bindable,
        object oldValue, object newValue) => RedrawGrid(bindable);
    private static void OnPreviewSelectionBackgroundColorChanged(BindableObject bindable,
        object oldValue, object newValue) => RedrawGrid(bindable);
    private static void OnPreviewSelectionBordersColorChanged(BindableObject bindable,
        object oldValue, object newValue) => RedrawGrid(bindable);
    private static void OnPreviewTextColorChanged(BindableObject bindable,
        object oldValue, object newValue) => RedrawGrid(bindable);
    private static void OnSelectionBackgroundColorChanged(BindableObject bindable,
        object oldValue, object newValue) => RedrawGrid(bindable);
    private static void OnSelectionBordersColorChanged(BindableObject bindable,
        object oldValue, object newValue) => RedrawGrid(bindable);
    private static void OnBordersColorChanged(BindableObject bindable,
        object oldValue, object newValue) => RedrawGrid(bindable);
    private static void OnOccupiedColor1Changed(BindableObject bindable,
        object oldValue, object newValue) => RedrawGrid(bindable);
    private static void OnOccupiedColor2Changed(BindableObject bindable,
        object oldValue, object newValue) => RedrawGrid(bindable);
    private static void OnOccupiedColor3Changed(BindableObject bindable,
        object oldValue, object newValue) => RedrawGrid(bindable);
    private static void OnOccupiedColor4Changed(BindableObject bindable,
        object oldValue, object newValue) => RedrawGrid(bindable);
    private static void OnMaxRowsChanged(BindableObject bindable,
        object oldValue, object newValue) => RedrawGrid(bindable);
    private static void OnMaxColumnsChanged(BindableObject bindable,
        object oldValue, object newValue) => RedrawGrid(bindable);

    private static void OnSelectedCellChanged(BindableObject bindable,
        object oldValue, object newValue) => RedrawGrid(bindable);

    private static void OnGridDefinedChanged(BindableObject bindable,
        object oldValue, object newValue) => RedrawGrid(bindable);
}
