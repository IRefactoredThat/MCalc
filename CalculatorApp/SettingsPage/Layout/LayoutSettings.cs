using System.ComponentModel;
using System.Runtime.CompilerServices;
using CalculatorApp.CalculatorPage.MainGrid.Models;
using CalculatorApp.SettingsPage.Backup;

namespace CalculatorApp.SettingsPage.Layout;

public enum GridLayout { Simple, Mixed, Custom }
public enum LayoutItemCategory { Numbers, Primary, Secondary,
    Trigonometric, Hyperbolic, Special }
public enum ButtonShape { Circle, Square }

public class LayoutSettings(
    IPreferences preferences,
    CustomModelStorage customModelStorage,
    SimpleGridViewModel simpleGridViewModel,
    MixedGridViewModel mixedGridViewModel)
    : INotifyPropertyChanged, IReload
{
    private Lazy<CustomGridViewModel> _customGridViewModel = new(customModelStorage.Get);

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
            preferences.Set(nameof(GridLayout), (int)value);
            OnPropertyChanged();
            OnPropertyChanged(nameof(GridViewModel));
            GridViewModel.Update(IncludeACButton);
        }
    }

    public CustomGridViewModel CustomGridViewModel => _customGridViewModel.Value;

    public IGridViewModel GridViewModel => GridLayout switch
    {
        GridLayout.Simple => simpleGridViewModel,
        GridLayout.Mixed => mixedGridViewModel,
        _ => CustomGridViewModel
    };

    public static readonly Enum[] RequiredLayoutForCustomModel = [GridLayout.Custom];

    public const int MaxUnitWidth = 5, MaxUnitHeight = 10;

    public const double MinButtonSpacing = 5, MaxButtonSpacing = 15,
        DefaultButtonSpacing = 10, ButtonSpacingStep = 0.5;

    public double ButtonSpacing
    {
        get;
        set
        {
            field = value;
            preferences.Set(nameof(ButtonSpacing), value);
            OnPropertyChanged();
        }
    }

    public bool IncludeACButton
    {
        get;
        set
        {
            field = value;
            preferences.Set(nameof(IncludeACButton), value);
            GridViewModel.Update(value);
            OnPropertyChanged();
        }
    }

    public bool ShowHelpVisible
    {
        get;
        set
        {
            field = value;
            preferences.Set(nameof(ShowHelpVisible), value);
            OnPropertyChanged();
        }
    }

    public LayoutItemCategory ActiveCategory
    {
        get;
        set
        {
            field = value;
            preferences.Set(nameof(ActiveCategory), (int)ActiveCategory);
            OnPropertyChanged();
        }
    }

    public ButtonShape ButtonShape
    {
        get;
        set
        {
            field = value;
            preferences.Set(nameof(ButtonShape), (int)ButtonShape);
            OnPropertyChanged();
        }
    }

    public void Reload()
    {
        _customGridViewModel = new Lazy<CustomGridViewModel>(customModelStorage.Get);
        ShowHelpVisible = preferences.Get(nameof(ShowHelpVisible), true);
        GridLayout = (GridLayout)preferences.Get(nameof(GridLayout), 0);
        ActiveCategory = (LayoutItemCategory)preferences.Get(nameof(ActiveCategory), 0);
        ButtonSpacing = preferences.Get(nameof(ButtonSpacing), DefaultButtonSpacing);
        ButtonShape = (ButtonShape)preferences.Get(nameof(ButtonShape), 0);
        IncludeACButton = preferences.Get(nameof(IncludeACButton), false);
        OnPropertyChanged(nameof(GridViewModel));
    }
}
