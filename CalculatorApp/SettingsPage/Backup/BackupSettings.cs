using System.Windows.Input;
using CalculatorApp.CalculatorPage.Page;

namespace CalculatorApp.SettingsPage.Backup;

public class BackupSettings
{
    private readonly UserMessageDisplayer _userMessageDisplayer;
    private static readonly PickOptions ImportOptions = new()
    {
        PickerTitle = "Select MCalc backup",
        FileTypes = new FilePickerFileType(new
            Dictionary<DevicePlatform, IEnumerable<string>>()
            {
                [DevicePlatform.Android] = ["application/mcbak"]
            })
    };

    public ICommand ImportCommand { get; }

    private async void Import()
    {
        var result = await FilePicker.PickAsync(ImportOptions);
        if (result is null)
        {
            _userMessageDisplayer.ShowMessage("Backup not selected", null);
        }
    }

    public ICommand ExportCommand { get; }

    private static async void Export()
    {
        await Share.RequestAsync("test");
    }

    public BackupSettings(UserMessageDisplayer userMessageDisplayer)
    {
        _userMessageDisplayer = userMessageDisplayer;
        ImportCommand = new Command(Import);
        ExportCommand = new Command(Export);
    }
}
