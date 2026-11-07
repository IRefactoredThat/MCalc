using System.Globalization;
using System.Numerics;
using Essentials.ErrorType;
using Essentials.Numerics;
using Essentials.ResultType;

namespace Tests;

[TestFixture]
public class NumberTests
{
    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

    private static Number Decimal(decimal value) =>
        Number.Parse(value.ToString(Invariant), Invariant);

    private static void AtPrecision(int precision, Action action)
    {
        var previous = Number.Precision;

        try
        {
            Number.Precision = precision;
            action();
        }
        finally
        {
            Number.Precision = previous;
        }
    }

    private static string Text(Number value) =>
        value.ToString("G", Invariant);

    private static Number Value(Result<Number> result)
    {
        Number value = default;
        var failed = false;

        result.Switch(resultValue => value = resultValue, _ => failed = true);

        Assert.That(failed, Is.False, "Expected a value");

        return value;
    }

    private static Error Error(Result<Number> result)
    {
        Error? error = null;
        var succeeded = false;

        result.Switch(_ => succeeded = true, resultError => error = resultError);

        Assert.That(succeeded, Is.False, "Expected an error");

        return error!;
    }

    [Test]
    public void PrecisionControlsTheNumberOfDecimalPlaces() => AtPrecision(3, () =>
    {
        Assert.That(Text(Value(Number.One.DivideWith(Number.CreateUnsafe(3)))),
            Is.EqualTo("0.333"));

        AtPrecision(2, () =>
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(Text(Value(Number.One.DivideWith(Number.CreateUnsafe(3)))),
                    Is.EqualTo("0.33"));
                Assert.That(Text(Value(Number.CreateUnsafe(2).DivideWith(Number.CreateUnsafe(3)))),
                    Is.EqualTo("0.67"));
            }
        });
    });

    [Test]
    public void DecimalArithmeticIsExact() => AtPrecision(3, () =>
    {
        var sum = Value(Decimal(0.1m).AddTo(Decimal(0.2m)));
        var product = Value(Decimal(0.1m).MultiplyWith(Decimal(0.2m)));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(Text(sum), Is.EqualTo("0.3"));
            Assert.That(Text(product), Is.EqualTo("0.02"));
        }
    });

    [Test]
    public void ArithmeticOperationsAreEvaluated() => AtPrecision(3, () =>
    {
        var left = Decimal(0.1m);
        var right = Decimal(0.2m);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(Value(left.AddTo(right)), Is.EqualTo(Decimal(0.3m)));
            Assert.That(Value(right.SubtractFrom(left)), Is.EqualTo(Decimal(0.1m)));
            Assert.That(Value(left.MultiplyWith(right)), Is.EqualTo(Decimal(0.02m)));
            Assert.That(Value(right.DivideWith(left)), Is.EqualTo(Number.CreateUnsafe(2)));
        }
    });

    [Test]
    public void ArithmeticAboveTheWholeDigitLimitIsRejected() => AtPrecision(0, () =>
    {
        var max = Number.Parse(
            new string('9', Number.MaxWholeDigits), Invariant);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(Error(max.AddTo(Number.One)),
                Is.TypeOf<NumberOverflow>());
            Assert.That(Error(max.MultiplyWith(Number.CreateUnsafe(2))),
                Is.TypeOf<NumberOverflow>());
        }
    });

    [Test]
    public void InvalidOperationsReturnErrors()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(Error(Number.One.DivideWith(Number.Zero)),
                Is.TypeOf<ZeroDivision>());
            Assert.That(Error(Number.NegativeOne.Sqrt()),
                Is.TypeOf<InvalidOperation>());
            Assert.That(Error(Number.Zero.Ln()),
                Is.TypeOf<InvalidOperation>());
            Assert.That(Error(Number.CreateUnsafe(-3).Factorial()),
                Is.TypeOf<InvalidOperation>());
        }
    }

    [Test]
    public void ParsingRoundsToTheConfiguredPrecision() => AtPrecision(4, () =>
        Assert.That(Text(Number.Parse("1.234567", Invariant)),
            Is.EqualTo("1.2346")));

    [Test]
    public void PrecisionIsClampedToTheMaximum()
    {
        var previous = Number.Precision;

        try
        {
            Number.Precision = Number.MaxPrecision + 100;

            Assert.That(Number.Precision, Is.EqualTo(Number.MaxPrecision));
        }
        finally
        {
            Number.Precision = previous;
        }
    }

    [Test]
    public void ConstantsAreCorrectAtMaximumPrecision() =>
        AtPrecision(Number.MaxPrecision, () =>
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(Text(Number.Pi), Is.EqualTo(
                    "3.14159265358979323846264338327950288419716939937511"));
                Assert.That(Text(Number.E), Is.EqualTo(
                    "2.71828182845904523536028747135266249775724709369996"));
            }
        });

    [Test]
    public void HighPrecisionConstantsAndSquareRootAreCorrect() => AtPrecision(30, () =>
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(Text(Number.Pi),
                Is.EqualTo("3.14159265358979323846264338328"));
            Assert.That(Text(Number.E),
                Is.EqualTo("2.718281828459045235360287471353"));
            Assert.That(Text(Value(Number.CreateUnsafe(2).Sqrt())),
                Is.EqualTo("1.41421356237309504880168872421"));
        }
    });

    [Test]
    public void ConstantsExceedDoubleResolutionAtSeventeenDecimalPlaces() =>
        AtPrecision(17, () =>
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(Text(Number.Pi),
                    Is.EqualTo("3.14159265358979324"));
                Assert.That(Text(Number.E),
                    Is.EqualTo("2.71828182845904524"));
            }
        });

    [Test]
    public void HighPrecisionTranscendentalFunctionsAreCorrect() => AtPrecision(30, () =>
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(Text(Value(Number.CreateUnsafe(2).Ln())),
                Is.EqualTo("0.693147180559945309417232121458"));
            Assert.That(Text(Number.One.Sin()),
                Is.EqualTo("0.84147098480789650665250232163"));
            Assert.That(Text(Number.One.Cos()),
                Is.EqualTo("0.540302305868139717400936607443"));
            Assert.That(Text(Number.One.Tan()),
                Is.EqualTo("1.557407724654902230506974807458"));
            Assert.That(Text(Number.One.Cot()),
                Is.EqualTo("0.642092615934330703006419986594"));
            Assert.That(Text(Number.One.Atan()),
                Is.EqualTo("0.78539816339744830961566084582"));
            Assert.That(Text(Value(Decimal(0.5m).Asin())),
                Is.EqualTo("0.523598775598298873077107230547"));
            Assert.That(Text(Number.One.Sinh()),
                Is.EqualTo("1.175201193643801456882381850596"));
            Assert.That(Text(Number.One.Tanh()),
                Is.EqualTo("0.761594155955764888119458282605"));
            Assert.That(Text(Value(Number.CreateUnsafe(2).Acosh())),
                Is.EqualTo("1.316957896924816708625046347308"));
        }
    });

    [Test]
    public void SineOfAnAngleWithManyWholeDigitsIsReduced() => AtPrecision(10, () =>
    {
        var sine = Number.Parse("1E+100", Invariant).Sin();

        Assert.That(sine.Abs(), Is.LessThanOrEqualTo(Number.One));
    });

    [Test]
    public void HighPrecisionReciprocalTrigonometricFunctionsAreCorrect() =>
        AtPrecision(30, () =>
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(Text(Number.One.Sec()),
                    Is.EqualTo("1.850815717680925617911753241399"));
                Assert.That(Text(Number.One.Csc()),
                    Is.EqualTo("1.188395105778121216261599452375"));
                Assert.That(Text(Value(Number.One.Coth())),
                    Is.EqualTo("1.31303528549933130363616124693"));
            }
        });

    [Test]
    public void CircularFunctionsAreOddForAnglesBeyondAQuarterTurn() =>
        AtPrecision(30, () =>
        {
            var angle = Value(Number.CreateUnsafe(3).MultiplyWith(Number.Pi)
                .SelfMap(product => product.DivideWith(Number.CreateUnsafe(4))));
            var negativeAngle = angle.Negate();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(negativeAngle.Sin(),
                    Is.EqualTo(angle.Sin().Negate()));
                Assert.That(negativeAngle.Tan(),
                    Is.EqualTo(angle.Tan().Negate()));
                Assert.That(negativeAngle.Cot(),
                    Is.EqualTo(angle.Cot().Negate()));
            }
        });

    [Test]
    public void FactorialOfANonIntegerIsRejected() => AtPrecision(30, () =>
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(Error(Decimal(-0.5m).Factorial()),
                Is.TypeOf<InvalidOperation>());
            Assert.That(Error(Decimal(2.5m).Factorial()),
                Is.TypeOf<InvalidOperation>());
        }
    });

    [Test]
    public void DoubleFallbackIsNotUsedWhenTheValueExceedsDoubleResolution() =>
        AtPrecision(16, () =>
        {
            var square = Number.CreateUnsafe(BigInteger.Parse("10000000000000001"));
            var root = Value(square.Sqrt());

            // Math.Sqrt cannot distinguish 10^16 + 1 from 10^16, so this
            // result can only be produced by the fixed-point path.
            Assert.That(Text(root), Is.EqualTo("100000000.000000005"));
        });

    [Test]
    public void PowHandlesFractionsBetweenZeroAndOne() => AtPrecision(30, () =>
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(Text(Value(Decimal(0.5m).PowWith(Number.CreateUnsafe(2)))),
                Is.EqualTo("0.25"));
            Assert.That(Text(Value(Decimal(0.9m).PowWith(Number.CreateUnsafe(10)))),
                Is.EqualTo("0.3486784401"));

            Assert.That(Value(Decimal(0.5m).PowWith(Number.CreateUnsafe(1000))),
                Is.EqualTo(Number.Zero));
        }
    });

    [Test]
    public void PowHandlesLargeIntegerExponents() => AtPrecision(30, () =>
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(Text(Value(Decimal(1.5m).PowWith(Number.CreateUnsafe(32)))),
                Is.EqualTo("431439.883273989195004105567932128906"));
            Assert.That(Text(Value(Decimal(-1.5m).PowWith(Number.CreateUnsafe(33)))),
                Is.EqualTo("-647159.824910983792506158351898193359"));
        }
    });

    [Test]
    public void ModuloKeepsTheSignOfTheDivisor() => AtPrecision(30, () =>
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(Value(Number.CreateUnsafe(7).ModuloWith(Number.CreateUnsafe(-3))),
                Is.EqualTo(Number.CreateUnsafe(-2)));
            Assert.That(Value(Number.CreateUnsafe(-7).ModuloWith(Number.CreateUnsafe(3))),
                Is.EqualTo(Number.CreateUnsafe(2)));
            Assert.That(Value(Number.CreateUnsafe(-7).ModuloWith(Number.CreateUnsafe(-3))),
                Is.EqualTo(Number.CreateUnsafe(-1)));
        }
    });

    [Test]
    public void FactorialOfLargeIntegerIsExact() => AtPrecision(30, () =>
        Assert.That(
            Text(Value(Number.CreateUnsafe(30).Factorial())),
            Is.EqualTo("265252859812191058636308480000000")));

    [Test]
    public void LargeExponent() => AtPrecision(0, () =>
        Assert.That(
            Text(Value(Number.CreateUnsafe(10).PowWith(Number.CreateUnsafe(400)))),
                Is.EqualTo($"1{new string('0', 400)}")
            ));

    [Test]
    public void CustomDecimalFormatRoundsToTheRequestedPlaces() => AtPrecision(10, () =>
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(Number.Parse("1.235", Invariant).ToString("0.00", Invariant),
                Is.EqualTo("1.24"));
            Assert.That(Number.Parse("1.2", Invariant).ToString("0.00", Invariant),
                Is.EqualTo("1.20"));
        }
    });

    [Test]
    public void CustomDecimalFormatTrimsOptionalTrailingZeros() => AtPrecision(10, () =>
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(Number.Parse("1234.5", Invariant)
                .ToString("#,0.#####", Invariant), Is.EqualTo("1,234.5"));
            Assert.That(Number.Parse("1234.5", Invariant)
                .ToString("0.00##", Invariant), Is.EqualTo("1234.50"));
        }
    });

    [Test]
    public void CustomDecimalFormatGroupsThousands() => AtPrecision(10, () =>
        Assert.That(Number.Parse("1234567.5", Invariant)
            .ToString("#,0.00", Invariant), Is.EqualTo("1,234,567.50")));

    [Test]
    public void CustomFormatAtZeroPrecisionRoundsToAnInteger() => AtPrecision(10, () =>
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(Number.Parse("1234.5", Invariant)
                .ToString("#,0", Invariant), Is.EqualTo("1,235"));
            Assert.That(Number.Parse("1234.4", Invariant)
                .ToString("#,0", Invariant), Is.EqualTo("1,234"));
        }
    });

    [Test]
    public void CustomFormatAppliesTheNegativeSign() => AtPrecision(10, () =>
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(Number.Parse("-12.5", Invariant)
                .ToString("#,0.#", Invariant), Is.EqualTo("-12.5"));
            Assert.That(Number.Parse("-12.5", Invariant)
                .ToString("#,0.0;(#,0.0)", Invariant), Is.EqualTo("(12.5)"));
        }
    });

    [Test]
    public void CustomScientificFormatShowsTheNormalizedExponent() => AtPrecision(10, () =>
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(Number.Parse("1234.5", Invariant)
                .ToString("0.#####E+0", Invariant), Is.EqualTo("1.2345E+3"));
            Assert.That(Number.Parse("0.0012345", Invariant)
                .ToString("0.#####E+0", Invariant), Is.EqualTo("1.2345E-3"));
        }
    });

    [Test]
    public void CustomScientificFormatRoundsAndPadsTheExponent() => AtPrecision(10, () =>
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(Number.Parse("999.5", Invariant)
                .ToString("0E+0", Invariant), Is.EqualTo("1E+3"));
            Assert.That(Number.Parse("1234", Invariant)
                .ToString("0.0E+00", Invariant), Is.EqualTo("1.2E+03"));
        }
    });

    [Test]
    public void CustomFormatUsesTheConfiguredSeparators() => AtPrecision(10, () =>
    {
        var info = new NumberFormatInfo
        {
            NumberDecimalSeparator = ",",
            NumberGroupSeparator = "."
        };

        Assert.That(Number.Parse("1234.5", Invariant)
            .ToString("#,0.00", info), Is.EqualTo("1.234,50"));
    });
}
