using System.Buffers;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace CalculatorApp.TextFormatting;

internal static class RemovedTextPartFormattingExtensions
{
    private static readonly SearchValues<char> _specialRemovedCharacthers =
            SearchValues.Create("0123456789,.E+-");

    public static string Remove(this string text, int position, ValueMatch symbolInfo)
    {
        var targetCharacter = text[position - 1];

        if (_specialRemovedCharacthers.Contains(targetCharacter))
        {
            return text.RemoveCharacter(position, symbolInfo);
        }
        return text.Remove(symbolInfo.Index, symbolInfo.Length);
    }

    private static string RemoveCharacter(this string text,
        int position, ValueMatch symbolInfo)
    {
        if(symbolInfo.Length == 1)
        {
            return text.Remove(symbolInfo.Index, 1);
        }

        var updatedInfo = (symbolInfo.Index, symbolInfo.Length);
        if (text[position - 1] == ',')
        {
            text = text.Remove(position - 2, 2);
            updatedInfo.Length -= 2;
        }
        else
        {
            text = text.Remove(position - 1, 1);
            updatedInfo.Length -= 1;
        }

        if(!text.All(_specialRemovedCharacthers.Contains))
        {
            return text;
        }

        var span = text.AsSpan()
                .Slice(updatedInfo.Index, updatedInfo.Length);
        if(!double.TryParse(span, out var parsed))
        {
            return text.Remove(updatedInfo.Index, updatedInfo.Length)
                       .Insert(updatedInfo.Index, span.ToString());
        }

        var (_, fractionalDigits, _, containsE) = span.GetValueInfo();

        string format = containsE ? $"0.{new string('#', fractionalDigits)}E+0"
                                      : $"#,##0.{new string('0', fractionalDigits)}";

        var formatted = parsed.ToString(format);

        return text.Remove(updatedInfo.Index, updatedInfo.Length)
            .Insert(updatedInfo.Index, formatted);
    }
}