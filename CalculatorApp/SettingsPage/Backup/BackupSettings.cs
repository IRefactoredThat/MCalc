using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using CalculatorApp.CalculatorPage.History;
using CalculatorApp.CalculatorPage.MainGrid.Models;
using CalculatorApp.SettingsPage.Base;

namespace CalculatorApp.SettingsPage.Backup;

public record BackupFile(Preference[]? Preferences,
    CustomGridViewModel? CustomGridViewModel,
    ObservableCollection<HistoryItem>? HistoryItems);

[JsonSerializable(typeof(BackupFile))]
internal sealed partial class BackupFileJsonContext : JsonSerializerContext { }

public class BackupSettings(SettingsPreferences preferences) :
    INotifyPropertyChanged, IReload
{
    public SettingsPreferences Preferences { get; } = preferences;

    public const string BackupName = "mcalc.mcbak";
    public static readonly string BackupCachedPath =
        Path.Combine(FileSystem.CacheDirectory, BackupName);

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public bool BackupPreferences
    {
        get;
        set
        {
            field = value;
            Preferences.Set(nameof(BackupPreferences), value);
            OnPropertyChanged();
        }
    }

    public bool BackupHistory
    {
        get;
        set
        {
            field = value;
            Preferences.Set(nameof(BackupHistory), value);
            OnPropertyChanged();
        }
    }

    public bool BackupCustomModel
    {
        get;
        set
        {
            field = value;
            Preferences.Set(nameof(BackupCustomModel), value);
            OnPropertyChanged();
        }
    }

    public void Reload()
    {
        BackupPreferences = Preferences.Get(nameof(BackupPreferences), true);
        BackupHistory = Preferences.Get(nameof(BackupHistory), true);
        BackupCustomModel = Preferences.Get(nameof(BackupCustomModel), true);
    }
}
