using System.Text.Json.Serialization;

namespace Essentials.Calculator;

public interface IToken
{
    string Value { get; }
}

public abstract class OperatorToken : IToken
{
    public abstract string Value { get; }
}

public sealed class NumberToken : IToken, IEquatable<NumberToken>
{
    [JsonIgnore]
    public int IntegerLength { get; }
    public string Value { get; }

    public NumberToken(string value, DecimalSeparator decimalSeparator)
    {
        Value = value;
        IntegerLength = value.IndexOfAny((char)decimalSeparator, 'E');
        if (IntegerLength < 0)
        {
            IntegerLength = value.Length;
        }
    }

    public static readonly NumberToken Zero = new("0", DecimalSeparator.Period);
    public static readonly NumberToken One = new("1", DecimalSeparator.Period);
    public static readonly NumberToken Two = new("2", DecimalSeparator.Period);
    public static readonly NumberToken Three = new("3", DecimalSeparator.Period);
    public static readonly NumberToken Four = new("4", DecimalSeparator.Period);
    public static readonly NumberToken Five = new("5", DecimalSeparator.Period);
    public static readonly NumberToken Six = new("6", DecimalSeparator.Period);
    public static readonly NumberToken Seven = new("7", DecimalSeparator.Period);
    public static readonly NumberToken Eight = new("8", DecimalSeparator.Period);
    public static readonly NumberToken Nine = new("9", DecimalSeparator.Period);

    public bool Equals(NumberToken? other) => Value == other?.Value;

    public override bool Equals(object? obj) => Equals(obj as NumberToken);

    public override int GetHashCode() => Value.GetHashCode();
}

public sealed class PiToken : OperatorToken
{
    private PiToken() { }
    public override string Value => "π";
    public static readonly PiToken Instance = new();
}

public sealed class EToken : OperatorToken
{
    private EToken() { }
    public override string Value => "e";
    public static readonly EToken Instance = new();
}

public sealed class PlusToken : OperatorToken
{
    private PlusToken() { }
    public override string Value => "+";
    public static readonly PlusToken Instance = new();
}
public sealed class MinusToken : OperatorToken
{
    private MinusToken() { }
    public override string Value => "-";
    public static readonly MinusToken Instance = new();
}

public sealed class TimesToken : OperatorToken
{
    private TimesToken() { }
    public override string Value => "×";
    public static readonly TimesToken Instance = new();
}

public sealed class DivToken : OperatorToken
{
    private DivToken() { }
    public override string Value => "÷";
    public static readonly DivToken Instance = new();
}

public sealed class CaretToken : OperatorToken
{
    private CaretToken() { }
    public override string Value => "^";
    public static readonly CaretToken Instance = new();
}

public sealed class PercentToken : OperatorToken
{
    private PercentToken() { }
    public override string Value => "%";
    public static readonly PercentToken Instance = new();
}

public sealed class ExclamationMarkToken : OperatorToken
{
    private ExclamationMarkToken() { }
    public override string Value => "!";
    public static readonly ExclamationMarkToken Instance = new();
}

public sealed class OpenBracketToken : OperatorToken
{
    private OpenBracketToken() { }
    public override string Value => "(";
    public static readonly OpenBracketToken Instance = new();
}

public sealed class ClosedBracketToken : OperatorToken
{
    private ClosedBracketToken() { }
    public override string Value => ")";
    public static readonly ClosedBracketToken Instance = new();
}

public sealed class SinToken : OperatorToken
{
    private SinToken() { }
    public override string Value => "sin";
    public static readonly SinToken Instance = new();
}

public sealed class CosToken : OperatorToken
{
    private CosToken() { }
    public override string Value => "cos";
    public static readonly CosToken Instance = new();
}

public sealed class TanToken : OperatorToken
{
    private TanToken() { }
    public override string Value => "tan";
    public static readonly TanToken Instance = new();
}

