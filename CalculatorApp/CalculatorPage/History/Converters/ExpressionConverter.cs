using System.Globalization;
using System.Text;
using CalculatorApp.SettingsPage.Formatting;
using Essentials.Calculator;

namespace CalculatorApp.CalculatorPage.History.Converters;

public class ExpressionConverter : BindableObject, IValueConverter
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
        if (value is not IReadOnlyList<IToken> tokens)
        {
            return string.Empty;
        }

        return tokens.Aggregate(_buffer, (current, token) =>
            token is NumberToken number ?
                ResultConverter.FormatNumberToken(number, FormattingSettings, current)
                : current.Append(token.Value), finalBuilder =>
        {
            var result = finalBuilder.ToString();
            finalBuilder.Clear();
            return result;
        });
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException();
}