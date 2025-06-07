namespace CalculatorApp.UIComponents;

public partial class CalculatorOutputField : Label
{
	private const double FontSizeFactor = 0.8;

	public CalculatorOutputField()
	{
		InitializeComponent();

		CalculatorInputField.FontSizeChanged += UpdateFontSize;
	}

	private void UpdateFontSize(double size) => FontSize = size * FontSizeFactor;
}