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
    public void MaximumPrecisionRootsAndLogarithmsAreCorrect() =>
        AtPrecision(Number.MaxPrecision, () =>
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(Text(Value(Number.CreateUnsafe(2).Sqrt())), Is.EqualTo(
                    "1.41421356237309504880168872420969807856967187537695"));
                Assert.That(Text(Number.CreateUnsafe(2).Cbrt()), Is.EqualTo(
                    "1.25992104989487316476721060727822835057025146470151"));
                Assert.That(Text(Value(Number.CreateUnsafe(2).Ln())), Is.EqualTo(
                    "0.69314718055994530941723212145817656807550013436026"));
                Assert.That(Text(Value(Number.CreateUnsafe(2).Log())), Is.EqualTo(
                    "0.30102999566398119521373889472449302676818988146211"));
            }
        });

    [Test]
    public void MaximumPrecisionTrigonometricFunctionsAreCorrect() =>
        AtPrecision(Number.MaxPrecision, () =>
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(Text(Number.One.Sin()), Is.EqualTo(
                    "0.84147098480789650665250232163029899962256306079837"));
                Assert.That(Text(Number.One.Cos()), Is.EqualTo(
                    "0.54030230586813971740093660744297660373231042061792"));
                Assert.That(Text(Number.One.Tan()), Is.EqualTo(
                    "1.55740772465490223050697480745836017308725077238152"));
                Assert.That(Text(Number.One.Cot()), Is.EqualTo(
                    "0.64209261593433070300641998659426562023027811391817"));
                Assert.That(Text(Number.One.Sec()), Is.EqualTo(
                    "1.85081571768092561791175324139865019347039665509401"));
                Assert.That(Text(Number.One.Csc()), Is.EqualTo(
                    "1.18839510577812121626159945237455100352782983409796"));
                Assert.That(Text(Number.One.Atan()), Is.EqualTo(
                    "0.78539816339744830961566084581987572104929234984378"));
                Assert.That(Text(Value(Decimal(0.5m).Asin())), Is.EqualTo(
                    "0.52359877559829887307710723054658381403286156656252"));
                Assert.That(Text(Value(Decimal(0.5m).Acos())), Is.EqualTo(
                    "1.04719755119659774615421446109316762806572313312504"));
            }
        });

    [Test]
    public void MaximumPrecisionHyperbolicFunctionsAreCorrect() =>
        AtPrecision(Number.MaxPrecision, () =>
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(Text(Number.One.Sinh()), Is.EqualTo(
                    "1.1752011936438014568823818505956008151557179813341"));
                Assert.That(Text(Number.One.Cosh()), Is.EqualTo(
                    "1.54308063481524377847790562075706168260152911236586"));
                Assert.That(Text(Number.One.Tanh()), Is.EqualTo(
                    "0.76159415595576488811945828260479359041276859725794"));
                Assert.That(Text(Number.One.Asinh()), Is.EqualTo(
                    "0.88137358701954302523260932497979230902816032826164"));
                Assert.That(Text(Value(Number.CreateUnsafe(2).Acosh())), Is.EqualTo(
                    "1.31695789692481670862504634730796844402698197146752"));
                Assert.That(Text(Value(Decimal(0.5m).Atanh())), Is.EqualTo(
                    "0.54930614433405484569762261846126285232374527891137"));
            }
        });

    [Test]
    public void LargeAnglesAreReducedToMaximumPrecisionAccuracy() =>
        AtPrecision(Number.MaxPrecision, () =>
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(Text(Number.Parse("1E+5", Invariant).Sin()), Is.EqualTo(
                    "0.03574879797201650931647050069580882900904569257811"));
                Assert.That(Text(Number.Parse("1E+10", Invariant).Sin()), Is.EqualTo(
                    "-0.48750602508751069152779429434810604167644731692279"));
                Assert.That(Text(Number.Parse("1E+100", Invariant).Cos()), Is.EqualTo(
                    "-0.92808190507465534345619464377695592818318207643905"));
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
