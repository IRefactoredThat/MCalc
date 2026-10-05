using Calculator.Operands;

namespace Tests;

[TestFixture]
public class SmartRoundTests
{
    private static double Rounded(double value) => new Number(value).SmartRound();

    [Test]
    public void NoiseBelowOneIsErased()
    {
        Assert.That(Rounded(0.1 + 0.2), Is.EqualTo(0.3));
    }

    [Test]
    public void NoiseAboveOneIsErased()
    {
        Assert.That(Rounded(2.0000000000000004), Is.EqualTo(2));
    }

    [Test]
    public void NoiseJustBelowOneIsErased()
    {
        Assert.That(Rounded(0.49999999999999994), Is.EqualTo(0.5));
    }

    [Test]
    public void SubtractingNearlyEqualOperandsIsCloseToZero()
    {
        Assert.That(Rounded((0.1 + 0.2) - 0.3), Is.Zero);
    }

    [Test]
    public void NoiseFromSquaringASquareRootIsErased()
    {
        Assert.That(Rounded(Math.Sqrt(2) * Math.Sqrt(2)), Is.EqualTo(2));
    }

    [Test]
    public void NoiseFromSineOfThirtyDegreesIsErased()
    {
        Assert.That(Rounded(Math.Sin(Math.PI / 6)), Is.EqualTo(0.5));
    }

    [Test]
    public void SignificantDigitsAreKept()
    {
        Assert.That(Rounded(1.0 / 3.0), Is.EqualTo(0.3333333333333333));
    }

    [Test]
    public void IrrationalValuesAreReturnedUnchanged()
    {
        Assert.That(Rounded(Math.PI), Is.EqualTo(Math.PI));
    }

    [Test]
    public void ZeroProducedByMultiplicationStaysZero()
    {
        Assert.That(Rounded(0.0 * 5.0), Is.Zero);
    }

    [Test]
    public void ANumberBelowToleranceIsCollapsedToZero()
    {
        Assert.That(Rounded(OperandExtensions.Tolerance / 2), Is.Zero);
    }

    [Test]
    public void ALargeWholeNumberIsReturnedUnchanged()
    {
        Assert.That(Rounded(1e18), Is.EqualTo(1e18));
    }

    [Test]
    public void RoundingIsIdempotent()
    {
        var once = Rounded(0.1 + 0.2);

        Assert.That(Rounded(once), Is.EqualTo(once));
    }

    [Test]
    public void NegativeNoiseBelowOneIsErased()
    {
        Assert.That(Rounded(-0.1 - 0.2), Is.EqualTo(-0.3));
    }

    [Test]
    public void NegativeNoiseAboveOneIsErased()
    {
        Assert.That(Rounded(-2.0000000000000004), Is.EqualTo(-2));
    }

    [Test]
    public void NegativeNoiseJustBelowOneIsErased()
    {
        Assert.That(Rounded(-0.49999999999999994), Is.EqualTo(-0.5));
    }

    [Test]
    public void ASmallNegativeBelowToleranceIsCollapsedToZero()
    {
        Assert.That(Rounded(-OperandExtensions.Tolerance / 2), Is.Zero);
    }

    [Test]
    public void AValueExactlyAtToleranceIsNotCollapsedToZero()
    {
        const double tolerance = OperandExtensions.Tolerance;
        var rounded = Rounded(tolerance);
        Assert.That(double.Abs(tolerance - rounded),
            Is.LessThanOrEqualTo(tolerance * double.Max(1, tolerance)));
    }

    [Test]
    public void ANegativeValueExactlyAtToleranceIsNotCollapsedToZero()
    {
        const double tolerance = OperandExtensions.Tolerance;
        var rounded = Rounded(-tolerance);
        Assert.That(double.Abs(-tolerance - rounded),
            Is.LessThanOrEqualTo(tolerance * double.Max(1, Math.Abs(tolerance))));
    }

    [Test]
    public void ValueJustAboveToleranceIsPreservedOrRoundedWithinTolerance()
    {
        const double value = OperandExtensions.Tolerance * 1.1;
        var rounded = Rounded(value);
        Assert.That(double.Abs(value - rounded),
            Is.LessThanOrEqualTo(OperandExtensions.Tolerance * double.Max(1, Math.Abs(value))));
    }

    [Test]
    public void NegativeValueJustAboveToleranceIsPreservedOrRoundedWithinTolerance()
    {
        const double value = -OperandExtensions.Tolerance * 1.1;
        var rounded = Rounded(value);
        Assert.That(double.Abs(value - rounded),
            Is.LessThanOrEqualTo(OperandExtensions.Tolerance * double.Max(1, Math.Abs(value))));
    }

    [Test]
    public void SmallPositiveAboveToleranceBehavesCorrectly()
    {
        const double value = 5e-15;
        var rounded = Rounded(value);
        Assert.That(double.Abs(value - rounded),
            Is.LessThanOrEqualTo(OperandExtensions.Tolerance * double.Max(1, Math.Abs(value))));
    }

    [Test]
    public void SmallNegativeAboveToleranceBehavesCorrectly()
    {
        const double value = -5e-15;
        var rounded = Rounded(value);
        Assert.That(double.Abs(value - rounded),
            Is.LessThanOrEqualTo(OperandExtensions.Tolerance * double.Max(1, Math.Abs(value))));
    }

    [Test]
    public void PostRoundingSmallValueDoesNotProduceNegativeZero()
    {
        const double value = 1.1e-15;
        var rounded = Rounded(value);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(double.Abs(value - rounded),
                    Is.LessThanOrEqualTo(OperandExtensions.Tolerance * double.Max(1, Math.Abs(value))));
            Assert.That(rounded == 0 && double.IsNegative(rounded), Is.False);
        }
    }

    [Test]
    public void NegativeSmallValueAboveToleranceRemainsWithinTolerance()
    {
        const double value = -1.1e-15;
        var rounded = Rounded(value);
        Assert.That(double.Abs(value - rounded),
            Is.LessThanOrEqualTo(OperandExtensions.Tolerance * double.Max(1, Math.Abs(value))));
    }

    [Test]
    public void RoundingIsIdempotentForNegativeValues()
    {
        var once = Rounded(-0.1 - 0.2);
        Assert.That(Rounded(once), Is.EqualTo(once));
    }
}
