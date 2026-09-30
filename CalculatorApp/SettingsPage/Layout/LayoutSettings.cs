using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CalculatorApp.CalculatorPage.Buttons;
using CalculatorApp.CalculatorPage.MainGrid.Cells;
using CalculatorApp.CalculatorPage.MainGrid.Models;
using CalculatorApp.CalculatorPage.MainGrid.Parts;
using CalculatorApp.CalculatorPage.Page;

namespace CalculatorApp.SettingsPage.Layout;

public enum GridLayout { Simple, Mixed, Custom }
public enum LayoutItemCategory { Numbers, Primary, Secondary,
    Trigonometric, Hyperbolic, Special }
public enum ButtonShape { Circle, Square }

public class LayoutSettings : INotifyPropertyChanged
{
    private readonly SimpleGridViewModel _simpleGridViewModel;
    private readonly MixedGridViewModel _mixedGridViewModel;
    private readonly CustomGridViewModel _customGridViewModel;
    private readonly UserMessageDisplayer _userMessageDisplayer;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public GridLayout GridLayout
    {
        get;
        set
        {
            field = value;
            Preferences.Set(nameof(GridLayout), (int)value);
            OnPropertyChanged();
            OnPropertyChanged(nameof(GridViewModel));
        }
    }

    public static readonly Enum[] RequiredLayoutForCustomModel = [GridLayout.Custom];

    public const int MaxUnitWidth = 5, MaxUnitHeight = 10;

    public IGridViewModel GridViewModel => GridLayout switch
    {
        GridLayout.Simple => _simpleGridViewModel,
        GridLayout.Mixed => _mixedGridViewModel,
        _ => _customGridViewModel,
    };

    public bool GridDefined => UnitWidth > 0 && UnitHeight > 0;
    public bool IsCellSelected => SelectedCell != default;
    public bool IsCellOccupied => _customGridViewModel.Parts.ContainsKey(SelectedCell);
    public bool SelectingSubgrid =>
        !_customGridViewModel.Parts
            .GetValueOrDefault(SelectedCell)?
            .CanBeShownAsSingleButton(out _) ?? false;

    public int UnitWidth
    {
        get => _customGridViewModel.UnitWidth;
        set
        {
            _customGridViewModel.UnitWidth = Math.Clamp(value, 0, MaxUnitWidth);
            OnPropertyChanged(nameof(IsCellOccupied));
            OnPropertyChanged(nameof(GridDefined));
        }
    }

    public int UnitHeight
    {
        get => _customGridViewModel.UnitHeight;
        set
        {
            _customGridViewModel.UnitHeight = Math.Clamp(value, 0, MaxUnitHeight);
            OnPropertyChanged(nameof(IsCellOccupied));
            OnPropertyChanged(nameof(GridDefined));
        }
    }

