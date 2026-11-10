using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace Essentials.Numerics;

public readonly partial struct Number : IParsable<Number>
{
    public static Number One => FromCurrentScale(ScaleFactor);
    public static Number Zero => FromCurrentScale(0);
    public static Number NegativeOne => FromCurrentScale(-ScaleFactor);
    public static Number HalfCircleDegrees => FromCurrentScale(180 * ScaleFactor);
    public static Number Ten => FromCurrentScale(10 * ScaleFactor);

    internal const string PiDigits =
        "31415926535897932384626433832795028841971693993751058209749445923078164062862089986280348253421170679" +
        "82148086513282306647093844609550582231725359408128481117450284102701938521105559644622948954930381964" +
        "42881097566593344612847564823378678316527120190914564856692346034861045432664821339360726024914127372" +
        "45870066063155881748815209209628292540917153643678925903600113305305488204665213841469519415116094330" +
        "57270365759591953092186117381932611793105118548074462379962749567351885752724891227938183011949129833" +
        "673362440656643086021394946395224737190702179860943702770539217176293176752384674818467669405132";
    private const int ConstantScale = MaxPrecision + 1;
    private static readonly BigInteger MaxPi =
        BigInteger.Parse(PiDigits.AsSpan(0, ConstantScale + 1));
    public static Number Pi =>
        FromScaled(MaxPi.Rescale(ConstantScale, Precision), Precision);

    private const string EDigits =
        "27182818284590452353602874713526624977572470936999595749669676277240766303535475945713821785251664274";
    private static readonly BigInteger MaxE =
        BigInteger.Parse(EDigits.AsSpan(0, ConstantScale + 1));
    public static Number E =>
        FromScaled(MaxE.Rescale(ConstantScale, Precision), Precision);

    internal const string Ln2Digits =
        "69314718055994530941723212145817656807550013436025525412068000949339362196969471560586332699641868754";

    public bool IsZero() => _scaled.IsZero;

    private bool IsInteger() =>
        _scale == 0 || _scaled % BigIntegerExtensions.Pow10(_scale) == 0;

    public override string ToString() => ToString("G", CultureInfo.CurrentCulture);

    public string ToString(string? format, IFormatProvider? provider)
    {
        provider ??= CultureInfo.CurrentCulture;

        if (_scale > Precision)
        {
            var rounded = FromScaled(_scaled.Rescale(_scale, Precision), Precision);
            return rounded.ToString(format, provider);
        }

        if (string.IsNullOrEmpty(format) || format is "G" or "g" or "R" or "r")
        {
            return FormatGeneral(provider, trim: true);
        }

        if (format is "F" or "f")
        {
            return FormatFixed(_scale, provider, useGrouping: false);
        }

        if (format is "N" or "n")
        {
            return FormatFixed(_scale, provider, useGrouping: true);
        }

        if (format is "E" or "e")
        {
            return FormatScientific(_scale, provider);
        }

        if (format.Contains('0') || format.Contains('#'))
        {
            return FormatCustom(format, provider);
        }

        return FormatGeneral(provider, trim: true);
    }

    private string FormatGeneral(IFormatProvider provider, bool trim)
    {
        var (integer, fraction, negative) = GetParts();
        var info = NumberFormatInfo.GetInstance(provider);
        var builder = new StringBuilder();

        if (negative)
        {
            builder.Append(info.NegativeSign);
        }

        builder.Append(integer);

        if (trim)
        {
            fraction = fraction.TrimEnd('0');
        }

        if (fraction.Length > 0)
        {
            builder.Append(info.NumberDecimalSeparator);
            builder.Append(fraction);
        }

        return builder.ToString();
    }

    private string FormatFixed(int digits, IFormatProvider provider, bool useGrouping)
    {
        var (integer, fraction, negative) = GetParts();
        var info = NumberFormatInfo.GetInstance(provider);
        var builder = new StringBuilder();

        if (negative)
        {
            builder.Append(info.NegativeSign);
        }

        if (useGrouping && !string.IsNullOrEmpty(info.NumberGroupSeparator))
        {
            var firstGroup = integer.Length % 3;

            if (firstGroup == 0)
            {
                firstGroup = 3;
            }

            for (var i = 0; i < integer.Length; i++)
            {
                builder.Append(integer[i]);

                var groupEnd = i + 1 < integer.Length &&
                    ((i + 1 == firstGroup) ||
                     (i + 1 > firstGroup && (i + 1 - firstGroup) % 3 == 0));

                if (groupEnd)
                {
                    builder.Append(info.NumberGroupSeparator);
                }
            }
        }
        else
        {
            builder.Append(integer);
        }

        if (digits > 0)
        {
            builder.Append(info.NumberDecimalSeparator);
            builder.Append(fraction.Length >= digits
                ? fraction[..digits]
                : fraction.PadRight(digits, '0'));
        }

        return builder.ToString();
    }

    private string FormatScientific(int digits, IFormatProvider provider)
    {
        var info = NumberFormatInfo.GetInstance(provider);
        var builder = new StringBuilder();
        var abs = BigInteger.Abs(_scaled);

        if (abs.IsZero)
        {
            builder.Append('0');

            if (digits > 0)
            {
                builder.Append(info.NumberDecimalSeparator);
                builder.Append(new string('0', digits));
            }

            builder.Append('E');
            builder.Append('+');
            builder.Append('0');
            return builder.ToString();
        }

        var text = abs.ToString(CultureInfo.InvariantCulture);
        var exponent = text.Length - _scale - 1;

        if (_scaled < 0)
        {
            builder.Append(info.NegativeSign);
        }

        builder.Append(text[0]);

        var fractionLength = Math.Min(digits, Math.Max(0, text.Length - 1));

        if (digits > 0)
        {
            builder.Append(info.NumberDecimalSeparator);
            builder.Append(fractionLength > 0 ? text.Substring(1, fractionLength) : string.Empty);
            builder.Append(new string('0', Math.Max(0, digits - fractionLength)));
        }

        builder.Append('E');
        builder.Append(exponent >= 0 ? '+' : '-');
        builder.Append(Math.Abs(exponent));
        return builder.ToString();
    }

    private enum CustomTokenKind
    {
        Literal,
        Zero,
        Digit,
        Decimal,
        Group,
        Exponent
    }

    private readonly record struct CustomToken(CustomTokenKind Kind, string Text);

    private string FormatCustom(string format, IFormatProvider provider)
    {
        var info = NumberFormatInfo.GetInstance(provider);
        var sections = format.Split(';');
        var negative = _scaled < 0;
        var zero = _scaled.IsZero;

        var section = zero && sections.Length >= 3 ? sections[2]
            : negative && sections.Length >= 2 ? sections[1]
            : sections[0];

        var includeSign = negative && !(sections.Length >= 2);

        var tokens = TokenizeCustomFormat(section);
        var firstDigit = tokens.FindIndex(
            token => token.Kind is CustomTokenKind.Zero or CustomTokenKind.Digit);

        if (firstDigit < 0)
        {
            return string.Concat(tokens.Select(token => token.Text));
        }

        var lastDigit = tokens.FindLastIndex(
            token => token.Kind is CustomTokenKind.Zero or CustomTokenKind.Digit);

        var prefix = string.Concat(tokens.Take(firstDigit).Select(token => token.Text));
        var suffix = string.Concat(tokens.Skip(lastDigit + 1).Select(token => token.Text));

        var exponentIndex = -1;

        for (var i = firstDigit; i <= lastDigit; i++)
        {
            if (tokens[i].Kind == CustomTokenKind.Exponent)
            {
                exponentIndex = i;
                break;
            }
        }

        var numberEnd = exponentIndex >= 0 ? exponentIndex : lastDigit + 1;
        var decimalIndex = -1;

        for (var i = firstDigit; i < numberEnd; i++)
        {
            if (tokens[i].Kind == CustomTokenKind.Decimal)
            {
                decimalIndex = i;
                break;
            }
        }

        var integerPattern = new StringBuilder();
        var grouping = false;
        var integerEnd = decimalIndex >= 0 ? decimalIndex : numberEnd;

        for (var i = firstDigit; i < integerEnd; i++)
        {
            if (tokens[i].Kind is CustomTokenKind.Zero or CustomTokenKind.Digit)
            {
                integerPattern.Append(tokens[i].Kind == CustomTokenKind.Zero ? '0' : '#');
            }
            else if (tokens[i].Kind == CustomTokenKind.Group)
            {
                grouping = true;
            }
        }

        var fractionPattern = new StringBuilder();

        if (decimalIndex >= 0)
        {
            for (var i = decimalIndex + 1; i < numberEnd; i++)
            {
                if (tokens[i].Kind is CustomTokenKind.Zero or CustomTokenKind.Digit)
                {
                    fractionPattern.Append(
                        tokens[i].Kind == CustomTokenKind.Zero ? '0' : '#');
                }
            }
        }

        var exponentDigits = 1;
        var exponentSignAlways = false;
        var exponentSignOptional = false;

        if (exponentIndex >= 0)
        {
            var index = exponentIndex + 1;

            if (index <= lastDigit &&
                tokens[index].Kind == CustomTokenKind.Literal &&
                tokens[index].Text is "+" or "-")
            {
                exponentSignAlways = tokens[index].Text == "+";
                exponentSignOptional = tokens[index].Text == "-";
                index++;
            }

            var placeholderCount = 0;
            var requiredDigits = 0;

            for (; index <= lastDigit; index++)
            {
                if (tokens[index].Kind is CustomTokenKind.Zero or CustomTokenKind.Digit)
                {
                    placeholderCount++;

                    if (tokens[index].Kind == CustomTokenKind.Zero)
                    {
                        requiredDigits = placeholderCount;
                    }
                }
            }

            exponentDigits = Math.Max(requiredDigits, 1);
        }

        var builder = new StringBuilder();

        if (includeSign)
        {
            builder.Append(info.NegativeSign);
        }

        builder.Append(prefix);

        if (exponentIndex >= 0)
        {
            AppendScientific(builder, integerPattern.ToString(), fractionPattern.ToString(),
                exponentDigits, exponentSignAlways, exponentSignOptional, info);
        }
        else
        {
            AppendFixed(builder, integerPattern.ToString(), fractionPattern.ToString(),
                grouping, decimalIndex >= 0, info);
        }

        builder.Append(suffix);
        return builder.ToString();
    }

    private void AppendFixed(StringBuilder builder, string integerPattern,
        string fractionPattern, bool grouping, bool hasDecimalPoint,
        NumberFormatInfo info)
    {
        var (integer, fraction) = GetRoundedParts(fractionPattern.Length);
        var minimumIntegerDigits = integerPattern.Count(character => character == '0');

        if (integer == "0" && minimumIntegerDigits == 0)
        {
            integer = string.Empty;
        }
        else if (integer.Length < minimumIntegerDigits)
        {
            integer = integer.PadLeft(minimumIntegerDigits, '0');
        }

        builder.Append(grouping ? ApplyGrouping(integer, info) : integer);

        var minimumFractionDigits = fractionPattern.LastIndexOf('0') + 1;
        var keep = fraction.Length;

        while (keep > minimumFractionDigits && fraction[keep - 1] == '0')
        {
            keep--;
        }

        if (keep > 0 || (hasDecimalPoint && fractionPattern.Length == 0))
        {
            builder.Append(info.NumberDecimalSeparator);
            builder.Append(fraction.AsSpan(0, keep));
        }
    }

    private void AppendScientific(StringBuilder builder, string integerPattern,
        string fractionPattern, int minimumExponentDigits, bool exponentSignAlways,
        bool exponentSignOptional, NumberFormatInfo info)
    {
        var integerDigits = Math.Max(1, integerPattern.Length);
        var (integer, fraction, exponent) = GetScientificParts(
            integerDigits, fractionPattern.Length);

        var minimumIntegerDigits = Math.Max(
            integerPattern.Count(character => character == '0'), 1);

        if (integer.Length < minimumIntegerDigits)
        {
            integer = integer.PadLeft(minimumIntegerDigits, '0');
        }

        builder.Append(integer);

        var minimumFractionDigits = fractionPattern.LastIndexOf('0') + 1;
        var keep = fraction.Length;

        while (keep > minimumFractionDigits && fraction[keep - 1] == '0')
        {
            keep--;
        }

        if (keep > 0)
        {
            builder.Append(info.NumberDecimalSeparator);
            builder.Append(fraction.AsSpan(0, keep));
        }

        builder.Append('E');

        if (exponentSignAlways)
        {
            builder.Append(exponent < 0 ? '-' : '+');
        }
        else if (exponentSignOptional && exponent < 0)
        {
            builder.Append('-');
        }

        builder.Append(Math.Abs(exponent)
            .ToString(CultureInfo.InvariantCulture)
            .PadLeft(minimumExponentDigits, '0'));
    }

    private (string Integer, string Fraction) GetRoundedParts(int fractionDigits)
    {
        var abs = BigInteger.Abs(_scaled);
        var rounded = abs.Rescale(_scale, fractionDigits);
        var factor = BigIntegerExtensions.Pow10(fractionDigits);
        var integer = rounded / factor;
        var fraction = (rounded % factor).ToString(
            $"D{fractionDigits}", CultureInfo.InvariantCulture);

        return (integer.ToString(CultureInfo.InvariantCulture), fraction);
    }

    private (string Integer, string Fraction, int Exponent) GetScientificParts(
        int integerDigits, int fractionDigits)
    {
        var abs = BigInteger.Abs(_scaled);

        if (abs.IsZero)
        {
            return (new string('0', integerDigits),
                new string('0', fractionDigits), 0);
        }

        var text = abs.ToString(CultureInfo.InvariantCulture);
        var exponent = text.Length - _scale - integerDigits;
        var needed = integerDigits + fractionDigits;
        string digits;

        if (text.Length > needed)
        {
            var kept = BigInteger.Parse(text[..needed], CultureInfo.InvariantCulture);

            if (text[needed] >= '5')
            {
                kept += BigInteger.One;
            }

            digits = kept.ToString(CultureInfo.InvariantCulture);

            if (digits.Length > needed)
            {
                exponent += digits.Length - needed;
                digits = digits[..needed];
            }
        }
        else
        {
            digits = text.PadRight(needed, '0');
        }

        return (digits[..integerDigits], digits[integerDigits..], exponent);
    }

    private static string ApplyGrouping(string digits, NumberFormatInfo info)
    {
        var sizes = info.NumberGroupSizes;

        if (digits.Length <= 1 ||
            string.IsNullOrEmpty(info.NumberGroupSeparator) ||
            sizes.Length == 0 || sizes[0] <= 0)
        {
            return digits;
        }

        var builder = new StringBuilder(digits.Length + digits.Length / 3);
        var index = digits.Length;
        var group = 0;

        while (index > 0)
        {
            var size = sizes[Math.Min(group, sizes.Length - 1)];

            if (size <= 0)
            {
                return digits;
            }

            var start = Math.Max(0, index - size);

            if (builder.Length > 0)
            {
                builder.Insert(0, info.NumberGroupSeparator);
            }

            builder.Insert(0, digits.Substring(start, index - start));
            index = start;
            group++;
        }

        return builder.ToString();
    }

    private static List<CustomToken> TokenizeCustomFormat(string format)
    {
        var tokens = new List<CustomToken>();
        var literal = new StringBuilder();

        void FlushLiteral()
        {
            if (literal.Length == 0)
            {
                return;
            }

            tokens.Add(new CustomToken(CustomTokenKind.Literal, literal.ToString()));
            literal.Clear();
        }

        for (var index = 0; index < format.Length; index++)
        {
            var character = format[index];

            switch (character)
            {
                case '\\' when index + 1 < format.Length:
                    literal.Append(format[++index]);
                    break;
                case '\'' or '"':
                    var quote = character;
                    index++;

                    while (index < format.Length && format[index] != quote)
                    {
                        if (format[index] == '\\' && index + 1 < format.Length)
                        {
                            index++;
                        }

                        literal.Append(format[index++]);
                    }

                    break;
                case '0':
                    FlushLiteral();
                    tokens.Add(new CustomToken(CustomTokenKind.Zero, "0"));
                    break;
                case '#':
                    FlushLiteral();
                    tokens.Add(new CustomToken(CustomTokenKind.Digit, "#"));
                    break;
                case '.':
                    FlushLiteral();
                    tokens.Add(new CustomToken(CustomTokenKind.Decimal, "."));
                    break;
                case ',':
                    FlushLiteral();
                    tokens.Add(new CustomToken(CustomTokenKind.Group, ","));
                    break;
                case 'E' or 'e':
                    FlushLiteral();
                    tokens.Add(new CustomToken(
                        CustomTokenKind.Exponent, character.ToString()));
                    break;
                default:
                    literal.Append(character);
                    break;
            }
        }

        FlushLiteral();
        return tokens;
    }

    private (string Integer, string Fraction, bool Negative) GetParts()
    {
        var abs = BigInteger.Abs(_scaled);
        var factor = BigIntegerExtensions.Pow10(_scale);
        var integer = abs / factor;
        var fraction = (abs % factor).ToString(
            $"D{_scale}", CultureInfo.InvariantCulture);

        return (integer.ToString(CultureInfo.InvariantCulture), fraction, _scaled < 0);
    }

    public static Number Parse(string s, IFormatProvider? provider = null) =>
        Parse(s, NumberStyles.Float, provider);

    private static Number Parse(string s, NumberStyles style,
        IFormatProvider? provider = null)
    {
        if (!TryParse(s, style, provider, out var result))
        {
            throw new FormatException($"The input string '{s}' was not in a correct format.");
        }

        return result;
    }

    public static bool TryParse(string? s, NumberStyles style, IFormatProvider? provider,
        out Number result)
    {
        if (s is null)
        {
            result = default;
            return false;
        }

        return TryParse(s.AsSpan(), style, provider, out result);
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Number result)
    {
        return TryParse(s, NumberStyles.Float, provider, out result);
    }

    private static bool TryParse(ReadOnlySpan<char> s, NumberStyles style,
        IFormatProvider? provider, out Number result)
    {
        result = default;
        var info = NumberFormatInfo.GetInstance(provider);
        var span = s;

        if ((style & NumberStyles.AllowLeadingWhite) != 0)
        {
            span = span.TrimStart();
        }

        if ((style & NumberStyles.AllowTrailingWhite) != 0)
        {
            span = span.TrimEnd();
        }

        if (span.IsEmpty)
        {
            return false;
        }

        var negative = false;
        var index = 0;

        if ((style & NumberStyles.AllowLeadingSign) != 0 &&
            span[0] is '+' or '-')
        {
            negative = span[0] == '-';
            index++;
        }
        else if ((style & NumberStyles.AllowParentheses) != 0 && span[0] == '(')
        {
            negative = true;
            index++;

            if (span[^1] != ')')
            {
                return false;
            }

            span = span[..^1];
        }

        var mantissa = BigInteger.Zero;
        var fractionDigits = 0;
        var significantDigits = 0;
        var significant = false;
        var inFraction = false;
        var anyDigits = false;
        var exponentIndex = -1;

        for (; index < span.Length; index++)
        {
            var character = span[index];

            if (char.IsAsciiDigit(character))
            {
                significant |= character != '0';

                if (significant)
                {
                    significantDigits++;
                }

                if (inFraction)
                {
                    fractionDigits++;
                }

                mantissa = mantissa * 10 + (character - '0');
                anyDigits = true;
            }
            else if ((style & NumberStyles.AllowDecimalPoint) != 0 &&
                     !inFraction && character.ToString() == info.NumberDecimalSeparator)
            {
                inFraction = true;
            }
            else if ((style & NumberStyles.AllowThousands) != 0 &&
                     character.ToString() == info.NumberGroupSeparator) { }
            else if ((style & NumberStyles.AllowExponent) != 0 &&
                     character is 'e' or 'E')
            {
                exponentIndex = index;
                break;
            }
            else if ((style & NumberStyles.AllowCurrencySymbol) != 0 &&
                     character.ToString() == info.CurrencySymbol) { }
            else
            {
                return false;
            }
        }

        if (!anyDigits)
        {
            return false;
        }

        var exponent = 0;

        if (exponentIndex >= 0)
        {
            var exponentSpan = span[(exponentIndex + 1)..];
            var exponentNegative = false;

            if (exponentSpan.IsEmpty)
            {
                return false;
            }

            if (exponentSpan[0] is '+' or '-')
            {
                exponentNegative = exponentSpan[0] == '-';
                exponentSpan = exponentSpan[1..];
            }

            if (exponentSpan.IsEmpty)
            {
                return false;
            }

            foreach (var character in exponentSpan)
            {
                if (!char.IsAsciiDigit(character))
                {
                    return false;
                }

                exponent = exponent * 10 + (character - '0');

                if (exponent > MaxExponent)
                {
                    return false;
                }
            }

            if (exponentNegative)
            {
                exponent = -exponent;
            }
        }

        if (significantDigits + exponent - fractionDigits > MaxWholeDigits)
        {
            return false;
        }

        var shift = Precision + exponent - fractionDigits;

        BigInteger scaled;

        if (shift >= 0)
        {
            scaled = mantissa * BigIntegerExtensions.Pow10(shift);
        }
        else
        {
            scaled = mantissa.DivideRounded(BigIntegerExtensions.Pow10(-shift));
        }

        result = FromScaled(negative ? -scaled : scaled, Precision);
        return true;
    }

}
