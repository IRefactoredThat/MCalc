using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Input;
using CalculatorApp.CalculatorPage.History;
using CalculatorApp.CalculatorPage.History.Converters;
using CalculatorApp.CalculatorPage.History.Storage;
using CalculatorApp.CalculatorPage.MainGrid.Models;
using CalculatorApp.CalculatorPage.MainGrid.Parts;
using CalculatorApp.CalculatorPage.Page;
using CalculatorApp.SettingsPage.Base;
using CommunityToolkit.Maui.Storage;

namespace CalculatorApp.SettingsPage.Backup;

public record BackupFile(Preference[]? Preferences,
    CustomGridViewModel? CustomGridViewModel,
    ObservableCollection<HistoryItem>? HistoryItems);

[JsonSerializable(typeof(BackupFile))]
internal sealed partial class BackupFileJsonContext : JsonSerializerContext { }

public class BackupSettings : INotifyPropertyChanged, IReload
{
    private readonly IReadOnlyList<IReload> _reloadableSettings;
    private readonly UserMessageDisplayer _userMessageDisplayer;
    private readonly HistoryStorage _historyStorage;
    private readonly CustomModelStorage _modelStorage;
    private readonly SettingsPreferences _preferences;
    private readonly BackupFileJsonContext _jsonContext;

    private static readonly PickOptions ImportOptions = new()
    {
        PickerTitle = "Select MCalc backup"
    };
    private const string BackupName = "mcalc.mcbak";
    private static readonly string BackupCachedPath =
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
            _preferences.Set(nameof(BackupPreferences), value);
            OnPropertyChanged();
        }
    }

    public bool BackupHistory
    {
        get;
        set
        {
            field = value;
            _preferences.Set(nameof(BackupHistory), value);
            OnPropertyChanged();
        }
    }

    public bool BackupCustomModel
    {
        get;
        set
        {
            field = value;
            _preferences.Set(nameof(BackupCustomModel), value);
            OnPropertyChanged();
        }
    }

    public ICommand ImportCommand { get; }

    private async void Import()
    {
        try
        {
            var result = await FilePicker.PickAsync(ImportOptions);
            if (result is null)
            {
                _userMessageDisplayer.ShowMessage("Backup not selected", null);
                return;
            }

            await using var stream = File.OpenRead(result.FullPath);
            var backup = await JsonSerializer.DeserializeAsync<BackupFile>(
                stream, _jsonContext.BackupFile);
            if (backup is null)
            {
                _userMessageDisplayer.ShowMessage("Backup not found", null);
                return;
            }

            ImportFrom(backup);
            _userMessageDisplayer.ShowMessage("Backup imported", null);
        }
        catch (JsonException)
        {
            _userMessageDisplayer.ShowMessage("Backup has invalid format", null);
        }
        catch (Exception)
        {
            _userMessageDisplayer.ShowMessage("Backup import failed", null);
        }
    }

    private void ImportFrom(BackupFile backup)
    {
        if (BackupPreferences && backup.Preferences is { } preferences)
        {
            _preferences.Save(preferences);
        }
        if (BackupCustomModel && backup.CustomGridViewModel is { } customModel)
        {
            _modelStorage.Save(customModel);
        }
        if (BackupHistory && backup.HistoryItems is { } historyItems)
        {
            _historyStorage.Save(historyItems);
        }

        foreach (var service in _reloadableSettings)
        {
            service.Reload();
        }
        Reload();
    }

    public ICommand ExportCommand { get; }

    private async void Export()
    {
        try
        {
            var backupFile = new BackupFile(
                Preferences: BackupPreferences ?
                    _preferences.Get() : null,
                CustomGridViewModel: BackupCustomModel ?
                    _modelStorage.Get() : null,
                HistoryItems: BackupHistory ?
                    _historyStorage.Get() : null
            );

            await using var stream = File.Create(BackupCachedPath);
            await JsonSerializer.SerializeAsync(stream,
                backupFile, _jsonContext.BackupFile);
            stream.Position = 0;
            await FileSaver.SaveAsync(BackupName, stream);
            _userMessageDisplayer.ShowMessage("Backup exported", null);
        }
        catch (Exception)
        {
            _userMessageDisplayer.ShowMessage("Backup export failed", null);
        }
    }

    public BackupSettings(UserMessageDisplayer userMessageDisplayer,
        HistoryStorage historyStorage, CustomModelStorage modelStorage,
        SettingsPreferences preferences, IReadOnlyList<IReload> reloadableSettings,
        TokenConverter tokenConverter, NumberTokenConverter numberTokenConverter)
    {
        _userMessageDisplayer = userMessageDisplayer;
        _historyStorage = historyStorage;
        _modelStorage = modelStorage;
        _preferences = preferences;
        _reloadableSettings = reloadableSettings;
        _jsonContext = new BackupFileJsonContext(new JsonSerializerOptions
        {
            Converters =
            {
                new GridPartJsonConverter(modelStorage.Pool),
                tokenConverter,
                numberTokenConverter,
            },
        });

        ImportCommand = new Command(Import);
        ExportCommand = new Command(Export);
    }

    public void Reload()
    {
        BackupPreferences = _preferences.Get(nameof(BackupPreferences), true);
        BackupHistory = _preferences.Get(nameof(BackupHistory), true);
        BackupCustomModel = _preferences.Get(nameof(BackupCustomModel), true);
    }
}
