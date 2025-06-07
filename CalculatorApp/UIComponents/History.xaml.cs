using CalculatorApp.History;

namespace CalculatorApp.UIComponents;

public partial class History : ContentPage
{
    private const double ItemFontSizeFactor = 1.4;
    public static double ItemFontSize { get; private set; }

	public History()
	{
		InitializeComponent();
        CalculatorInputField.FontSizeChanged += UpdateItemFontSize;
    }

    private void UpdateItemFontSize(double fontSize) => 
        ItemFontSize = fontSize * ItemFontSizeFactor;

	public void SetIfEmptyPage()
	{
        if(SavedExpressions.AreNotEmpty())
        {
            return;
        }

        var label = new Label()
        {
            Text = "Calculate some expressions and they will be remembered here.",
            TextColor = Colors.White,
        };
        expressionStack.Add(label);
    }

	protected override void OnAppearing()
	{
		base.OnAppearing();

		var savedExpressionInfos = SavedExpressions.Get();
        SetIfEmptyPage();

        foreach (var info in savedExpressionInfos)
        {
            var button = new ExpressionButton(info);
            expressionStack.Add(button);
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
		expressionStack.Clear();
    }

    public void RemoveButton(ExpressionButton button)
	{
		expressionStack.Remove(button);
        SavedExpressions.TryRemove(button.Info);
	}
}