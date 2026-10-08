using System.ComponentModel;
using System.Runtime.CompilerServices;
using CalculatorApp.CalculatorPage.MainGrid.Models;

namespace CalculatorApp.SettingsPage.Layout;

public enum GridLayout { Simple, Mixed, Custom }
public enum LayoutItemCategory { Numbers, Primary, Secondary,
    Trigonometric, Hyperbolic, Special }
public enum ButtonShape { Circle, Square }

public class LayoutSettings : INotifyPropertyChanged
{
    private readonly IPreferences _preferences;
    private readonly SimpleGridViewModel _simpleGridViewModel;
    private readonly MixedGridViewModel _mixedGridViewModel;
    private readonly Lazy<CustomGridViewModel> _customGridViewModel;

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
            _preferences.Set(nameof(GridLayout), (int)value);
            OnPropertyChanged();
            OnPropertyChanged(nameof(GridViewModel));
        }
    }

    public CustomGridViewModel CustomGridViewModel => _customGridViewModel.Value;

    public IGridViewModel GridViewModel => GridLayout switch
    {
        GridLayout.Simple => _simpleGridViewModel,
        GridLayout.Mixed => _mixedGridViewModel,
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
            _preferences.Set(nameof(ButtonSpacing), value);
            OnPropertyChanged();
        }
    }

    public bool ShowHelpVisible
    {
        get;
        set
        {
            field = value;
            _preferences.Set(nameof(ShowHelpVisible), value);
            OnPropertyChanged();
        }
    }

    public LayoutItemCategory ActiveCategory
    {
        get;
        set
        {
            field = value;
            _preferences.Set(nameof(ActiveCategory), (int)ActiveCategory);
            OnPropertyChanged();
        }
    }

    public ButtonShape ButtonShape
    {
        get;
        set
        {
            field = value;
            _preferences.Set(nameof(ButtonShape), (int)ButtonShape);
            OnPropertyChanged();
        }
    }

    public LayoutSettings(IPreferences preferences, CustomModelStorage modelStorage,
        SimpleGridViewModel simpleGridViewModel, MixedGridViewModel mixedGridViewModel)
    {
        _preferences = preferences;
        _simpleGridViewModel = simpleGridViewModel;
        _mixedGridViewModel = mixedGridViewModel;
        _customGridViewModel = new Lazy<CustomGridViewModel>(modelStorage.Get);

        ShowHelpVisible = _preferences.Get(nameof(ShowHelpVisible), true);
        GridLayout = (GridLayout)_preferences.Get(nameof(GridLayout), 0);
        ActiveCategory = (LayoutItemCategory)_preferences.Get(nameof(ActiveCategory), 0);
        ButtonSpacing = _preferences.Get(nameof(ButtonSpacing), DefaultButtonSpacing);
        ButtonShape = (ButtonShape)_preferences.Get(nameof(ButtonShape), 0);
    }
}
