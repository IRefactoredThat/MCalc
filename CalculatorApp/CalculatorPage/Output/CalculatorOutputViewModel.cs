using System.ComponentModel;
using CalculatorApp.SettingsPage.Formatting;
using Essentials.ErrorType;
using Essentials.ResultType;

namespace CalculatorApp.CalculatorPage.Output;

public class CalculatorOutputViewModel : INotifyPropertyChanged
{
    private readonly FormattingSettings _formattingSettings;
    private readonly Calculation _calculation;

    public CalculatorOutputViewModel(FormattingSettings formattingSettings,
        Calculation calculation)
    {
        _formattingSettings = formattingSettings;
        _calculation = calculation;

        Output = calculation.Result?
            .ValueOrDefault(_formattingSettings.Format, string.Empty) ?? string.Empty;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Output
    {
        get;
        set
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Output)));
        }
    }

    public void Update(Result<double> newResult)
    {
        _calculation.Result = newResult;
        Output = newResult.ValueOrDefault(_formattingSettings.Format, string.Empty);
    }

    public void ConsumeResult(Action<string> setResult, Action<Error> onError)
    {
        if (_calculation.Result is null)
        {
            onError(new EmptyExpression());
            return;
        }
        _calculation.Result?.Switch(_ =>
        {
            setResult(Output);
            _calculation.Result = null;
        }, onError);
    }
}
