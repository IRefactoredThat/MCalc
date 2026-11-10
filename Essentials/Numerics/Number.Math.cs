using System.Numerics;
using Essentials.Calculator;
using Essentials.ErrorType;
using Essentials.ResultType;

namespace Essentials.Numerics;

public readonly partial struct Number
{
    private const int WorkingGuardDigits = 5;
    private const int MaxResultBits = 100_000;

    private static int WorkingScale => Precision + WorkingGuardDigits;

    private static Result<Number> FromArithmetic(BigInteger scaled, OperatorToken token)
    {
        if (scaled.ExceedsWholeDigitLimit(Precision))
        {
            return new NumberOverflow(token);
        }
        return FromCurrentScale(scaled);
    }

    public Result<Number> AddTo(Number other)
    {
        if (_scale == Precision && other._scale == Precision)
        {
            return FromArithmetic(_scaled + other._scaled, PlusToken.Instance);
        }

        var scale = Math.Max(_scale, other._scale);
        var result = (ToScaled(scale) + other.ToScaled(scale))
            .Rescale(scale, Precision);

        return FromArithmetic(result, PlusToken.Instance);
    }

    public Result<Number> SubtractFrom(Number other)
    {
        if (_scale == Precision && other._scale == Precision)
        {
            return FromArithmetic(_scaled - other._scaled, MinusToken.Instance);
        }

        var scale = Math.Max(_scale, other._scale);
        var result = (ToScaled(scale) - other.ToScaled(scale))
            .Rescale(scale, Precision);

        return FromArithmetic(result, MinusToken.Instance);
    }

    public Result<Number> MultiplyWith(Number other)
    {
        if (_scale == Precision && other._scale == Precision)
        {
            return FromArithmetic(
                (_scaled * other._scaled).DivideRounded(ScaleFactor), TimesToken.Instance);
        }

        var scale = Math.Max(_scale, other._scale);

        var product = ToScaled(scale) * other.ToScaled(scale);

        return FromArithmetic(product.Rescale(scale * 2, Precision), TimesToken.Instance);
    }

    public Result<Number> DivideWith(Number other)
    {
        if (other.IsZero())
        {
            return new ZeroDivision();
        }

        if (_scale == Precision && other._scale == Precision)
        {
            return FromArithmetic(
                (_scaled * ScaleFactor).DivideRounded(other._scaled), DivToken.Instance);
        }

        var scale = Math.Max(_scale, other._scale);

        var dividend = ToScaled(scale) * BigIntegerExtensions.Pow10(Precision);

        return FromArithmetic(dividend.DivideRounded(other.ToScaled(scale)), DivToken.Instance);
    }

    public Result<Number> ModuloWith(Number other)
    {
        if (other.IsZero())
        {
            return new ZeroDivision();
        }

        if (_scale == Precision && other._scale == Precision)
        {
            return FromArithmetic(_scaled.FlooredRemainder(other._scaled), PercentToken.Instance);
        }

        var scale = Math.Max(_scale, other._scale);

        var remainder = ToScaled(scale)
            .FlooredRemainder(other.ToScaled(scale));

        return FromArithmetic(remainder.Rescale(scale, Precision), PercentToken.Instance);
    }

    public Number Negate() => FromScaled(-_scaled, _scale);

    public Number Abs() => _scaled.Sign < 0 ? Negate() : this;

    public Result<Number> Ln()
    {
        if (this <= Zero)
        {
            return new InvalidOperation(LnToken.Instance);
        }

        var scale = WorkingScale;

        return FromWorking(ToScaled(scale).Ln(scale), scale);
    }

    public Result<Number> Log()
    {
        if (this <= Zero)
        {
            return new InvalidOperation(LogToken.Instance);
        }

        var scale = WorkingScale;
        var logarithm = ToScaled(scale).Ln(scale);
        var ln10 = BigIntegerExtensions.Ln10(scale);

        return FromWorking((logarithm * BigIntegerExtensions.Pow10(scale))
            .DivideRounded(ln10), scale);
    }

    public Result<Number> PowWith(Number other)
    {
        if (IsZero())
        {
            if (other.IsZero())
            {
                return One;
            }

            if (other < Zero)
            {
                return new InvalidOperation(CaretToken.Instance);
            }

            return Zero;
        }

        if (other.IsInteger())
        {
            return PowInteger(other.ToBigInteger());
        }

        if (this < Zero)
        {
            return new InvalidOperation(CaretToken.Instance);
        }

        var scale = WorkingScale;
        var logarithm = ToScaled(scale).Ln(scale);
        var exponent = other.ToScaled(scale);
        var product = (logarithm * exponent).DivideRounded(BigIntegerExtensions.Pow10(scale));

        return FromWorking(product.Exp(scale), scale);
    }

    public Number PowInteger(BigInteger exponent)
    {
        if (exponent.IsZero)
        {
            return One;
        }

        var magnitude = Abs();

        if (magnitude == One)
        {
            return this < Zero && !exponent.IsEven ? NegativeOne : One;
        }

        var negative = exponent < 0;
        exponent = BigInteger.Abs(exponent);

        var scale = Precision;
        var scaleFactor = BigIntegerExtensions.Pow10(scale);

        if (!negative && magnitude > One)
        {
            var wholeBits = magnitude.ToScaled(0).GetBitLength() - 1;

            if (wholeBits > 0 &&
                exponent > BigIntegerExtensions.MaxBitsForScale(scale) / wholeBits)
            {
                throw new OverflowException();
            }
        }

        var result = scaleFactor;
        var factor = ToScaled(scale);

        while (!exponent.IsZero)
        {
            if (!exponent.IsEven)
            {
                result = (result * factor).DivideRounded(scaleFactor);

                if (negative)
                {
                    if (result.GetBitLength() > MaxResultBits)
                    {
                        throw new OverflowException();
                    }
                }
                else
                {
                    if (result.ExceedsWholeDigitLimit(scale))
                    {
                        throw new OverflowException();
                    }
                }
            }

            exponent >>= 1;

            if (!exponent.IsZero)
            {
                factor = (factor * factor).DivideRounded(scaleFactor);

                if (negative)
                {
                    if (factor.GetBitLength() > MaxResultBits)
                    {
                        throw new OverflowException();
                    }
                }
                else
                {
                    if (factor.ExceedsWholeDigitLimit(scale))
                    {
                        throw new OverflowException();
                    }
                }
            }
        }

        if (negative)
        {
            if (result.IsZero)
            {
                throw new OverflowException();
            }

            result = (scaleFactor * scaleFactor).DivideRounded(result);
        }

        return FromWorking(result, scale);
    }

    public Result<Number> Sqrt()
    {
        if (this < Zero)
        {
            return new InvalidOperation(SqrtToken.Instance);
        }

        var scale = WorkingScale;

        return FromWorking(ToScaled(scale).Sqrt(scale), scale);
    }

    public Number Cbrt()
    {
        var scale = WorkingScale;
        return FromWorking(ToScaled(scale).RootN(3, scale), scale);
    }

    private static BigInteger DegreesToRadians(BigInteger scaledDegrees, int scale)
    {
        var scaleFactor = BigIntegerExtensions.Pow10(scale);
        var fullCircle = 360 * scaleFactor;
        var halfCircle = 180 * scaleFactor;
        var reduced = scaledDegrees.FlooredRemainder(fullCircle);
        if (reduced > halfCircle)
        {
            reduced -= fullCircle;
        }

        return (reduced * BigIntegerExtensions.Pi(scale)).DivideRounded(halfCircle);
    }

    private static BigInteger RadiansToDegrees(BigInteger radians, int scale)
    {
        var scaleFactor = BigIntegerExtensions.Pow10(scale);
        return (radians * 180 * scaleFactor).DivideRounded(BigIntegerExtensions.Pi(scale));
    }

    private static bool TryQuadrantAngle(BigInteger scaledDegrees, int scale, out int quadrant)
    {
        var scaleFactor = BigIntegerExtensions.Pow10(scale);
        var fullCircle = 360 * scaleFactor;
        var reduced = scaledDegrees.FlooredRemainder(fullCircle);
        var quadrantStep = 90 * scaleFactor;
        var rem = reduced.FlooredRemainder(quadrantStep);
        if (rem.IsZero)
        {
            quadrant = (int)(reduced / quadrantStep);
            return true;
        }

        quadrant = -1;
        return false;
    }

    public Number Sin(AngleMode mode = AngleMode.RAD)
    {
        var scale = WorkingScale;
        var value = ToScaled(scale);

        if (mode is AngleMode.DEG)
        {
            if (TryQuadrantAngle(value, scale, out var quadrant))
            {
                return quadrant switch
                {
                    1 => One,
                    3 => NegativeOne,
                    _ => Zero
                };
            }

            value = DegreesToRadians(value, scale);
        }

        return FromWorking(value.Sin(scale), scale);
    }

    public Number Cos(AngleMode mode = AngleMode.RAD)
    {
        var scale = WorkingScale;
        var value = ToScaled(scale);

        if (mode is AngleMode.DEG)
        {
            if (TryQuadrantAngle(value, scale, out var quadrant))
            {
                return quadrant switch
                {
                    0 => One,
                    2 => NegativeOne,
                    _ => Zero
                };
            }

            value = DegreesToRadians(value, scale);
        }

        return FromWorking(value.Cos(scale), scale);
    }

    public Number Tan(AngleMode mode = AngleMode.RAD)
    {
        var scale = WorkingScale;
        var value = ToScaled(scale);

        if (mode is AngleMode.DEG)
        {
            if (TryQuadrantAngle(value, scale, out var quadrant))
            {
                return quadrant switch
                {
                    0 or 2 => Zero,
                    _ => throw new DivideByZeroException()
                };
            }

            value = DegreesToRadians(value, scale);
        }

        return FromWorking(value.Tan(scale), scale);
    }

    public Result<Number> Asin(AngleMode mode = AngleMode.RAD)
    {
        if (this < NegativeOne || this > One)
        {
            return new InvalidOperation(AsinToken.Instance);
        }

        var scale = WorkingScale;
        var value = ToScaled(scale);
        var scaleFactor = BigIntegerExtensions.Pow10(scale);
        var root = (scaleFactor * scaleFactor - value * value)
            .DivideRounded(scaleFactor).Sqrt(scale);
        var angle = value.Atan2(root, scale);

        if (mode is AngleMode.DEG)
        {
            angle = RadiansToDegrees(angle, scale);
        }

        return FromWorking(angle, scale);
    }

    public Result<Number> Acos(AngleMode mode = AngleMode.RAD)
    {
        if (this < NegativeOne || this > One)
        {
            return new InvalidOperation(AcosToken.Instance);
        }

        var scale = WorkingScale;
        var value = ToScaled(scale);
        var scaleFactor = BigIntegerExtensions.Pow10(scale);
        var root = (scaleFactor * scaleFactor - value * value)
            .DivideRounded(scaleFactor).Sqrt(scale);
        var angle = root.Atan2(value, scale);

        if (mode is AngleMode.DEG)
        {
            angle = RadiansToDegrees(angle, scale);
        }

        return FromWorking(angle, scale);
    }

    public Number Atan(AngleMode mode = AngleMode.RAD)
    {
        var scale = WorkingScale;
        var angle = ToScaled(scale).Atan(scale);

        if (mode is AngleMode.DEG)
        {
            angle = RadiansToDegrees(angle, scale);
        }

        return FromWorking(angle, scale);
    }

    private Number Atan2With(Number other, AngleMode mode = AngleMode.RAD)
    {
        var scale = WorkingScale;
        var angle = other.ToScaled(scale).Atan2(ToScaled(scale), scale);

        if (mode is AngleMode.DEG)
        {
            angle = RadiansToDegrees(angle, scale);
        }

        return FromWorking(angle, scale);
    }

    public Number Sinh()
    {
        var scale = WorkingScale;
        var value = ToScaled(scale);
        var scaleFactor = BigIntegerExtensions.Pow10(scale);
        var exponential = BigInteger.Abs(value).Exp(scale);
        var inverse = (scaleFactor * scaleFactor).DivideRounded(exponential);
        var difference = value.Sign < 0
            ? inverse - exponential
            : exponential - inverse;

        return FromWorking(difference.DivideRounded(2), scale);
    }

    public Number Cosh()
    {
        var scale = WorkingScale;
        var value = ToScaled(scale);
        var scaleFactor = BigIntegerExtensions.Pow10(scale);
        var exponential = BigInteger.Abs(value).Exp(scale);
        var inverse = (scaleFactor * scaleFactor).DivideRounded(exponential);
        var sum = exponential + inverse;

        return FromWorking(sum.DivideRounded(2), scale);
    }

    public Number Tanh()
    {
        var scale = WorkingScale;
        var scaleFactor = BigIntegerExtensions.Pow10(scale);
        var value = 2 * ToScaled(scale);

        if (value > 80 * scaleFactor)
        {
            return One;
        }

        if (value < -80 * scaleFactor)
        {
            return NegativeOne;
        }

        var exponential = value.Exp(scale);
        var numerator = exponential - scaleFactor;
        var denominator = exponential + scaleFactor;

        return FromWorking((numerator * scaleFactor).DivideRounded(denominator), scale);
    }

    public Number Asinh()
    {
        if (this < Zero)
        {
            return Negate().Asinh().Negate();
        }

        var scale = WorkingScale;
        var value = ToScaled(scale);
        var scaleFactor = BigIntegerExtensions.Pow10(scale);
        var root = (value * value + scaleFactor * scaleFactor)
            .DivideRounded(scaleFactor).Sqrt(scale);
        var logarithm = (value + root).Ln(scale);

        return FromWorking(logarithm, scale);
    }

    public Result<Number> Acosh()
    {
        if (this < One)
        {
            return new InvalidOperation(AcoshToken.Instance);
        }

        var scale = WorkingScale;
        var value = ToScaled(scale);
        var scaleFactor = BigIntegerExtensions.Pow10(scale);
        var root = (value * value - scaleFactor * scaleFactor)
            .DivideRounded(scaleFactor).Sqrt(scale);
        var logarithm = (value + root).Ln(scale);

        return FromWorking(logarithm, scale);
    }

    public Result<Number> Atanh()
    {
        if (this <= NegativeOne || this >= One)
        {
            return new InvalidOperation(AtanhToken.Instance);
        }

        var scale = WorkingScale;

        return FromWorking(ToScaled(scale).Atanh(scale), scale);
    }

    public Number Cot(AngleMode mode = AngleMode.RAD)
    {
        var scale = WorkingScale;
        var value = ToScaled(scale);

        if (mode is AngleMode.DEG)
        {
            if (TryQuadrantAngle(value, scale, out var quadrant))
            {
                return quadrant switch
                {
                    1 or 3 => Zero,
                    _ => throw new DivideByZeroException()
                };
            }

            value = DegreesToRadians(value, scale);
        }

        return FromWorking(value.Cot(scale), scale);
    }

    public Number Sec(AngleMode mode = AngleMode.RAD)
    {
        var scale = WorkingScale;
        var value = ToScaled(scale);

        if (mode is AngleMode.DEG)
        {
            if (TryQuadrantAngle(value, scale, out var quadrant))
            {
                return quadrant switch
                {
                    0 => One,
                    2 => NegativeOne,
                    _ => throw new DivideByZeroException()
                };
            }

            value = DegreesToRadians(value, scale);
        }

        var scaleFactor = BigIntegerExtensions.Pow10(scale);
        var cosine = value.Cos(scale);

        return FromWorking((scaleFactor * scaleFactor).DivideRounded(cosine), scale);
    }

    public Number Csc(AngleMode mode = AngleMode.RAD)
    {
        var scale = WorkingScale;
        var value = ToScaled(scale);

        if (mode is AngleMode.DEG)
        {
            if (TryQuadrantAngle(value, scale, out var quadrant))
            {
                return quadrant switch
                {
                    1 => One,
                    3 => NegativeOne,
                    _ => throw new DivideByZeroException()
                };
            }

            value = DegreesToRadians(value, scale);
        }

        var scaleFactor = BigIntegerExtensions.Pow10(scale);
        var sine = value.Sin(scale);

        return FromWorking((scaleFactor * scaleFactor).DivideRounded(sine), scale);
    }

    public Number Acot(AngleMode mode = AngleMode.RAD) => Atan2With(One, mode);

    public Result<Number> Asec(AngleMode mode = AngleMode.RAD)
    {
        if (Abs() < One)
        {
            return new InvalidOperation(AsecToken.Instance);
        }

        var scale = WorkingScale;
        var scaleFactor = BigIntegerExtensions.Pow10(scale);
        var value = (scaleFactor * scaleFactor).DivideRounded(ToScaled(scale));
        var root = (scaleFactor * scaleFactor - value * value)
            .DivideRounded(scaleFactor).Sqrt(scale);
        var angle = root.Atan2(value, scale);

        if (mode is AngleMode.DEG)
        {
            angle = RadiansToDegrees(angle, scale);
        }

        return FromWorking(angle, scale);
    }

    public Result<Number> Acsc(AngleMode mode = AngleMode.RAD)
    {
        if (Abs() < One)
        {
            return new InvalidOperation(AcscToken.Instance);
        }

        var scale = WorkingScale;
        var scaleFactor = BigIntegerExtensions.Pow10(scale);
        var value = (scaleFactor * scaleFactor).DivideRounded(ToScaled(scale));
        var root = (scaleFactor * scaleFactor - value * value)
            .DivideRounded(scaleFactor).Sqrt(scale);
        var angle = value.Atan2(root, scale);

        if (mode is AngleMode.DEG)
        {
            angle = RadiansToDegrees(angle, scale);
        }

        return FromWorking(angle, scale);
    }

    public Result<Number> Coth() => One.DivideWith(Tanh());

    public Result<Number> Sech() => One.DivideWith(Cosh());

    public Result<Number> Csch() => One.DivideWith(Sinh());

    public Result<Number> Acoth() => One.DivideWith(this)
        .SelfMap(result => result.Atanh());

    public Result<Number> Asech() => One.DivideWith(this)
        .SelfMap(result => result.Acosh());

    public Result<Number> Acsch() => One.DivideWith(this)
        .SelfMap(result => result.Asinh());

    public Result<Number> Factorial()
    {
        if (!IsInteger() || this < Zero)
        {
            return new InvalidOperation(ExclamationMarkToken.Instance);
        }

        var count = ToBigInteger();

        if (count > FactorialBound)
        {
            return new NumberOverflow(ExclamationMarkToken.Instance);
        }

        var result = BigInteger.One;

        for (BigInteger i = 2; i <= count; i++)
        {
            result *= i;
        }

        return CreateUnsafe(result);
    }
}
