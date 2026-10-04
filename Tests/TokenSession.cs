using System.Globalization;
using Calculator.ExpressionComposition;
using Calculator.ExpressionParsing;
using Calculator.ExpressionSolving;
using Essentials.ErrorType;
using Essentials.Calculator;
using Essentials.ResultType;

namespace Tests;

internal sealed class TokenSession
{
    private readonly Tokens _tokens = new();

    public DecimalSeparator DecimalSeparator { get; set; } = DecimalSeparator.Period;

    public ThousandSeparator ThousandSeparator { get; set; } = ThousandSeparator.Space;

    public AngleMode Mode { get; set; } = AngleMode.RAD;

    public NumberFormatInfo Info => new()
    {
        NumberDecimalSeparator = ((char)DecimalSeparator).ToString(),
        NumberGroupSeparator = ((char)ThousandSeparator).ToString()
    };

    public IEnumerable<string> TokenValues => _tokens.Select(token => token.Value);

    public int Count => _tokens.Count;

    public IToken this[int index] => _tokens[index];

    public int DisplayPosition => _tokens.DisplayPosition;

    public void Clear() => _tokens.Clear();

    private static readonly NumberToken[] DigitTokens =
    [
        NumberToken.Zero,
        NumberToken.One,
        NumberToken.Two,
        NumberToken.Three,
        NumberToken.Four,
        NumberToken.Five,
        NumberToken.Six,
        NumberToken.Seven,
        NumberToken.Eight,
        NumberToken.Nine
    ];

    public NumberToken Number(string value) => new(value, DecimalSeparator);

    public NumberToken DecimalSeparatorToken() => Number(((char)DecimalSeparator).ToString());

    public void TypeNumber(string digits)
    {
        foreach (var digit in digits)
        {
            Add(DigitTokens[digit - '0']);
        }
    }

    public (string Text, int Position) Add(IToken token)
    {
        var logicalPosition = _tokens.AddToken(token, DecimalSeparator);

        return Render(logicalPosition);
    }

    public (string Text, int Position) Paste(IReadOnlyList<IToken> sequence)
    {
        var logicalPosition = _tokens.AddTokens(sequence, DecimalSeparator);

        return Render(logicalPosition);
    }

    public (string Text, int Position) Remove()
    {
        var logicalPosition = _tokens.RemoveToken(DecimalSeparator);

        return Render(logicalPosition);
    }

    public void MoveCursorTo(int displayPosition)
    {
        var text = RenderedText();

        _tokens.UpdateTokenPosition(displayPosition,
            IsAfterThousandSeparator(text, displayPosition));
    }

    private bool IsAfterThousandSeparator(string text, int position) =>
        position > 0 && position <= text.Length && text[position - 1] == (char)ThousandSeparator;

    private (string Text, int Position) Render(int logicalPosition)
    {
        var (text, position) = _tokens.GetTextInfo(logicalPosition, ThousandSeparator,
            DecimalSeparator);

        _tokens.UpdateTokenPosition(position, IsAfterThousandSeparator(text, position));

        return (text, position);
    }

    public static IEnumerable<IToken> AllOperatorTokens =>
    [
        PiToken.Instance,
        EToken.Instance,
        .. SymbolOperators.Select(entry => entry.Token),
        .. NamedOperators.Select(entry => entry.Token)
    ];

    private static readonly (char Typed, IToken Token)[] ConstantOperators =
    [
        ('p', PiToken.Instance),
        ('E', EToken.Instance)
    ];

    private static readonly (char Typed, IToken Token)[] SymbolOperators =
    [
        ('+', PlusToken.Instance),
        ('-', MinusToken.Instance),
        ('*', TimesToken.Instance),
        ('/', DivToken.Instance),
        ('^', CaretToken.Instance),
        ('%', PercentToken.Instance),
        ('!', ExclamationMarkToken.Instance),
        ('(', OpenBracketToken.Instance),
        (')', ClosedBracketToken.Instance)
    ];

    private static readonly (string Typed, IToken Token)[] NamedOperators =
    [
        ("sin", SinToken.Instance),
        ("cos", CosToken.Instance),
        ("tan", TanToken.Instance),
        ("cot", CotToken.Instance),
        ("sec", SecToken.Instance),
        ("csc", CscToken.Instance),
        ("asin", AsinToken.Instance),
        ("acos", AcosToken.Instance),
        ("atan", AtanToken.Instance),
        ("acot", AcotToken.Instance),
        ("asec", AsecToken.Instance),
        ("acsc", AcscToken.Instance),
        ("sinh", SinhToken.Instance),
        ("cosh", CoshToken.Instance),
        ("tanh", TanhToken.Instance),
        ("coth", CothToken.Instance),
        ("sech", SechToken.Instance),
        ("csch", CschToken.Instance),
        ("asinh", AsinhToken.Instance),
        ("acosh", AcoshToken.Instance),
        ("atanh", AtanhToken.Instance),
        ("acoth", AcothToken.Instance),
        ("asech", AsechToken.Instance),
        ("acsch", AcschToken.Instance),
        ("sqrt", SqrtToken.Instance),
        ("cbrt", CbrtToken.Instance),
        ("log", LogToken.Instance),
        ("ln", LnToken.Instance)
    ];

    private static readonly Dictionary<char, IToken> SymbolTokens =
        ConstantOperators.Concat(SymbolOperators).ToDictionary(entry => entry.Typed,
            entry => entry.Token);

    private static readonly Dictionary<string, IToken> NameTokens =
        NamedOperators.ToDictionary(entry => entry.Typed, entry => entry.Token);

    private static readonly string[] OperatorNames =
    [
        .. NamedOperators.Select(entry => entry.Typed)
            .OrderByDescending(name => name.Length)
    ];

    public void Type(string source)
    {
        var index = 0;

        while (index < source.Length)
        {
            var name = OperatorNames.FirstOrDefault(candidate =>
                source.AsSpan(index).StartsWith(candidate, StringComparison.Ordinal));

            if (name is not null)
            {
                Add(NameTokens[name]);
                index += name.Length;
                continue;
            }

            var character = source[index];

            if (char.IsAsciiDigit(character))
            {
                TypeNumber(character.ToString());
            }
            else if (character is '.' or ',')
            {
                Add(Number(character.ToString()));
            }
            else
            {
                Add(SymbolTokens[character]);
            }

            index++;
        }
    }

    public Result<Expression> Parse() =>
        ExpressionParsingExtensions.Parse(_tokens, Info, Mode);

    public string RenderedText() => _tokens.GetTextInfo(_tokens.DisplayPosition, ThousandSeparator,
        DecimalSeparator).Text;

    public Result<double> Evaluate() =>
        Parse().Map(ExpressionEvaluatingExtensions.EvaluateAndRound);

    public double Value()
    {
        double? value = null;
        string? failure = null;

        Evaluate().Switch(result => value = result, error => failure = error.GetType().Name);

        Assert.That(failure, Is.Null, "Expected a value");

        return value!.Value;
    }

    public Error Error()
    {
        Error? error = null;
        var succeeded = false;

        Evaluate().Switch(_ => succeeded = true, failure => error = failure);

        Assert.That(succeeded, Is.False, "Expected an error");

        return error!;
    }
}
