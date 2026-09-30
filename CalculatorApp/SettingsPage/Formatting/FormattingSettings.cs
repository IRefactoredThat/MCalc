using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using CalculatorApp.CalculatorPage.Buttons;
using CalculatorApp.CalculatorPage.Input;
using Essentials.Calculator;

namespace CalculatorApp.SettingsPage.Formatting;

public enum ENotation { Enabled, Auto, Disabled }
public enum ThousandSeparator { Comma = ',', Period = '.', Space = ' ' }
public enum DecimalSeparator { Comma = ',', Period = '.' }

public class FormattingSettings : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public DecimalSeparator DecimalSeparator
    {
        get;
        set
        {
            field = value;
            Preferences.Set(nameof(DecimalSeparator), (int)value);
            DecimalSeparatorButtonViewModel.Separator = value;
            OnPropertyChanged();
        }
    }

    public DecimalSeparatorButtonViewModel DecimalSeparatorButtonViewModel { get; }

    public ThousandSeparator ThousandSeparator
    {
        get;
        set
        {
            if (value == ThousandSeparator.Comma &&
                DecimalSeparator == DecimalSeparator.Comma)
            {
                DecimalSeparator = DecimalSeparator.Period;
            }

            if (value == ThousandSeparator.Period &&
                DecimalSeparator == DecimalSeparator.Period)
            {
                DecimalSeparator = DecimalSeparator.Comma;
            }

            field = value;
            Preferences.Set(nameof(ThousandSeparator), (int)value);
            OnPropertyChanged();
        }
    }

    public ENotation ENotation
    {
        get;
        set
        {
            field = value;
            Preferences.Set(nameof(ENotation), (int)value);
            OnPropertyChanged();
            OnPropertyChanged(nameof(ENotationExponentThreshold));
        }
    }

    public int AutoENotationDigits
    {
        get;
        set
        {
            field = value;
            Preferences.Set(nameof(AutoENotationDigits), value);
            OnPropertyChanged();
            OnPropertyChanged(nameof(ENotationExponentThreshold));
        }
    }

    public const int MinAutoENotationDigits = 1;
    public const int DefaultAutoENotationDigits = 3;
    public const int MaxAutoENotationDigits = 10;

    private const DecimalSeparator DefaultDecimalSeparator = DecimalSeparator.Period;
    private const ThousandSeparator DefaultThousandSeparator = ThousandSeparator.Comma;
    private const ENotation DefaultENotation = ENotation.Auto;

    public FormattingSettings()
    {
        DecimalSeparatorButtonViewModel = new DecimalSeparatorButtonViewModel();
        DecimalSeparator = (DecimalSeparator)Preferences.Get(nameof(DecimalSeparator), (int)DefaultDecimalSeparator);
        ThousandSeparator = (ThousandSeparator)Preferences.Get(nameof(ThousandSeparator), (int)DefaultThousandSeparator);
        ENotation = (ENotation)Preferences.Get(nameof(ENotation), (int)DefaultENotation);
        AutoENotationDigits = Preferences.Get(nameof(AutoENotationDigits), DefaultAutoENotationDigits);
    }

    public NumberFormatInfo NumberFormatInfo => new()
    {
        NumberDecimalSeparator = ((char)DecimalSeparator).ToString(),
        NumberGroupSeparator = ((char)ThousandSeparator).ToString(),
    };

    public static Enum[] RequiredNotationForDigits => [ENotation.Auto];
    public static Enum[] RequiredThousandForDecimalSeparator => [ThousandSeparator.Space];

    public int ENotationExponentThreshold => ENotation switch
    {
        ENotation.Enabled => Tokens.MaxTotalDigits,
        ENotation.Auto => AutoENotationDigits,
        ENotation.Disabled => -1,
        _ => throw new ArgumentOutOfRangeException(nameof(ENotation), ENotation, null)
    };

    public string Format(double value)
    {
        if (value == 0.0 || ENotation == ENotation.Disabled)
        {
            return value.ToString(
                $"#,0.{new string('#', Tokens.MaxTotalDigits - 1)}",
                NumberFormatInfo);
        }

        var absValue = double.Abs(value);

        var eNotationDigits = ENotation == ENotation.Auto
            ? AutoENotationDigits
            : -Tokens.MaxTotalDigits;

        var minDisplay = double.Pow(10, -eNotationDigits);
        var maxDisplay = double.Pow(10, eNotationDigits);

        var useScientific = absValue < minDisplay || absValue >= maxDisplay;

        var format = useScientific || ENotation == ENotation.Enabled
            ? $"0.{new string('#', Tokens.MaxTotalDigits - 1)}E+0"
            : $"#,0.{new string('#', Tokens.MaxTotalDigits - 1)}";

        return value.ToString(format, NumberFormatInfo);
    }
}
