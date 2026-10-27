using CalculatorApp.Platforms.Android.Alerts;
using CalculatorApp.Platforms.Android.Extensions;
using CalculatorApp.SettingsPage.Typography;

namespace CalculatorApp.CalculatorPage.Page;

public partial class CalculatorGridPage
{
    private readonly Action _clearAction;

    public CalculatorGridPage(CalculatorPageViewModel pageViewModel,
        TypographySettings typographySettings,
        TextExtensions textExtensions,
        ClearHistoryDialog clearHistoryDialog)
    {
        InitializeComponent();

        BindingContext = pageViewModel;
        _clearAction = () => clearHistoryDialog.ShowClearHistoryConfirmation(
            () => pageViewModel.HistoryViewModel.DeleteAllCommand.Execute(null));
        InputOutput.Initialize(typographySettings, textExtensions);
    }

    private void OnHistoryButtonClicked(object? sender, EventArgs e)
    {
        BottomSheet.IsOpen = !BottomSheet.IsOpen;
    }

    private void OnClearHistoryClicked(object? sender, EventArgs e)
    {
        _clearAction.Invoke();
    }
}
