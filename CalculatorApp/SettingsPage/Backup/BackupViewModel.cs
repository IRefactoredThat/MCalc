using System.Text.Json;
using System.Windows.Input;
using CalculatorApp.CalculatorPage.History.Converters;
using CalculatorApp.CalculatorPage.History.Storage;
using CalculatorApp.CalculatorPage.MainGrid.Models;
using CalculatorApp.CalculatorPage.MainGrid.Parts;
using CalculatorApp.CalculatorPage.Page;
using CommunityToolkit.Maui.Storage;

namespace CalculatorApp.SettingsPage.Backup;

public class BackupViewModel
{
    private readonly IReadOnlyList<IReload> _reloadableSettings;
    private readonly UserMessageDisplayer _userMessageDisplayer;
    private readonly HistoryStorage _historyStorage;
    private readonly CustomModelStorage _modelStorage;
    private readonly BackupFileJsonContext _jsonContext;

    public BackupSettings BackupSettings { get; }

    public ICommand ToggleBackupPreferencesCommand { get; }
    private void ToggleBackupPreferences() =>
        BackupSettings.BackupPreferences = !BackupSettings.BackupPreferences;

    public ICommand ToggleBackupHistoryCommand { get; }
    private void ToggleBackupHistory() =>
        BackupSettings.BackupHistory = !BackupSettings.BackupHistory;

    public ICommand ToggleBackupCustomModelCommand { get; }
    private void ToggleBackupCustomModel() =>
        BackupSettings.BackupCustomModel = !BackupSettings.BackupCustomModel;

    public ICommand ImportCommand { get; }

    private async void Import()
    {
        try
        {
            var result = await FilePicker.PickAsync();
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
        if (BackupSettings.BackupPreferences && backup.Preferences is { } preferences)
        {
            BackupSettings.Preferences.Save(preferences);
        }
        if (BackupSettings.BackupCustomModel && backup.CustomGridViewModel is { } customModel)
        {
            _modelStorage.Save(customModel);
        }
        if (BackupSettings.BackupHistory && backup.HistoryItems is { } historyItems)
        {
            _historyStorage.Save(historyItems);
        }

        foreach (var service in _reloadableSettings)
        {
            service.Reload();
        }
        BackupSettings.Reload();
    }

    public ICommand ExportCommand { get; }

    private async void Export()
    {
        try
        {
            var backupFile = new BackupFile(
                Preferences: BackupSettings.BackupPreferences ?
                    BackupSettings.Preferences.Get() : null,
                CustomGridViewModel: BackupSettings.BackupCustomModel ?
                    _modelStorage.Get() : null,
                HistoryItems: BackupSettings.BackupHistory ?
                    _historyStorage.Get() : null
            );

            await using var stream = File.Create(BackupSettings.BackupCachedPath);
            await JsonSerializer.SerializeAsync(stream,
                backupFile, _jsonContext.BackupFile);
            stream.Position = 0;
            await FileSaver.SaveAsync(BackupSettings.BackupName, stream);
            _userMessageDisplayer.ShowMessage("Backup exported", null);
        }
        catch (Exception)
        {
            _userMessageDisplayer.ShowMessage("Backup export failed", null);
        }
    }

    public BackupViewModel(UserMessageDisplayer userMessageDisplayer,
        HistoryStorage historyStorage,
        CustomModelStorage modelStorage,
        IReadOnlyList<IReload> reloadableSettings,
        TokenConverter tokenConverter,
        NumberTokenConverter numberTokenConverter,
        BackupSettings backupSettings)
    {
        _userMessageDisplayer = userMessageDisplayer;
        _historyStorage = historyStorage;
        _modelStorage = modelStorage;
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
        BackupSettings = backupSettings;

        ImportCommand = new Command(Import);
        ExportCommand = new Command(Export);
        ToggleBackupPreferencesCommand = new Command(ToggleBackupPreferences);
        ToggleBackupHistoryCommand = new Command(ToggleBackupHistory);
        ToggleBackupCustomModelCommand = new Command(ToggleBackupCustomModel);
    }
}
