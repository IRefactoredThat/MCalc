using System.Numerics;
using Essentials.Calculator;
using Essentials.ErrorType;
using Essentials.ResultType;

namespace Essentials.Numerics;

public readonly partial struct Number
{
    private const int WorkingGuardDigits = 10;
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

    public Number Abs() => this < Zero ? Negate() : this;

    public Result<Number> Ln()
    {
        if (this <= Zero)
        {
            return new InvalidOperation(LnToken.Instance);
        }

        var scale = Precision;

        return FromWorking(ToScaled(scale).Ln(scale), scale);
    }

    public Result<Number> Log()
    {
        if (this <= Zero)
        {
            return new InvalidOperation(LogToken.Instance);
        }

        var scale = Precision;
        var logarithm = ToScaled(scale).Ln(scale);
        var ln10 = (10 * BigIntegerExtensions.Pow10(scale)).Ln(scale);

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

        var scale = Precision;
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

        var scale = Precision;

        return FromWorking(ToScaled(scale).Sqrt(scale), scale);
    }

    public Number Cbrt()
    {
        var scale = Precision;
        return FromWorking(ToScaled(scale).RootN(3, scale), scale);
    }

    public Number Sin()
    {
        var scale = Precision;
        return FromWorking(ToScaled(scale).Sin(scale), scale);
    }

    public Number Cos()
    {
        var scale = Precision;
        return FromWorking(ToScaled(scale).Cos(scale), scale);
    }

    public Number Tan()
    {
        var scale = Precision;
        return FromWorking(ToScaled(scale).Tan(scale), scale);
    }

    public Result<Number> Asin()
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
        return FromWorking(angle, scale);
    }

    public Result<Number> Acos()
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

        return FromWorking(angle, scale);
    }

    public Number Atan()
    {
        var scale = WorkingScale;
        return FromWorking(ToScaled(scale).Atan(scale), scale);
    }

    private Number Atan2With(Number other)
    {
        var scale = WorkingScale;
        return FromWorking(other.ToScaled(scale).Atan2(ToScaled(scale), scale), scale);
    }

    public Result<Number> ToRadians() =>
        MultiplyWith(Pi)
            .SelfMap(result => result.DivideWith(HalfCircleDegrees));

    public Result<Number> ToDegrees() =>
        MultiplyWith(HalfCircleDegrees)
            .SelfMap(result => result.DivideWith(Pi));

    public Number Sinh()
    {
        var scale = WorkingScale;
        var value = ToScaled(scale);
        var positive = value.Exp(scale);
        var negative = (-value).Exp(scale);

        return FromWorking((positive - negative).DivideRounded(2), scale);
    }

    public Number Cosh()
    {
        var scale = Precision;
        var value = ToScaled(scale);
        var positive = value.Exp(scale);
        var negative = (-value).Exp(scale);

        return FromWorking((positive + negative).DivideRounded(2), scale);
    }

    public Number Tanh()
    {
        var scale = Precision;
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

        var scale = Precision;
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

        var scale = Precision;

        return FromWorking(ToScaled(scale).Atanh(scale), scale);
    }

    public Number Cot()
    {
        var scale = WorkingScale;
        return FromWorking(ToScaled(scale).Cot(scale), scale);
    }

    public Number Sec()
    {
        var scale = WorkingScale;
        var scaleFactor = BigIntegerExtensions.Pow10(scale);
        var cosine = ToScaled(scale).Cos(scale);

        return FromWorking((scaleFactor * scaleFactor).DivideRounded(cosine), scale);
    }

    public Number Csc()
    {
        var scale = WorkingScale;
        var scaleFactor = BigIntegerExtensions.Pow10(scale);
        var sine = ToScaled(scale).Sin(scale);

        return FromWorking((scaleFactor * scaleFactor).DivideRounded(sine), scale);
    }

    public Number Acot() => Atan2With(One);

    public Result<Number> Asec() => One.DivideWith(this)
        .SelfMap(result => result.Acos());

    public Result<Number> Acsc() => One.DivideWith(this)
        .SelfMap(result => result.Asin());

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
