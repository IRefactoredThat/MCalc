using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CalculatorApp.CalculatorPage.Buttons;
using CalculatorApp.CalculatorPage.MainGrid.Cells;
using CalculatorApp.CalculatorPage.MainGrid.Models;
using CalculatorApp.CalculatorPage.MainGrid.Parts;
using CalculatorApp.CalculatorPage.Page;
using CalculatorApp.SettingsPage.Layout;

namespace CalculatorApp.SettingsPage.CustomLayout;

public class CustomLayoutViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private readonly UserMessageDisplayer _userMessageDisplayer;
    private readonly CustomModelStorage _customModelStorage;
    public LayoutSettings LayoutSettings { get; }
    public CustomGridViewModel CustomGridViewModel { get; }

    public bool GridDefined => UnitWidth > 0 && UnitHeight > 0;
    public bool IsCellSelected => SelectedCell != default;
    public bool IsCellOccupied => CustomGridViewModel.Parts.ContainsKey(SelectedCell);
    public bool SelectingSubgrid =>
        !CustomGridViewModel.Parts
            .GetValueOrDefault(SelectedCell)?
            .CanBeShownAsSingleButton(out _) ?? false;

    public int UnitWidth
    {
        get => CustomGridViewModel.UnitWidth;
        set
        {
            CustomGridViewModel.UnitWidth = Math.Clamp(value, 0, LayoutSettings.MaxUnitWidth);
            _customModelStorage.Save(CustomGridViewModel);
            OnPropertyChanged(nameof(IsCellOccupied));
            OnPropertyChanged(nameof(GridDefined));
        }
    }

    public int UnitHeight
    {
        get => CustomGridViewModel.UnitHeight;
        set
        {
            CustomGridViewModel.UnitHeight = Math.Clamp(value, 0, LayoutSettings.MaxUnitHeight);
            _customModelStorage.Save(CustomGridViewModel);
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

            if (CustomGridViewModel.Parts.TryGetValue(SelectedCell, out var part))
            {
                part.Add(model);
                CustomGridViewModel.Update(SelectedCell, part);
                _customModelStorage.Save(CustomGridViewModel);
            }
            else
            {
                var newPart = new GridViewModelPart(LayoutMode) { model };
                CustomGridViewModel.Add(SelectedCell, newPart);
                _customModelStorage.Save(CustomGridViewModel);
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

    public ICommand FullClearCommand { get; }

    private void FullClear()
    {
        LayoutItems.RestoreToInitialState();
        CustomGridViewModel.Clear();
        _customModelStorage.Delete();
        OnPropertyChanged(nameof(SelectingSubgrid));
        OnPropertyChanged(nameof(IsCellOccupied));
    }
    public ICommand IncreaseRowSpanCommand { get; }
    private void IncreaseRowSpan()
    {
        var part = CustomGridViewModel.Parts[SelectedCell];
        part.RowSpanExtension = Math.Clamp(part.RowSpanExtension + 1, 0, SelectedCell.RowSpan);
        CustomGridViewModel.Update(SelectedCell, part);
        _customModelStorage.Save(CustomGridViewModel);
    }
    public ICommand DecreaseRowSpanCommand { get; }
    private void DecreaseRowSpan()
    {
        var part = CustomGridViewModel.Parts[SelectedCell];
        part.RowSpanExtension = Math.Clamp(part.RowSpanExtension - 1, 0, SelectedCell.RowSpan);
        CustomGridViewModel.Update(SelectedCell, part);
        _customModelStorage.Save(CustomGridViewModel);
    }
    public ICommand IncreaseColumnSpanCommand { get; }
    private void IncreaseColumnSpan()
    {
        var part = CustomGridViewModel.Parts[SelectedCell];
        part.ColumnSpanExtension = Math.Clamp(part.ColumnSpanExtension + 1, 0, SelectedCell.ColumnSpan);
        CustomGridViewModel.Update(SelectedCell, part);
        _customModelStorage.Save(CustomGridViewModel);
    }
    public ICommand DecreaseColumnSpanCommand { get; }
    private void DecreaseColumnSpan()
    {
        var part = CustomGridViewModel.Parts[SelectedCell];
        part.ColumnSpanExtension = Math.Clamp(part.ColumnSpanExtension - 1, 0, SelectedCell.ColumnSpan);
        CustomGridViewModel.Update(SelectedCell, part);
        _customModelStorage.Save(CustomGridViewModel);
    }
    public ICommand ClearPartCommand { get; }
    private void ClearPart()
    {
        foreach (var model in CustomGridViewModel.Parts[SelectedCell])
        {
            LayoutItems.Add(model);
        }
        CustomGridViewModel.Remove(SelectedCell);
        OnPropertyChanged(nameof(SelectingSubgrid));
        OnPropertyChanged(nameof(IsCellOccupied));
        _customModelStorage.Save(CustomGridViewModel);
    }
    public ICommand ToggleLayoutModeCommand { get; }
    private void ToggleLayoutMode()
    {
        LayoutMode = LayoutMode == SubgridLayoutMode.Carousel ?
            SubgridLayoutMode.Grid : SubgridLayoutMode.Carousel;
        var part = CustomGridViewModel.Parts[SelectedCell];
        part.LayoutMode = LayoutMode;
        CustomGridViewModel.Update(SelectedCell, part);
        _customModelStorage.Save(CustomGridViewModel);
    }

    public ICommand RemoveButtonCommand { get; }
    private void RemoveButton(IBaseButtonViewModel model)
    {
        var (cell, part) = CustomGridViewModel.Parts
            .FirstOrDefault(part => part.Value.Contains(model));

        LayoutItems.Add(model);
        part.Remove(model);

        if (part.HasNoButtons)
        {
            CustomGridViewModel.Remove(cell);
        }
        else
        {
            CustomGridViewModel.Update(cell, part);
        }
        OnPropertyChanged(nameof(SelectingSubgrid));
        OnPropertyChanged(nameof(IsCellOccupied));

        _customModelStorage.Save(CustomGridViewModel);
    }

    public ICommand SelectCellCommand { get; }
    private void SelectCell(IBaseButtonViewModel model)
    {
        SelectedCell = CustomGridViewModel.Parts
            .FirstOrDefault(part => part.Value.Contains(model))
            .Key;
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
            action = () => LayoutSettings.ShowHelpVisible = false;
        }
        _userMessageDisplayer.ShowMessage(message, action);
    }

    public ObservableSortedList<IBaseButtonViewModel> LayoutItems { get; }

    public CustomLayoutViewModel(LayoutSettings layoutSettings,
        CustomModelStorage customModelStorage,
        UserMessageDisplayer userMessageDisplayer)
    {
        LayoutSettings = layoutSettings;
        _customModelStorage = customModelStorage;
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

        CustomGridViewModel = layoutSettings.CustomGridViewModel;
        LayoutItems = new ObservableSortedList<IBaseButtonViewModel>(
            customModelStorage.Pool);
        foreach (var part in CustomGridViewModel.Parts.Values)
        {
            foreach (var model in part)
            {
                LayoutItems.Remove(model);
            }
        }
    }
}
