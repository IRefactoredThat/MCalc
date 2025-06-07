using static CalculatorApp.TextFormatting.InputFormattingExtensions;

namespace CalculatorApp.TextFormatting;

internal static class AddedTextPartFormattingExtensions
{
    public static (int IntegerDigits, int FractionalDigits, 
        bool ContainsDot, bool ContainsE) GetValueInfo(this ReadOnlySpan<char> value)
    {
        var dotIndex = value.IndexOf('.');
        var commaCount = value.Count(',');

        var eIndex = value.IndexOf('E');
        var length = eIndex >= 0 ? eIndex : value.Length;

        if (dotIndex == -1)
        {
            return (length - commaCount, 0, false, eIndex >= 0);
        }

        return (dotIndex - commaCount, length - dotIndex - 1, true, eIndex >= 0);
    }

    public static bool TryParseInput(this ReadOnlySpan<char> input, out string formatted)
    {
        formatted = input.ToString();

        if (double.TryParse(input, out var result))
        {
            var (integerDigits, fractionalDigits, containsDot, containsE) = 
                input.GetValueInfo();

            if (fractionalDigits > MaxFractionalDigitCount || 
                integerDigits + fractionalDigits > MaxDigitCount)
            {
                return false;
            }

            string format = containsE ? $"0.{new string('#', fractionalDigits)}E+0"
                                      : $"#,##0.{new string('0', fractionalDigits)}";

            formatted = result.ToString(format);

            if (!formatted.Contains('.') && containsDot)
            {
                formatted += '.';
            }
        }

        return true;
    }
}