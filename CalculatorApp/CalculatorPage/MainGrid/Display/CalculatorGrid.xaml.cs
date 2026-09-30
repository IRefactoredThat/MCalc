using CalculatorApp.CalculatorPage.Page;

namespace CalculatorApp.CalculatorPage.MainGrid.Display;

public partial class CalculatorGrid
{
    public CalculatorGrid() => InitializeComponent();

    public static readonly BindableProperty InputOutputProperty =
        BindableProperty.Create(
            nameof(InputOutput),
            typeof(IInputOutput),
            typeof(CalculatorGrid));

    public IInputOutput InputOutput
    {
        get => (IInputOutput)GetValue(InputOutputProperty);
        set => SetValue(InputOutputProperty, value);
    }
}