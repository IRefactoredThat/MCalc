using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CalculatorApp.SettingsPage.History;

public enum EntryOverflowBehavior { Overwrite, Keep }
public enum EntryDuplicationBehavior { Allow, Ignore, Move }

public class HistorySettings : INotifyPropertyChanged
{
    private readonly IPreferences _preferences;
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public bool HistoryEnabled
    {
        get;
        set
        {
            field = value;
            _preferences.Set(nameof(HistoryEnabled), value);
            OnPropertyChanged();
        }
    }

    public int MaxEntryCount
    {
        get;
        set
        {
            field = value;
            _preferences.Set(nameof(MaxEntryCount), value);
            OnPropertyChanged();
        }
    }

    public EntryOverflowBehavior OverflowBehavior
    {
        get;
        set
        {
            field = value;
            _preferences.Set(nameof(OverflowBehavior), (int)value);
            OnPropertyChanged();
        }
    }

    public EntryDuplicationBehavior DuplicationBehavior
    {
        get;
        set
        {
            field = value;
            _preferences.Set(nameof(DuplicationBehavior), (int)value);
            OnPropertyChanged();
        }
    }

    public const int DefaultMaxEntryCount = 50, MinMaxEntryCount = 25, MaxMaxEntryCount = 200, EntryCountStep = 25;

    public HistorySettings(IPreferences preferences)
    {
        _preferences = preferences;
        HistoryEnabled = preferences.Get(nameof(HistoryEnabled), true);
        MaxEntryCount = preferences.Get(nameof(MaxEntryCount), DefaultMaxEntryCount);
        OverflowBehavior = (EntryOverflowBehavior)preferences.Get(nameof(OverflowBehavior), 0);
    }
}
