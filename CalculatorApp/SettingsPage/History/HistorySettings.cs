using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CalculatorApp.SettingsPage.History;

public enum EntryOverflowBehavior { Overwrite, Keep }
public enum EntryDuplicationBehavior { Allow, Ignore, Move }

public class HistorySettings : INotifyPropertyChanged
{
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
            Preferences.Set(nameof(HistoryEnabled), value);
            OnPropertyChanged();
        }
    }

    public int MaxEntryCount
    {
        get;
        set
        {
            field = value;
            Preferences.Set(nameof(MaxEntryCount), value);
            OnPropertyChanged();
        }
    }

    public EntryOverflowBehavior OverflowBehavior
    {
        get;
        set
        {
            field = value;
            Preferences.Set(nameof(OverflowBehavior), (int)value);
            OnPropertyChanged();
        }
    }

    public EntryDuplicationBehavior DuplicationBehavior
    {
        get;
        set
        {
            field = value;
            Preferences.Set(nameof(DuplicationBehavior), (int)value);
            OnPropertyChanged();
        }
    }

    public const int DefaultMaxEntryCount = 50, MinMaxEntryCount = 25, MaxMaxEntryCount = 200, EntryCountStep = 25;

    public HistorySettings()
    {
        HistoryEnabled = Preferences.Get(nameof(HistoryEnabled), true);
        MaxEntryCount = Preferences.Get(nameof(MaxEntryCount), DefaultMaxEntryCount);
        OverflowBehavior = (EntryOverflowBehavior)Preferences.Get(nameof(OverflowBehavior), 0);
    }
}