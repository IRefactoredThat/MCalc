using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CalculatorApp.SettingsPage.Animations;

public enum OutputToInputAnimation { Morph, Push, None }
public enum ClickAnimation { Scale, Spring, None }
public enum TransitionAnimation { Fade, None }

public class AnimationSettings : INotifyPropertyChanged
{
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
            Preferences.Set(nameof(CalculatorClickAnimation), (int)value);
            field = value;
            OnPropertyChanged();
        }
    }

    public TransitionAnimation CalculatorTransitionAnimation
    {
        get;
        set
        {
            Preferences.Set(nameof(CalculatorTransitionAnimation), (int)value);
            field = value;
            OnPropertyChanged();
        }
    }

    public OutputToInputAnimation OutputToInputAnimation
    {
        get;
        set
        {
            Preferences.Set(nameof(OutputToInputAnimation), (int)value);
            field = value;
            OnPropertyChanged();
        }
    }

    public ClickAnimation SettingsClickAnimation
    {
        get;
        set
        {
            Preferences.Set(nameof(SettingsClickAnimation), (int)value);
            field = value;
            OnPropertyChanged();
        }
    }

    public TransitionAnimation SettingsTransitionAnimation
    {
        get;
        set
        {
            Preferences.Set(nameof(SettingsTransitionAnimation), (int)value);
            field = value;
            OnPropertyChanged();
        }
    }

    public AnimationSettings()
    {
        CalculatorClickAnimation = (ClickAnimation)
            Preferences.Get(nameof(CalculatorClickAnimation), 0);
        CalculatorTransitionAnimation = (TransitionAnimation)
            Preferences.Get(nameof(CalculatorTransitionAnimation), 0);
        OutputToInputAnimation = (OutputToInputAnimation)
            Preferences.Get(nameof(OutputToInputAnimation), 0);
        SettingsClickAnimation = (ClickAnimation)
            Preferences.Get(nameof(SettingsClickAnimation), 0);
        SettingsTransitionAnimation = (TransitionAnimation)
            Preferences.Get(nameof(SettingsTransitionAnimation), 0);
    }
}
