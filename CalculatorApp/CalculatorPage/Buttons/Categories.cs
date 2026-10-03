using System.ComponentModel;
using CalculatorApp.SettingsPage.Formatting;
using CalculatorApp.SettingsPage.Layout;
using Essentials.Calculator;

namespace CalculatorApp.CalculatorPage.Buttons;

public interface IBaseButtonViewModel
{
    LayoutItemCategory Category { get; }
    string Text { get; }
}

public interface ICalculatorButtonViewModel : IBaseButtonViewModel
{
    public IToken Token { get; }
}

public class AngleModeButtonViewModel : IBaseButtonViewModel
{
    public string Text => nameof(AngleMode.DEG);
    public LayoutItemCategory Category => LayoutItemCategory.Special;
}

public class InverseButtonViewModel : IBaseButtonViewModel
{
    public string Text => "1/2";
    public LayoutItemCategory Category => LayoutItemCategory.Special;
}

public class EqualButtonViewModel : IBaseButtonViewModel
{
    public string Text => "=";
    public LayoutItemCategory Category => LayoutItemCategory.Special;
}

public class BackspaceButtonViewModel : IBaseButtonViewModel
{
    public string Text => "⌫";
    public LayoutItemCategory Category => LayoutItemCategory.Special;
}

public class NumberButtonViewModel(NumberToken token) : ICalculatorButtonViewModel
{
    public string Text => token.Value;
    public LayoutItemCategory Category => LayoutItemCategory.Numbers;
    public IToken Token => token;
}

public class PrimaryButtonViewModel(OperatorToken token) : ICalculatorButtonViewModel
{
    public string Text => token.Value;
    public LayoutItemCategory Category => LayoutItemCategory.Primary;
    public IToken Token => token;
}

public class SecondaryButtonViewModel(OperatorToken token) : ICalculatorButtonViewModel
{
    public string Text => token.Value;
    public LayoutItemCategory Category => LayoutItemCategory.Secondary;
    public IToken Token => token;
}
public class AlternateButtonViewModel(OperatorToken token,
    OperatorToken alternateToken, LayoutItemCategory category)
    : ICalculatorButtonViewModel, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public string Text => Token.Value;
    public LayoutItemCategory Category => category;

    public IToken Token
    {
        get;
        private set
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Token)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
        }
    } = token;

    public void ToAlternate() => Token =
        ReferenceEquals(Token, alternateToken) ? token : alternateToken;
}

public class DecimalSeparatorButtonViewModel :
    ICalculatorButtonViewModel, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public DecimalSeparator Separator
    {
        get;
        set
        {
            field = value;
            Token = new NumberToken(((char)Separator).ToString(), value);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
        }
    }

    public string Text => Token.Value;
    public LayoutItemCategory Category => LayoutItemCategory.Numbers;

    public IToken Token
    {
        get;
        private set
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Token)));
        }
    } = null!;
}
