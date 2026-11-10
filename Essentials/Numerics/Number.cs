using System.Numerics;
using Essentials.Calculator;

namespace Essentials.Numerics;

/// <summary>
/// Represents an arbitrary range fixed-point number with a configurable number of decimal places.
/// </summary>
/// <remarks>
/// <para>
/// The value is stored as a <see cref="BigInteger"/> scaled by
/// <c>10 ^ <see cref="Precision"/></c>. Unlike <see cref="double"/> there is no
/// binary rounding error for values such as <c>0.1</c>, and the integer part can
/// grow without overflowing.
/// </para>
/// <para>
/// <see cref="Precision"/> is the number of digits after the decimal separator.
/// It can be changed at any time and applies to every operation performed after
/// the change.
/// </para>
/// </remarks>
public readonly partial struct Number : IEquatable<Number>, IComparable<Number>, IExpressionPart
{
    private readonly BigInteger _scaled;
    private readonly int _scale;

    private Number(BigInteger scaled, int scale) => (_scaled, _scale) = (scaled, scale);

    /// <summary>
    /// Creates a <see cref="Number"/> from an unscaled <paramref name="value"/>
    /// without checking the whole-digit limit.
    /// </summary>
    /// <remarks>
    /// Intended for tests and known-safe internal values only.
    /// </remarks>
    public static Number CreateUnsafe(BigInteger value) =>
        FromCurrentScale(value * ScaleFactor);

    /// <summary>
    /// Maximum number of digits allowed before the decimal separator.
    /// Values produced by operations are limited to this whole-part size.
    /// </summary>
    public const int MaxWholeDigits = 500;

    private const int FactorialBound = 250;
    private const int MaxExponent = 100_000;

    /// <summary>
    /// Minimum number of digits allowed after the decimal separator.
    /// </summary>
    public const int MinPrecision = 0;
    /// <summary>
    /// Maximum number of digits allowed after the decimal separator.
    /// </summary>
    public const int MaxPrecision = 50;
    /// <summary>
    /// Default number of digits allowed after the decimal separator.
    /// </summary>
    public const int DefaultPrecision = 5;

    /// <summary>
    /// Active number of digits allowed after the decimal separator.
    /// </summary>
    public static int Precision
    {
        get;
        set => field = Math.Clamp(value, MinPrecision, MaxPrecision);
    } = DefaultPrecision;

    private static BigInteger ScaleFactor => BigIntegerExtensions.Pow10(Precision);

    private static Number FromScaled(BigInteger scaled, int scale) => new(scaled, scale);

    private static Number FromCurrentScale(BigInteger scaled) => new(scaled, Precision);

    private BigInteger ToScaled(int scale)
    {
        if (_scale == scale)
        {
            return _scaled;
        }

        if (_scale < scale)
        {
            return _scaled * BigIntegerExtensions.Pow10(scale - _scale);
        }

        return _scaled.DivideRounded(BigIntegerExtensions.Pow10(_scale - scale));
    }

    private BigInteger ToBigInteger() => _scaled / BigIntegerExtensions.Pow10(_scale);

    private static Number FromWorking(BigInteger scaled, int workingScale) =>
        FromScaled(scaled, workingScale);

    public int CompareTo(Number other)
    {
        return ToScaled(Precision).CompareTo(other.ToScaled(Precision));
    }

    private int CompareExactTo(Number other)
    {
        if (_scale == other._scale)
        {
            return _scaled.CompareTo(other._scaled);
        }

        var scale = Math.Max(_scale, other._scale);

        return ToScaled(scale).CompareTo(other.ToScaled(scale));
    }

    public bool Equals(Number other) => CompareTo(other) == 0;

    public override bool Equals(object? obj) => obj is Number other && Equals(other);

    public override int GetHashCode() => ToScaled(Precision).GetHashCode();

    public static bool operator ==(Number left, Number right) => left.Equals(right);

    public static bool operator !=(Number left, Number right) => !left.Equals(right);

    public static bool operator <(Number left, Number right) => left.CompareTo(right) < 0;

    public static bool operator <=(Number left, Number right) => left.CompareTo(right) <= 0;

    public static bool operator >(Number left, Number right) => left.CompareTo(right) > 0;

    public static bool operator >=(Number left, Number right) => left.CompareTo(right) >= 0;
}