    public CellRange SelectedCell
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SelectingSubgrid));
            OnPropertyChanged(nameof(IsCellOccupied));
            OnPropertyChanged(nameof(IsCellSelected));
        }
    }

    public object? SelectedItem
    {
        get;
        set
        {
            if (value is not IBaseButtonViewModel model)
            {
                return;
            }

            if (_customGridViewModel.Parts.TryGetValue(SelectedCell, out var part))
            {
                part.Add(model);
                _customGridViewModel.Update(SelectedCell, part);
            }
            else
            {
                var newPart = new GridViewModelPart(LayoutMode) { model };
                _customGridViewModel.Add(SelectedCell, newPart);
            }

            LayoutItems.Remove(model);

            // Explicitly clear selection so collection updates
            // properly after repeated removal/insertion
            field = null;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SelectingSubgrid));
            OnPropertyChanged(nameof(IsCellOccupied));
        }
    }

    public SubgridLayoutMode LayoutMode
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public const double MinButtonSpacing = 5, MaxButtonSpacing = 15,
        DefaultButtonSpacing = 10, ButtonSpacingStep = 0.5;

    public double ButtonSpacing
    {
        get;
        set
        {
            field = value;
            Preferences.Set(nameof(ButtonSpacing), value);
            OnPropertyChanged();
        }
    }

    public ICommand FullClearCommand { get; }

    private void FullClear()
    {
        LayoutItems.RestoreToInitialState();
        _customGridViewModel.Clear();
        OnPropertyChanged(nameof(SelectingSubgrid));
        OnPropertyChanged(nameof(IsCellOccupied));
    }
    public ICommand IncreaseRowSpanCommand { get; }
    private void IncreaseRowSpan()
    {
        var part = _customGridViewModel.Parts[SelectedCell];
        part.RowSpanExtension = Math.Clamp(part.RowSpanExtension + 1, 0, SelectedCell.RowSpan);
        _customGridViewModel.Update(SelectedCell, part);
    }
    public ICommand DecreaseRowSpanCommand { get; }
    private void DecreaseRowSpan()
    {
        var part = _customGridViewModel.Parts[SelectedCell];
        part.RowSpanExtension = Math.Clamp(part.RowSpanExtension - 1, 0, SelectedCell.RowSpan);
        _customGridViewModel.Update(SelectedCell, part);
    }
    public ICommand IncreaseColumnSpanCommand { get; }
    private void IncreaseColumnSpan()
    {
        var part = _customGridViewModel.Parts[SelectedCell];
        part.ColumnSpanExtension = Math.Clamp(part.ColumnSpanExtension + 1, 0, SelectedCell.ColumnSpan);
        _customGridViewModel.Update(SelectedCell, part);
    }
    public ICommand DecreaseColumnSpanCommand { get; }
    private void DecreaseColumnSpan()
    {
        var part = _customGridViewModel.Parts[SelectedCell];
        part.ColumnSpanExtension = Math.Clamp(part.ColumnSpanExtension - 1, 0, SelectedCell.ColumnSpan);
        _customGridViewModel.Update(SelectedCell, part);
    }
    public ICommand ClearPartCommand { get; }
    private void ClearPart()
    {
        foreach (var model in _customGridViewModel.Parts[SelectedCell])
        {
            LayoutItems.Add(model);
        }
        _customGridViewModel.Remove(SelectedCell);
        OnPropertyChanged(nameof(SelectingSubgrid));
        OnPropertyChanged(nameof(IsCellOccupied));
    }
    public ICommand ToggleLayoutModeCommand { get; }
    private void ToggleLayoutMode()
    {
        LayoutMode = LayoutMode == SubgridLayoutMode.Carousel ?
            SubgridLayoutMode.Grid : SubgridLayoutMode.Carousel;
        var part = _customGridViewModel.Parts[SelectedCell];
        part.LayoutMode = LayoutMode;
        _customGridViewModel.Update(SelectedCell, part);
    }

    public ICommand RemoveButtonCommand { get; }
    private void RemoveButton(IBaseButtonViewModel model)
    {
        var (cell, part) = _customGridViewModel.Parts
            .FirstOrDefault(part => part.Value.Contains(model));

        LayoutItems.Add(model);
        part.Remove(model);

        if (part.HasNoButtons)
        {
            _customGridViewModel.Remove(cell);
        }
        else
        {
            _customGridViewModel.Update(cell, part);
        }

        OnPropertyChanged(nameof(SelectingSubgrid));
        OnPropertyChanged(nameof(IsCellOccupied));
    }

    public ICommand SelectCellCommand { get; }
    private void SelectCell(IBaseButtonViewModel model)
    {
        SelectedCell = _customGridViewModel.Parts
            .FirstOrDefault(part => part.Value.Contains(model))
            .Key;
    }

    public bool ShowHelpVisible
    {
        get;
        set
        {
            field = value;
            Preferences.Set(nameof(ShowHelpVisible), value);
            OnPropertyChanged();
        }
    }
    public ICommand ShowHelpCommand { get; }

    private void ShowHelp()
    {
        string message;
        Action? action = null;
        if (!GridDefined)
        {
            message = "Define the grid by dragging across and selecting desired scale";
        }
        else if(!IsCellSelected)
        {
            message = "Select cell by dragging across and selecting desired scale or clicking";
        }
        else if (!IsCellOccupied)
        {
            message = "Click on labels below to add them to current selection";
        }
        else
        {
            message = "Buttons on the side allow clearing whole grid, and " +
                      "expanding, collapsing, clearing and changing layout " +
                      "of current selection";
            action = () => ShowHelpVisible = false;
        }
        _userMessageDisplayer.ShowMessage(message, action);
    }

    public ObservableSortedList<IBaseButtonViewModel> LayoutItems { get; }

    public LayoutItemCategory ActiveCategory
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            Preferences.Set(nameof(ActiveCategory), (int)ActiveCategory);
        }
    }

    public ButtonShape ButtonShape
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            Preferences.Set(nameof(ButtonShape), (int)ButtonShape);
        }
    }

    public LayoutSettings(SimpleGridViewModel simpleViewModel,
        MixedGridViewModel mixedViewModel, CustomGridViewModel customViewModel,
        UserMessageDisplayer userMessageDisplayer)
    {
        _simpleGridViewModel = simpleViewModel;
        _mixedGridViewModel = mixedViewModel;
        _customGridViewModel = customViewModel;
        _userMessageDisplayer = userMessageDisplayer;

        FullClearCommand = new Command(FullClear);
        ToggleLayoutModeCommand = new Command(ToggleLayoutMode);
        IncreaseRowSpanCommand = new Command(IncreaseRowSpan);
        DecreaseRowSpanCommand = new Command(DecreaseRowSpan);
        IncreaseColumnSpanCommand = new Command(IncreaseColumnSpan);
        DecreaseColumnSpanCommand = new Command(DecreaseColumnSpan);
        ClearPartCommand = new Command(ClearPart);
        RemoveButtonCommand = new Command<IBaseButtonViewModel>(RemoveButton);
        SelectCellCommand = new Command<IBaseButtonViewModel>(SelectCell);
        ShowHelpCommand = new Command(ShowHelp);

        LayoutItems = new ObservableSortedList<IBaseButtonViewModel>(
            _customGridViewModel.Pool);
        foreach (var part in _customGridViewModel.Parts.Values)
        {
            foreach (var model in part)
            {
                LayoutItems.Remove(model);
            }
        }

        ShowHelpVisible = Preferences.Get(nameof(ShowHelpVisible), true);
        GridLayout = (GridLayout)Preferences.Get(nameof(GridLayout), 0);
        ActiveCategory = (LayoutItemCategory)Preferences.Get(nameof(ActiveCategory), 0);
        ButtonSpacing = Preferences.Get(nameof(ButtonSpacing), DefaultButtonSpacing);
        ButtonShape = (ButtonShape)Preferences.Get(nameof(ButtonShape), 0);
    }
}
