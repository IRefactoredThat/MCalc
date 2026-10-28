using CalculatorApp.Platforms.Android.Alerts;

namespace CalculatorApp.SettingsPage.App;

public partial class ExceptionLog
{
    public ExceptionLog() => InitializeComponent();

    public static readonly BindableProperty ExceptionLogDialogProperty =
        BindableProperty.Create(
            nameof(ExceptionLogDialog),
            typeof(ExceptionLogDialog),
            typeof(ExceptionLog));

    public ExceptionLogDialog ExceptionLogDialog
    {
        get => (ExceptionLogDialog)GetValue(ExceptionLogDialogProperty);
        set => SetValue(ExceptionLogDialogProperty, value);
    }

    private void OnClicked(object? sender, EventArgs e)
    {
        ExceptionLogDialog.ShowExceptionLog();
    }
}
