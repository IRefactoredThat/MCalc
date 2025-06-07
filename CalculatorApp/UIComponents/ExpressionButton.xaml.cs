using CalculatorApp.History;

namespace CalculatorApp.UIComponents;

public partial class ExpressionButton : Button
{
    public ExpressionInfo Info { get; }

	public ExpressionButton(ExpressionInfo info)
	{
		InitializeComponent();
        FontSize = History.ItemFontSize;

        Text = $"{info.Expression} = {info.Result}";
        Info = info;
    }

    private bool _doubleTapped;

    private async void OnSingleTapped(object sender, TappedEventArgs e)
    {
        await Task.Delay(300);
        if(_doubleTapped)
        {
            return;
        }
        _doubleTapped = false;

        await Shell.Current.GoToAsync($"//{nameof(CalculatorGrid)}",
                new Dictionary<string, object>()
                {
                    ["expression"] = Info.Expression,
                    ["result"] = Info.Result,
                    ["mode"] = Info.Mode
                });
    }

    private void OnDoubleTapped(object sender, TappedEventArgs e)
    {
        _doubleTapped = true;
        RemoveButtonFromHistory();
    }

    private void RemoveButtonFromHistory()
    {
        if (Shell.Current.CurrentPage is History history)
        {
            history.RemoveButton(this);
            history.SetIfEmptyPage();
        }
    }
}