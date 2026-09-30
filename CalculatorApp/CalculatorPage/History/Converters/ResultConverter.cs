using System.Globalization;
using System.Text;
using CalculatorApp.SettingsPage.Formatting;
using Essentials.Calculator;

namespace CalculatorApp.CalculatorPage.History.Converters;

public class ResultConverter : BindableObject, IValueConverter
{
    public static readonly BindableProperty FormattingSettingsProperty =
        BindableProperty.Create(
            nameof(FormattingSettings),
            typeof(FormattingSettings),
            typeof(ExpressionConverter));

    public FormattingSettings FormattingSettings
    {
        get => (FormattingSettings)GetValue(FormattingSettingsProperty);
        set => SetValue(FormattingSettingsProperty, value);
    }

    private readonly StringBuilder _buffer = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not NumberToken numberToken)
        {
            return string.Empty;
        }

        var result = FormatNumberToken(numberToken,
            FormattingSettings, _buffer).ToString();
        _buffer.Clear();
        return result;
    }

    public static StringBuilder FormatNumberToken(NumberToken number,
        FormattingSettings formattingSettings, StringBuilder builder)
    {
        var firstGroup = number.IntegerLength % 3;
        if (firstGroup == 0)
            firstGroup = 3;

        for (var offset = 0; offset < number.IntegerLength; offset++)
        {
            builder.Append(number.Value[offset]);

            var isGroupEnd =
                offset + 1 < number.IntegerLength &&
                ((offset + 1 == firstGroup) ||
                 ((offset + 1 > firstGroup) &&
                  ((offset + 1 - firstGroup) % 3 == 0)));

            if (isGroupEnd)
            {
                builder.Append((char)formattingSettings.ThousandSeparator);
            }
        }

        if (number.IntegerLength < number.Value.Length)
        {
            builder.Append(number.Value[number.IntegerLength] == 'E' ? 'E' : (char)formattingSettings.DecimalSeparator);
        }

        var startIndex = number.IntegerLength + 1;

        if (startIndex < number.Value.Length)
        {
            builder.Append(number.Value, startIndex, number.Value.Length - startIndex);
        }

        return builder;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}