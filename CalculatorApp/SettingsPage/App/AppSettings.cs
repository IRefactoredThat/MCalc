using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Input;
using Android.Runtime;
using CalculatorApp.CalculatorPage.Page;

namespace CalculatorApp.SettingsPage.App;

using Shell = Microsoft.Maui.Controls.Shell;

public record CapturedException(DateTimeOffset Time, string Message, string StackTrace)
{
    public static CapturedException From(Exception exception) =>
        new(
            Time: DateTimeOffset.Now,
            Message: exception.Message,
            StackTrace: exception.StackTrace ?? string.Empty
        );
}

[JsonSerializable(typeof(CapturedException))]
internal sealed partial class AppSettingsJsonContext : JsonSerializerContext { }

public class AppSettings
{
    private const string ExceptionMessage = "Exception occured. Check logs in settings";
    private const int MaxExceptionCount = 50;
    public static readonly Uri RepositoryUrl =
        new("https://github.com/IRefactoredThat/MCalc");
    public static readonly Uri AuthorUrl =
        new("https://github.com/IRefactoredThat");
    private static readonly string Errors = Path.Combine(
        FileSystem.AppDataDirectory, "errors.json");

    private bool _handlingException;
    public UserMessageDisplayer UserMessageDisplayer { get; }
    public ObservableCollection<CapturedException> Exceptions { get; }
    public ICommand CopyErrorsCommand { get; }

    public AppSettings(UserMessageDisplayer userMessageDisplayer)
    {
        UserMessageDisplayer = userMessageDisplayer;
        CopyErrorsCommand = new Command(CopyErrors);
        Exceptions = [];

        if (!File.Exists(Errors))
        {
            File.WriteAllText(Errors, string.Empty);
            return;
        }
        foreach (var line in File.ReadLines(Errors).TakeLast(MaxExceptionCount))
        {
            try
            {
                var typeInfo = AppSettingsJsonContext.Default.CapturedException;
                if (JsonSerializer.Deserialize(line, typeInfo) is { } captured)
                {
                    Exceptions.Add(captured);
                }
            }
            catch (JsonException)
            {

            }
        }
        Persist();
    }

    private async void CopyErrors()
    {
        await Clipboard.SetTextAsync(await File.ReadAllTextAsync(Errors));
    }

    public void Initialize()
    {
        AndroidEnvironment.UnhandledExceptionRaiser += OnAndroidExceptionOccurred;
        Exceptions.CollectionChanged += OnLogsChanged;
    }

    private void OnLogsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (Exceptions.Count > MaxExceptionCount)
        {
            Exceptions.RemoveAt(0);
            return;
        }
        Persist();
    }

    private void Persist()
    {
        try
        {
            File.WriteAllText(Errors, string.Join(Environment.NewLine,
                Exceptions.Select(x => JsonSerializer.Serialize(x,
                    AppSettingsJsonContext.Default.CapturedException))));
        }
        catch (IOException)
        {

        }
    }

    private void Log(Exception exception)
    {
        var captured = CapturedException.From(exception);
        Exceptions.Add(captured);
        UserMessageDisplayer.ShowMessage(ExceptionMessage, () =>
            Shell.Current.GoToAsync(nameof(AppSettingsPage)));
    }

    private void OnAndroidExceptionOccurred(object? sender, RaiseThrowableEventArgs e)
    {
        if (_handlingException)
        {
            return;
        }

        e.Handled = true;
        try
        {
            _handlingException = true;
            Log(e.Exception);
        }
        finally
        {
            _handlingException = false;
        }
    }
}
