using System.ComponentModel;
using CalculatorApp.SettingsPage.Formatting;
using Essentials.Calculator;

namespace CalculatorApp.CalculatorPage.Input;

public class CalculatorInputViewModel : INotifyPropertyChanged
{
    private readonly Tokens _tokens;
    private readonly FormattingSettings _formattingSettings;

    public CalculatorInputViewModel(Tokens tokens, FormattingSettings formattingSettings)
    {
        _tokens = tokens;
        _formattingSettings = formattingSettings;

        Position = tokens.DisplayPosition;
        Text = tokens.GetTextInfo(Position, formattingSettings.ThousandSeparator,
                formattingSettings.DecimalSeparator).Text;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public int Position
    {
        get;
        set
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Position)));
        }
    }

    public string Text
    {
        get;
        set
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
        }
    }

    public void SetResult(NumberToken resultToken)
    {
        _tokens.Clear();
        AddTokenAtCursor(resultToken);
    }

    public void Clear()
    {
        _tokens.Clear();
        Text = string.Empty;
        Position = 0;
    }

    public void AddTokenAtCursor(IToken token)
    {
        var logicalPosition = _tokens.AddToken(token, _formattingSettings.DecimalSeparator);
        UpdateInput(logicalPosition);
    }

    public void RemoveTokenAtCursor()
    {
        var logicalPosition = _tokens.RemoveToken(_formattingSettings.DecimalSeparator);
        UpdateInput(logicalPosition);
    }

    public void AddTokensAtCursor(IReadOnlyList<IToken> sequence)
    {
        var logicalPosition = _tokens.AddTokens(sequence, _formattingSettings.DecimalSeparator);
        UpdateInput(logicalPosition);
    }

    private void UpdateInput(int logicalPosition)
    {
        var info = _tokens.GetTextInfo(logicalPosition,
            _formattingSettings.ThousandSeparator, _formattingSettings.DecimalSeparator);
        Text = info.Text;
        Position = info.Position;
    }

    public Func<int, int> NormalizePosition => NormalizeCursorPosition;

    private int NormalizeCursorPosition(int cursorPosition)
    {
        var afterThousandSeparator = cursorPosition > 0 &&
                                     Text[cursorPosition - 1] == (char)_formattingSettings.ThousandSeparator;
        _tokens.UpdateTokenPosition(cursorPosition, afterThousandSeparator);
        return _tokens.DisplayPosition;
    }
}
