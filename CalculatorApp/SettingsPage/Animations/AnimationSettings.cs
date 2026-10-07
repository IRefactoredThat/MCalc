using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CalculatorApp.SettingsPage.Animations;

public enum OutputToInputAnimation { Morph, Push, None }
public enum ClickAnimation { Scale, Spring, None }
public enum TransitionAnimation { Fade, None }

public class AnimationSettings : INotifyPropertyChanged
{
    private readonly IPreferences _preferences;
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public ClickAnimation CalculatorClickAnimation
    {
        get;
        set
        {
            _preferences.Set(nameof(CalculatorClickAnimation), (int)value);
            field = value;
            OnPropertyChanged();
        }
    }

    public TransitionAnimation CalculatorTransitionAnimation
    {
        get;
        set
        {
            _preferences.Set(nameof(CalculatorTransitionAnimation), (int)value);
            field = value;
            OnPropertyChanged();
        }
    }

    public OutputToInputAnimation OutputToInputAnimation
    {
        get;
        set
        {
            _preferences.Set(nameof(OutputToInputAnimation), (int)value);
            field = value;
            OnPropertyChanged();
        }
    }

    public ClickAnimation SettingsClickAnimation
    {
        get;
        set
        {
            _preferences.Set(nameof(SettingsClickAnimation), (int)value);
            field = value;
            OnPropertyChanged();
        }
    }

    public TransitionAnimation SettingsTransitionAnimation
    {
        get;
        set
        {
            _preferences.Set(nameof(SettingsTransitionAnimation), (int)value);
            field = value;
            OnPropertyChanged();
        }
    }

    public AnimationSettings(IPreferences preferences)
    {
        _preferences = preferences;

        CalculatorClickAnimation = (ClickAnimation)
            preferences.Get(nameof(CalculatorClickAnimation), 0);
        CalculatorTransitionAnimation = (TransitionAnimation)
            preferences.Get(nameof(CalculatorTransitionAnimation), 0);
        OutputToInputAnimation = (OutputToInputAnimation)
            preferences.Get(nameof(OutputToInputAnimation), 0);
        SettingsClickAnimation = (ClickAnimation)
            preferences.Get(nameof(SettingsClickAnimation), 0);
        SettingsTransitionAnimation = (TransitionAnimation)
            preferences.Get(nameof(SettingsTransitionAnimation), 0);
    }
}
