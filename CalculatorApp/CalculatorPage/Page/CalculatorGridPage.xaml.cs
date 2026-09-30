using CalculatorApp.Platforms.Android.Extensions;
using CalculatorApp.SettingsPage.Typography;

namespace CalculatorApp.CalculatorPage.Page;

public partial class CalculatorGridPage
{
    public CalculatorGridPage(CalculatorPageViewModel pageViewModel,
        TypographySettings typographySettings,
        TextExtensions textExtensions)
    {
        InitializeComponent();

        BindingContext = pageViewModel;
        InputOutput.Initialize(typographySettings, textExtensions);
    }

    private void OnHistoryButtonClicked(object? sender, EventArgs e)
    {
        BottomSheet.IsOpen = !BottomSheet.IsOpen;
    }
}