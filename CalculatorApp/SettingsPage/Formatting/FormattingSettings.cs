using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using CalculatorApp.CalculatorPage.Buttons;
using CalculatorApp.SettingsPage.Backup;
using Essentials.Calculator;
using Essentials.Numerics;

namespace CalculatorApp.SettingsPage.Formatting;

public enum ENotation { Enabled, Auto, Disabled }

public class FormattingSettings(IPreferences preferences) :
    INotifyPropertyChanged, IReload
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
            preferences.Set(nameof(DecimalSeparator), (int)value);
            DecimalSeparatorButtonViewModel.Separator = value;
            OnPropertyChanged();
        }
    }

    public DecimalSeparatorButtonViewModel DecimalSeparatorButtonViewModel { get; } = new();

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
            preferences.Set(nameof(ThousandSeparator), (int)value);
            OnPropertyChanged();
        }
    }

    public ENotation ENotation
    {
        get;
        set
        {
            field = value;
            preferences.Set(nameof(ENotation), (int)value);
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
            preferences.Set(nameof(AutoENotationDigits), value);
            OnPropertyChanged();
            OnPropertyChanged(nameof(ENotationExponentThreshold));
        }
    }

    public const int MinAutoENotationDigits = 1;
    public const int DefaultAutoENotationDigits = 6;
    public const int MaxAutoENotationDigits = 50;

    public int Precision
    {
        get;
        set
        {
            Number.Precision = value;
            field = Number.Precision;
            OnPropertyChanged();
        }
    }

    private const DecimalSeparator DefaultDecimalSeparator = DecimalSeparator.Period;
    private const ThousandSeparator DefaultThousandSeparator = ThousandSeparator.Comma;
    private const ENotation DefaultENotation = ENotation.Auto;

    public void Reload()
    {
        DecimalSeparator = (DecimalSeparator)preferences.Get(nameof(DecimalSeparator), (int)DefaultDecimalSeparator);
        ThousandSeparator = (ThousandSeparator)preferences.Get(nameof(ThousandSeparator), (int)DefaultThousandSeparator);
        ENotation = (ENotation)preferences.Get(nameof(ENotation), (int)DefaultENotation);
        AutoENotationDigits = preferences.Get(nameof(AutoENotationDigits), DefaultAutoENotationDigits);
        Precision = preferences.Get(nameof(Precision), Number.DefaultPrecision);
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
        ENotation.Enabled => Number.MaxWholeDigits + Number.Precision,
        ENotation.Auto => AutoENotationDigits,
        ENotation.Disabled => -1,
        _ => throw new ArgumentOutOfRangeException(nameof(ENotation), ENotation, null)
    };

    public const int WholePartLength = 6;

    public string Format(Number value)
    {
        var fractionFormat = Number.Precision > 0
            ? $".{new string('#', Number.Precision)}"
            : string.Empty;
        var decimalFormat = $"#,0{fractionFormat}";
        var scientificFormat = $"0{fractionFormat}E+0";

        if (value.IsZero() || ENotation == ENotation.Disabled)
        {
            return value.ToString(decimalFormat, NumberFormatInfo);
        }

        var absValue = value.Abs();

        var eNotationDigits = (ENotation == ENotation.Auto ?
            AutoENotationDigits : 0) + Number.Precision;

        var minDisplay = Number.Ten.PowInteger(-eNotationDigits);
        var maxDisplay = Number.Ten.PowInteger(eNotationDigits);

        var useScientific = absValue < minDisplay || absValue >= maxDisplay;

        var format = useScientific || ENotation == ENotation.Enabled ? scientificFormat : decimalFormat;

        return value.ToString(format, NumberFormatInfo);
    }
}