public sealed class CotToken : OperatorToken
{
    private CotToken() { }
    public override string Value => "cot";
    public static readonly CotToken Instance = new();
}

public sealed class SecToken : OperatorToken
{
    private SecToken() { }
    public override string Value => "sec";
    public static readonly SecToken Instance = new();
}

public sealed class CscToken : OperatorToken
{
    private CscToken() { }
    public override string Value => "csc";
    public static readonly CscToken Instance = new();
}

public sealed class AsinToken : OperatorToken
{
    private AsinToken() { }
    public override string Value => "asin";
    public static readonly AsinToken Instance = new();
}

public sealed class AcosToken : OperatorToken
{
    private AcosToken() { }
    public override string Value => "acos";
    public static readonly AcosToken Instance = new();
}

public sealed class AtanToken : OperatorToken
{
    private AtanToken() { }
    public override string Value => "atan";
    public static readonly AtanToken Instance = new();
}

public sealed class AcotToken : OperatorToken
{
    private AcotToken() { }
    public override string Value => "acot";
    public static readonly AcotToken Instance = new();
}

public sealed class AsecToken : OperatorToken
{
    private AsecToken() { }
    public override string Value => "asec";
    public static readonly AsecToken Instance = new();
}

public sealed class AcscToken : OperatorToken
{
    private AcscToken() { }
    public override string Value => "acsc";
    public static readonly AcscToken Instance = new();
}

public sealed class SinhToken : OperatorToken
{
    private SinhToken() { }
    public override string Value => "sinh";
    public static readonly SinhToken Instance = new();
}

public sealed class CoshToken : OperatorToken
{
    private CoshToken() { }
    public override string Value => "cosh";
    public static readonly CoshToken Instance = new();
}

public sealed class TanhToken : OperatorToken
{
    private TanhToken() { }
    public override string Value => "tanh";
    public static readonly TanhToken Instance = new();
}

public sealed class CothToken : OperatorToken
{
    private CothToken() { }
    public override string Value => "coth";
    public static readonly CothToken Instance = new();
}

public sealed class SechToken : OperatorToken
{
    private SechToken() { }
    public override string Value => "sech";
    public static readonly SechToken Instance = new();
}

public sealed class CschToken : OperatorToken
{
    private CschToken() { }
    public override string Value => "csch";
    public static readonly CschToken Instance = new();
}

public sealed class AsinhToken : OperatorToken
{
    private AsinhToken() { }
    public override string Value => "asinh";
    public static readonly AsinhToken Instance = new();
}

public sealed class AcoshToken : OperatorToken
{
    private AcoshToken() { }
    public override string Value => "acosh";
    public static readonly AcoshToken Instance = new();
}

public sealed class AtanhToken : OperatorToken
{
    private AtanhToken() { }
    public override string Value => "atanh";
    public static readonly AtanhToken Instance = new();
}

public sealed class AcothToken : OperatorToken
{
    private AcothToken() { }
    public override string Value => "acoth";
    public static readonly AcothToken Instance = new();
}

public sealed class AsechToken : OperatorToken
{
    private AsechToken() { }
    public override string Value => "asech";
    public static readonly AsechToken Instance = new();
}

public sealed class AcschToken : OperatorToken
{
    private AcschToken() { }
    public override string Value => "acsch";
    public static readonly AcschToken Instance = new();
}

public sealed class SqrtToken : OperatorToken
{
    private SqrtToken() { }
    public override string Value => "√";
    public static readonly SqrtToken Instance = new();
}

public sealed class CbrtToken : OperatorToken
{
    private CbrtToken() { }
    public override string Value => "∛";
    public static readonly CbrtToken Instance = new();
}

public sealed class LogToken : OperatorToken
{
    private LogToken() { }
    public override string Value => "log";
    public static readonly LogToken Instance = new();
}

public sealed class LnToken : OperatorToken
{
    private LnToken() { }
    public override string Value => "ln";
    public static readonly LnToken Instance = new();
}
