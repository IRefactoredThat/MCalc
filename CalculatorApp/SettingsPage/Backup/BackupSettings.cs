using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Input;
using CalculatorApp.CalculatorPage.History;
using CalculatorApp.CalculatorPage.History.Storage;
using CalculatorApp.CalculatorPage.MainGrid.Models;
using CalculatorApp.CalculatorPage.Page;
using CalculatorApp.SettingsPage.Base;
using CommunityToolkit.Maui.Storage;

namespace CalculatorApp.SettingsPage.Backup;

public record BackupFile(Preference[] Preferences,
    CustomGridViewModel CustomGridViewModel,
    ObservableCollection<HistoryItem> HistoryItems);

[JsonSerializable(typeof(BackupFile))]
internal sealed partial class BackupFileJsonContext : JsonSerializerContext { }

public class BackupSettings
{
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

            _userMessageDisplayer.ShowMessage("Backup imported", null);
            Apply(backup);
        }
        catch (JsonException)
        {
            _userMessageDisplayer.ShowMessage("Backup has invalid format", null);
        }
        catch (Exception)
        {
            _userMessageDisplayer.ShowMessage("Backup import cancelled", null);
        }
    }

    private void Apply(BackupFile backup)
    {
        _modelStorage.Save(backup.CustomGridViewModel);
        _historyStorage.Save(backup.HistoryItems);
        _preferences.Save(backup.Preferences);
    }

    public ICommand ExportCommand { get; }

    private async void Export()
    {
        try
        {
            var backupFile = new BackupFile(
                Preferences: _preferences.Get(),
                CustomGridViewModel: _modelStorage.Get(),
                HistoryItems: _historyStorage.Get()
            );

            await using var stream = File.Create(BackupCachedPath);
            await JsonSerializer.SerializeAsync(stream, backupFile,
                BackupFileJsonContext.Default.BackupFile);
            stream.Position = 0;
            await FileSaver.SaveAsync(BackupName, stream);
            _userMessageDisplayer.ShowMessage("Backup exported", null);
        }
        catch (Exception)
        {
            _userMessageDisplayer.ShowMessage("Backup export cancelled", null);
        }
    }

    public BackupSettings(UserMessageDisplayer userMessageDisplayer,
        HistoryStorage historyStorage, CustomModelStorage modelStorage,
        SettingsPreferences preferences)
    {
        _userMessageDisplayer = userMessageDisplayer;
        _historyStorage = historyStorage;
        _modelStorage = modelStorage;
        _preferences = preferences;
        _jsonContext = new BackupFileJsonContext();

        ImportCommand = new Command(Import);
        ExportCommand = new Command(Export);
    }
}
