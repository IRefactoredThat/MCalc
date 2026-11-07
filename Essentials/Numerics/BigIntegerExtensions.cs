using System.Numerics;

namespace Essentials.Numerics;

internal static class BigIntegerExtensions
{
    private const int CachedPowerCount =
        Number.MaxWholeDigits + Number.MaxPrecision + 1;
    private const int CachedIntegerCount = 8_192;
    private const double Log2OfTen = 3.32192809488736234787;

    private static readonly Dictionary<int, BigInteger> PowersOfTen = new();
    private static readonly Dictionary<int, BigInteger> SmallIntegers = new();

    public static BigInteger Pow10(int exponent)
    {
        if (PowersOfTen.TryGetValue(exponent, out var power))
        {
            return power;
        }

        var cachedValue = BigInteger.Pow(10, exponent);
        if (exponent < CachedPowerCount)
        {
            PowersOfTen.Add(exponent, cachedValue);
        }
        return cachedValue;
    }

    /// <summary>
    /// Returns the number of bits needed by any value with
    /// <see cref="Number.MaxWholeDigits"/> whole digits at <paramref name="scale"/>.
    /// </summary>
    public static long MaxBitsForScale(int scale) =>
        (long)Math.Ceiling((Number.MaxWholeDigits + scale) * Log2OfTen);

    /// <summary>
    /// Approximates whether <paramref name="scaled"/> would have more than
    /// <see cref="Number.MaxWholeDigits"/> whole digits at <paramref name="scale"/>.
    /// </summary>
    /// <remarks>
    /// The bit length is only an approximation, so the exact power of ten is
    /// compared when the value is close to the boundary.
    /// </remarks>
    public static bool ExceedsWholeDigitLimit(this BigInteger scaled, int scale)
    {
        if (scaled.IsZero)
        {
            return false;
        }

        var absolute = BigInteger.Abs(scaled);
        var bits = absolute.GetBitLength();
        var maxBits = MaxBitsForScale(scale);

        if (bits < maxBits)
        {
            return false;
        }

        if (bits > maxBits)
        {
            return true;
        }

        return absolute >= Pow10(Number.MaxWholeDigits + scale);
    }

    private static BigInteger Pi(int scale) =>
        BigInteger.Parse(Number.PiDigits.AsSpan(0, scale + 2))
            .Rescale(scale + 1, scale);

    private static BigInteger Ln2(int scale) =>
        BigInteger.Parse(Number.Ln2Digits.AsSpan(0, scale + 1))
            .Rescale(scale + 1, scale);

    private static BigInteger SmallInteger(int value)
    {
        if (SmallIntegers.TryGetValue(value, out var result))
        {
            return result;
        }

        var cachedValue = new BigInteger(value);
        if (value < CachedIntegerCount)
        {
            SmallIntegers.Add(value, cachedValue);
        }

        return cachedValue;
    }

    public static BigInteger Exp(this BigInteger scaled, int scale)
    {
        var scaleFactor = Pow10(scale);

        if (scaled.IsZero)
        {
            return scaleFactor;
        }

        if (scaled < 0)
        {
            BigInteger positive;

            try
            {
                positive = (-scaled).Exp(scale);
            }
            catch (OverflowException)
            {
                return BigInteger.Zero;
            }

            return (scaleFactor * scaleFactor).DivideRounded(positive);
        }

        var ln2 = Ln2(scale);
        var exponent = scaled.DivideRounded(ln2);
        var remainder = scaled - exponent * ln2;

        if (exponent > MaxBitsForScale(scale) + 2 * scale + 2)
        {
            throw new OverflowException();
        }

        var term = scaleFactor;
        var sum = scaleFactor;

        for (var n = 1; ; n++)
        {
            term = (term * remainder).DivideRounded(scaleFactor);
            term = term.DivideRounded(n);

            if (term.IsZero)
            {
                break;
            }

            sum += term;

            if (n > scale * 2 + 100)
            {
                break;
            }
        }

        if (exponent.IsZero)
        {
            return sum;
        }

        if (exponent > 0)
        {
            return sum << (int)exponent;
        }

        return sum.DivideRounded(BigInteger.One << (int)-exponent);
    }

