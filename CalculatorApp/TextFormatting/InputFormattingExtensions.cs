using static Calculator.ExpressionComposition.ExpressionPartsParsingExtensions;

namespace CalculatorApp.TextFormatting;

internal record TextChangeInfo(string NewText, bool Succeeded);

internal static class InputFormattingExtensions
{
    public const int MaxDigitCount = 15;
    public const int MaxFractionalDigitCount = 10;
    public const int MaxDigitCountWithCommas = 19;

    public static TextChangeInfo RemoveBeforeCursor(this string text, int position)
    {
        if(string.IsNullOrEmpty(text))
        {
            return new(text, false);
        }

        var newText = text;
        var operationSuceeded = true;

        foreach (var symbolInfo in SymbolsExtractionRegex.EnumerateMatches(newText))
        {
            if (position > symbolInfo.Index &&
                position <= symbolInfo.Index + symbolInfo.Length)
            {
                var part = newText.AsSpan()
                    .Slice(symbolInfo.Index, symbolInfo.Length);
                var hasComma = part.Contains(',');
                if ((hasComma && symbolInfo.Length > MaxDigitCountWithCommas)
                    || (!hasComma && symbolInfo.Length > MaxDigitCount))
                {
                    operationSuceeded = false;
                    break;
                }

                newText = text.Remove(position, symbolInfo);
                break;
            }
        }

        if (newText == text)
        {
            newText = newText.Remove(position - 1, 1);
        }

        var span = newText.AsSpan();

        foreach (var symbolInfo in SymbolsExtractionRegex.EnumerateMatches(newText))
        {
            var part = span.Slice(symbolInfo.Index, symbolInfo.Length);
            if (part.Contains(',') && symbolInfo.Length > MaxDigitCountWithCommas)
            {
                var noCommaNumber = part
                    .ToString()
                    .Replace(",", "");

                return new(newText.Remove(symbolInfo.Index, symbolInfo.Length)
                    .Insert(symbolInfo.Index, noCommaNumber), false);
            }
            else if(part.TryParseInput(out var formatted))
            {
                return new(newText.Remove(symbolInfo.Index, symbolInfo.Length)
                    .Insert(symbolInfo.Index, formatted), true);
            }
        }

        return new(newText, operationSuceeded);
    }

    public static string InsertBeforeCursor(this string text, int position, 
        string toInsert)
    {
        var newText = text.Insert(position, toInsert);
        
        if (position > 0 && !char.IsDigit(toInsert[0]) && char.IsDigit(text[position - 1]))
        {
            var target = position - 1;
            foreach (var symbolInfo in SymbolsExtractionRegex.EnumerateMatches(newText))
            {
                if (target > symbolInfo.Index &&
                    target < symbolInfo.Index + symbolInfo.Length)
                {
                    _ = newText.AsSpan()
                        .Slice(symbolInfo.Index, symbolInfo.Length)
                        .TryParseInput(out var formatted);
                    newText = newText.Remove(symbolInfo.Index, symbolInfo.Length)
                        .Insert(symbolInfo.Index, formatted);
                    break;
                }
            }
        }

        var span = newText.AsSpan();
        
        foreach (var symbolInfo in SymbolsExtractionRegex.EnumerateMatches(newText))
        {
            if(symbolInfo.Length > MaxDigitCountWithCommas)
            {
                return text;
            }
            if(position > symbolInfo.Index && 
                position < symbolInfo.Index + symbolInfo.Length)
            {
                var part = span.Slice(symbolInfo.Index, symbolInfo.Length);

                if (!part.TryParseInput(out var formatted))
                {
                    return text;
                }

                newText = newText.Remove(symbolInfo.Index, symbolInfo.Length)
                                 .Insert(symbolInfo.Index, formatted);
                break;
            }
        }

        var currentPosition = 0;
        span = newText.AsSpan();

        foreach (var symbolInfo in SymbolsExtractionRegex.EnumerateMatches(newText))
        {
            var part = span.Slice(symbolInfo.Index, symbolInfo.Length);
            var isNumber = double.TryParse(part, out _);

            if(isNumber && part.Contains('.')
                && newText.Length > symbolInfo.Index + symbolInfo.Length &&
                newText[symbolInfo.Index + symbolInfo.Length] == '.')
            {
                return text;
            }

            if(isNumber && (symbolInfo.Index > 0 && newText.Length > 0
                && newText[symbolInfo.Index - 1] == ','))
            {
                return newText.Remove(symbolInfo.Index - 1, 1);
            }

            if(isNumber && (newText.Length > symbolInfo.Index + symbolInfo.Length) &&
                newText[symbolInfo.Index + symbolInfo.Length] == ',')
            {
                return newText.Remove(symbolInfo.Index + symbolInfo.Length, 1);
            }

            if (currentPosition != symbolInfo.Index)
            {
                return text;
            }
            currentPosition += symbolInfo.Length;
        }

        return currentPosition >= newText.Length - 1 ? newText : text;
    }
}