    public static BigInteger Ln(this BigInteger scaled, int scale)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(scaled);

        var scaleFactor = Pow10(scale);

        if (scaled == scaleFactor)
        {
            return BigInteger.Zero;
        }

        var bits = scaled.GetBitLength() - scaleFactor.GetBitLength();
        var exponent = (int)bits;

        if (exponent > 1_000_000)
        {
            throw new OverflowException();
        }

        var mantissa = exponent >= 0
            ? scaled.DivideRounded(BigInteger.One << exponent)
            : scaled << -exponent;

        while (mantissa < scaleFactor)
        {
            mantissa *= 2;
            exponent--;

            if (exponent < -1_000_000)
            {
                throw new OverflowException();
            }
        }

        while (mantissa >= scaleFactor * 2)
        {
            mantissa = mantissa.DivideRounded(2);
            exponent++;

            if (exponent > 1_000_000)
            {
                throw new OverflowException();
            }
        }

        var z = ((mantissa - scaleFactor) * scaleFactor).DivideRounded(mantissa + scaleFactor);
        var logarithm = 2 * z.AtanhSeries(scale);

        return logarithm + exponent *
            Ln2(scale);
    }

    private static BigInteger AtanhSeries(this BigInteger z, int scale)
    {
        var scaleFactor = Pow10(scale);
        var squared = (z * z).DivideRounded(scaleFactor);
        var term = z;
        var sum = z;

        for (var n = 1; ; n++)
        {
            term = (term * squared).DivideRounded(scaleFactor);

            if (term.IsZero)
            {
                break;
            }

            var addition = term.DivideRounded(SmallInteger(2 * n + 1));

            if (addition.IsZero)
            {
                break;
            }

            sum += addition;

            if (n > scale * 2 + 100)
            {
                break;
            }
        }

        return sum;
    }

    public static BigInteger Sqrt(this BigInteger scaled, int scale)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(scaled);

        if (scaled.IsZero)
        {
            return BigInteger.Zero;
        }

        const int guard = 4;
        var workingScale = scale + 2 * guard;
        var working = scaled * Pow10(2 * guard);
        var root = (working * Pow10(workingScale)).IntegerSqrt();

        return root.Rescale(workingScale, scale);
    }

    private static BigInteger IntegerSqrt(this BigInteger value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);

        if (value.IsZero)
        {
            return BigInteger.Zero;
        }

        var bits = (int)value.GetBitLength();
        var estimate = BigInteger.One << ((bits + 1) / 2);

        while (true)
        {
            var next = (estimate + value / estimate) >> 1;

            if (next >= estimate)
            {
                return estimate;
            }

            estimate = next;
        }
    }

    public static BigInteger RootN(this BigInteger scaled, int root, int scale)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(root);

        if (root == 1)
        {
            return scaled;
        }

        if (scaled < 0)
        {
            if (root % 2 == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(scaled));
            }

            return -(-scaled).RootN(root, scale);
        }

        if (scaled.IsZero)
        {
            return BigInteger.Zero;
        }

        const int guard = 4;
        var workingScale = scale + 2 * guard;
        var working = scaled * Pow10(2 * guard);

        var value = working * BigInteger.Pow(Pow10(workingScale), root - 1);
        var rootValue = value.IntegerRoot(root);

        return rootValue.Rescale(workingScale, scale);
    }

    private static BigInteger IntegerRoot(this BigInteger value, int root)
    {
        var bits = (int)value.GetBitLength();
        var estimate = BigInteger.One << ((bits + root - 1) / root);

        while (true)
        {
            var power = BigInteger.Pow(estimate, root - 1);
            var next = ((root - 1) * estimate + value / power) / root;

            if (next >= estimate)
            {
                return estimate;
            }

            estimate = next;
        }
    }

    /// <summary>
    /// Reduces <paramref name="scaled"/> to an equivalent angle in
    /// <c>[-π/2, π/2]</c> radians and reports whether the cosine of the
    /// original angle has the opposite sign.
    /// </summary>
    private static (BigInteger Reduced, bool NegateCosine) ReduceAngle(
        BigInteger scaled, int scale)
    {
        var integerDigits = scaled.IntegerDigits() - scale;
        var reductionScale = scale + Math.Max(0, integerDigits) + 5;
        var twoPi = 2 * Pi(reductionScale);
        var widened = scaled * Pow10(reductionScale - scale);
        var turns = widened.DivideRounded(twoPi);
        var reduced = (widened - turns * twoPi).Rescale(reductionScale, scale);

        var pi = Pi(scale);
        var halfPi = pi / 2;

        if (reduced > halfPi)
        {
            return (pi - reduced, true);
        }

        if (reduced < -halfPi)
        {
            return (-pi - reduced, true);
        }

        return (reduced, false);
    }

    public static BigInteger Sin(this BigInteger scaled, int scale)
    {
        var scaleFactor = Pow10(scale);

        if (scaled.IsZero)
        {
            return BigInteger.Zero;
        }

        var (reduced, _) = ReduceAngle(scaled, scale);
        var squared = (reduced * reduced).DivideRounded(scaleFactor);
        var sineTerm = reduced;
        var sineSum = reduced;

        for (var n = 1; ; n++)
        {
            sineTerm = -(sineTerm * squared).DivideRounded(scaleFactor);
            sineTerm = sineTerm.DivideRounded(
                SmallInteger(2 * n) * SmallInteger(2 * n + 1));

            if (sineTerm.IsZero)
            {
                break;
            }

            sineSum += sineTerm;

            if (n > scale * 2 + 100)
            {
                break;
            }
        }

        return sineSum;
    }

    public static BigInteger Cos(this BigInteger scaled, int scale)
    {
        var scaleFactor = Pow10(scale);

        if (scaled.IsZero)
        {
            return scaleFactor;
        }

        var (reduced, negateCosine) = ReduceAngle(scaled, scale);
        var squared = (reduced * reduced).DivideRounded(scaleFactor);
        var cosineTerm = scaleFactor;
        var cosineSum = scaleFactor;

        for (var n = 1; ; n++)
        {
            cosineTerm = -(cosineTerm * squared).DivideRounded(scaleFactor);
            cosineTerm = cosineTerm.DivideRounded(
                SmallInteger(2 * n - 1) * SmallInteger(2 * n));

            if (cosineTerm.IsZero)
            {
                break;
            }

            cosineSum += cosineTerm;

            if (n > scale * 2 + 100)
            {
                break;
            }
        }

        return negateCosine ? -cosineSum : cosineSum;
    }

    private static (BigInteger Sine, BigInteger Cosine) SinCos(this BigInteger scaled, int scale)
    {
        var scaleFactor = Pow10(scale);

        if (scaled.IsZero)
        {
            return (BigInteger.Zero, scaleFactor);
        }

        var (reduced, negateCosine) = ReduceAngle(scaled, scale);
        var squared = (reduced * reduced).DivideRounded(scaleFactor);
        var sineTerm = reduced;
        var sineSum = reduced;
        var cosineTerm = scaleFactor;
        var cosineSum = scaleFactor;

        for (var n = 1; ; n++)
        {
            sineTerm = -(sineTerm * squared).DivideRounded(scaleFactor);
            sineTerm = sineTerm.DivideRounded(
                SmallInteger(2 * n) * SmallInteger(2 * n + 1));

            cosineTerm = -(cosineTerm * squared).DivideRounded(scaleFactor);
            cosineTerm = cosineTerm.DivideRounded(
                SmallInteger(2 * n - 1) * SmallInteger(2 * n));

            if (sineTerm.IsZero && cosineTerm.IsZero)
            {
                break;
            }

            sineSum += sineTerm;
            cosineSum += cosineTerm;

            if (n > scale * 2 + 100)
            {
                break;
            }
        }

        return (sineSum, negateCosine ? -cosineSum : cosineSum);
    }

    public static BigInteger Tan(this BigInteger scaled, int scale)
    {
        var scaleFactor = Pow10(scale);
        var (sine, cosine) = scaled.SinCos(scale);

        return (sine * scaleFactor).DivideRounded(cosine);
    }

    public static BigInteger Cot(this BigInteger scaled, int scale)
    {
        var scaleFactor = Pow10(scale);
        var (sine, cosine) = scaled.SinCos(scale);

        return (cosine * scaleFactor).DivideRounded(sine);
    }

    public static BigInteger Atan(this BigInteger scaled, int scale)
    {
        var scaleFactor = Pow10(scale);

        if (scaled.IsZero)
        {
            return BigInteger.Zero;
        }

        var negative = scaled < 0;
        var value = BigInteger.Abs(scaled);
        var reciprocal = value > scaleFactor;

        if (reciprocal)
        {
            value = (scaleFactor * scaleFactor).DivideRounded(value);
        }

        var doublings = 0;
        var quarter = scaleFactor / 4;

        while (value > quarter)
        {
            var square = scaleFactor + (value * value).DivideRounded(scaleFactor);
            var root = square.Sqrt(scale);
            value = (value * scaleFactor).DivideRounded(scaleFactor + root);
            doublings++;
        }

        var squared = (value * value).DivideRounded(scaleFactor);
        var term = value;
        var sum = value;

        for (var n = 1; ; n++)
        {
            term = -(term * squared).DivideRounded(scaleFactor);
            var addition = term.DivideRounded(SmallInteger(2 * n + 1));

            if (addition.IsZero)
            {
                break;
            }

            sum += addition;

            if (n > scale * 2 + 100)
            {
                break;
            }
        }

        sum <<= doublings;

        if (reciprocal)
        {
            sum = Pi(scale) / 2 - sum;
        }

        return negative ? -sum : sum;
    }

    public static BigInteger Atan2(this BigInteger y, BigInteger x, int scale)
    {
        if (x.IsZero)
        {
            if (y.IsZero)
            {
                return BigInteger.Zero;
            }

            return y > 0 ?
                Pi(scale) / 2 :
                -Pi(scale) / 2;
        }

        var quotient = (y * Pow10(scale)).DivideRounded(x);
        var result = quotient.Atan(scale);

        if (x < 0)
        {
            result += y >= 0 ?
                Pi(scale) :
                -Pi(scale);
        }

        return result;
    }

    public static BigInteger Atanh(this BigInteger scaled, int scale)
    {
        var scaleFactor = Pow10(scale);

        if (BigInteger.Abs(scaled) >= scaleFactor)
        {
            throw new ArgumentOutOfRangeException(nameof(scaled));
        }

        var ratio = ((scaleFactor + scaled) * scaleFactor).DivideRounded(scaleFactor - scaled);

        return ratio.Ln(scale) / 2;
    }

    private static int IntegerDigits(this BigInteger value)
    {
        if (value.IsZero)
        {
            return 1;
        }

        var absolute = BigInteger.Abs(value);
        var estimate = (int)((absolute.GetBitLength() * 30_103) / 100_000) + 1;
        var power = Pow10(estimate - 1);

        while (absolute < power)
        {
            estimate--;
            power /= 10;
        }

        while (absolute >= power * 10)
        {
            estimate++;
            power *= 10;
        }

        return estimate;
    }

    public static BigInteger Rescale(this BigInteger scaled, int fromScale, int toScale)
    {
        if (fromScale == toScale)
        {
            return scaled;
        }

        if (fromScale < toScale)
        {
            return scaled * Pow10(toScale - fromScale);
        }

        return scaled.DivideRounded(Pow10(fromScale - toScale));
    }

    public static BigInteger DivideRounded(this BigInteger numerator, BigInteger denominator)
    {
        if (denominator == 0)
        {
            throw new DivideByZeroException();
        }

        var quotient = BigInteger.DivRem(numerator, denominator, out var remainder);

        if (remainder == 0)
        {
            return quotient;
        }

        var isNegative = (numerator < 0) != (denominator < 0);

        if (isNegative)
        {
            return quotient - (BigInteger.Abs(remainder) * 2 >= BigInteger.Abs(denominator)
                ? BigInteger.One
                : BigInteger.Zero);
        }

        return quotient + (BigInteger.Abs(remainder) * 2 >= BigInteger.Abs(denominator)
            ? BigInteger.One
            : BigInteger.Zero);
    }

    public static BigInteger FlooredRemainder(this BigInteger left, BigInteger right)
    {
        var remainder = BigInteger.Remainder(left, right);

        return !remainder.IsZero && remainder.Sign != right.Sign ?
            remainder + right : remainder;
    }
}
