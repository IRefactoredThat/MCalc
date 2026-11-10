using System.Globalization;
using Calculator.ExpressionSolving;
using Essentials.Calculator;
using Essentials.ErrorType;
using Essentials.Numerics;
using Essentials.ResultType;

namespace Tests;

[TestFixture]
public class ExpressionEvaluatingTests
{
    private TokenSession _session;

    [SetUp]
    public void Setup()
    {
        Number.Precision = 20;
        _session = new TokenSession();
    }

    [Test]
    public void AdditionOfTwoNumbersIsEvaluated()
    {
        _session.Type("2+3");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("5")));
    }

    [Test]
    public void SubtractionOfTwoNumbersIsEvaluated()
    {
        _session.Type("7-3");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("4")));
    }

    [Test]
    public void MultiplicationOfTwoNumbersIsEvaluated()
    {
        _session.Type("6*7");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("42")));
    }

    [Test]
    public void DivisionOfTwoNumbersIsEvaluated()
    {
        _session.Type("7/2");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("3.5")));
    }

    [Test]
    public void ModuloOfTwoNumbersIsEvaluated()
    {
        _session.Type("7%3");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("1")));
    }

    [Test]
    public void DecimalNumbersAreEvaluated()
    {
        _session.Type("1.5+2.25");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("3.75")));
    }

    [Test]
    public void DecimalSeparatorSettingIsHonouredWhenEvaluating()
    {
        _session.DecimalSeparator = DecimalSeparator.Comma;
        _session.Info.NumberDecimalSeparator = ",";
        _session.Type("1,5+2,5");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("4")));
    }

    [Test]
    public void MultiplicationAppliedBeforeAddition()
    {
        _session.Type("2+3*4");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("14")));
    }

    [Test]
    public void MultiplicationAppliedBeforeSubtraction()
    {
        _session.Type("2*3-4");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("2")));
    }

    [Test]
    public void DivisionAppliedBeforeAddition()
    {
        _session.Type("2+6/3");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("4")));
    }

    [Test]
    public void ModuloAppliedBeforeAddition()
    {
        _session.Type("1+5%3");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("3")));
    }

    [Test]
    public void MultiplicativeOperatorsShareOnePrecedenceLevel()
    {
        _session.Type("2+3*4/2%3");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("2")));
    }

    [Test]
    public void ExponentiationAppliedBeforeMultiplication()
    {
        _session.Type("2*3^2");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("18")));
    }

    [Test]
    public void ExponentiationAppliedBeforeDivision()
    {
        _session.Type("36/3^2");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("4")));
    }

    [Test]
    public void MultiplicationAppliedBeforeExponentiationOnBothSides()
    {
        _session.Type("2^2*3");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("12")));
    }

    [Test]
    public void SubtractionAndAdditionAppliedLast()
    {
        _session.Type("2+3*4-6/2");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("11")));
    }

    [Test]
    public void LeftSideUnaryOperatorAppliedBeforeMultiplication()
    {
        _session.Type("2*sqrt(9)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("6")));
    }

    [Test]
    public void LeftSideUnaryOperatorAppliedBeforeExponentiation()
    {
        _session.Type("2^sin2");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("1.8781306511987291509")));
    }

    [Test]
    public void RightSideUnaryOperatorBeforeExponentiationAppliedBeforeIt()
    {
        _session.Type("3!^2");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("36")));
    }

    [Test]
    public void RightSideUnaryOperatorAfterExponentiationAppliedBeforeIt()
    {
        _session.Type("2^3!");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("64")));
    }

    [Test]
    public void FactorialAppliedBeforeAddition()
    {
        _session.Type("2+3!");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("8")));
    }

    [Test]
    public void NegationAppliedAfterExponentiation()
    {
        _session.Type("-2^2");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("-4")));
    }

    [Test]
    public void NegationAppliesToTheWholeNumberItPrecedes()
    {
        _session.Type("-sqrt(4)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("-2")));
    }

    [Test]
    public void SubtractionIsEvaluatedLeftToRight()
    {
        _session.Type("7-2-3");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("2")));
    }

    [Test]
    public void DivisionIsEvaluatedLeftToRight()
    {
        _session.Type("8/2/2");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("2")));
    }

    [Test]
    public void ExponentiationIsEvaluatedRightToLeft()
    {
        _session.Type("2^2^3");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("256")));
    }

    [Test]
    public void ExponentiationChainIsEvaluatedRightToLeft()
    {
        _session.Type("2^3^2");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("512")));
    }

    [Test]
    public void RightAssociativeExponentiationStopsAtLowerPrecedenceOperators()
    {
        _session.Type("2^3^2+1");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("513")));
    }

    [Test]
    public void NegativeExponentIsSubstitutedBeforeTheChainIsEvaluated()
    {
        _session.Type("2^-3");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("0.125")));
    }

    [Test]
    public void NegationInsideAChainAppliesToTheExponentOnly()
    {
        _session.Type("2^-2^2");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("0.0625")));
    }

    [Test]
    public void GroupedExponentKeepsItsOwnValue()
    {
        _session.Type("2^(0-3)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("0.125")));
    }

    [Test]
    public void NegationAfterExponentEvaluatesBeforeTheFollowingAddition()
    {
        _session.Type("2^-3+4");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("4.125")));
    }

    [Test]
    public void NegationAfterExponentEvaluatesBeforeTheFollowingSubtraction()
    {
        _session.Type("2^-3-4");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("-3.875")));
    }

    [Test]
    public void NegationAfterExponentEvaluatesBeforeTheFollowingMultiplication()
    {
        _session.Type("2^-3*4");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("0.5")));
    }

    [Test]
    public void NegationAfterExponentEvaluatesBeforeTheFollowingDivision()
    {
        _session.Type("2^-3/4");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("0.03125")));
    }

    [Test]
    public void NegationAfterExponentEvaluatesBeforeTheFollowingModulo()
    {
        _session.Type("2^-3%2");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("0.125")));
    }

    [Test]
    public void ExponentiationInsideNegationGroupKeepsItsPrecedence()
    {
        _session.Type("2^-3^2");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("0.001953125")));
    }

    [Test]
    public void LeadingNegationAppliesToTheWholeRightAssociativeChain()
    {
        _session.Type("-2^2^3");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("-256")));
    }

    [Test]
    public void ExponentiationAfterChainedNumbersOnlyAppliesToLastOne()
    {
        _session.Type("2p^2");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("19.73920880217871723764")));
    }

    [Test]
    public void ExponentiationBeforeChainedNumbersOnlyAppliesToFirstOne()
    {
        _session.Type("2^2p");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("12.56637061435917295384")));
    }

    [Test]
    public void RepeatedNegationCancelsOut()
    {
        _session.Type("--5");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("5")));
    }

    [Test]
    public void RepeatedNegationInAChainIsEvaluatedLeftToRight()
    {
        _session.Type("0-0-5");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("-5")));
    }

    [Test]
    public void NegationBeforeAnAdditiveOperatorYieldsTheNegatedNumber()
    {
        _session.Type("+-5");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("-5")));
    }

    [Test]
    public void GroupingOverridesPrecedence()
    {
        _session.Type("(2+3)*4");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("20")));
    }

    [Test]
    public void NestedGroupingIsSubstitutedFromTheInsideOut()
    {
        _session.Type("((2+3))*4");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("20")));
    }

    [Test]
    public void GroupingAroundASingleNumberIsRemoved()
    {
        _session.Type("(((5)))");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("5")));
    }

    [Test]
    public void RedundantGroupingDoesNotChangeTheResult()
    {
        _session.Type("2*(3*4)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("24")));
    }

    [Test]
    public void UnclosedGroupingIsAcceptedAndEvaluated()
    {
        _session.Type("(2+3");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("5")));
    }

    [Test]
    public void UnopenedGroupingIsAcceptedAndEvaluated()
    {
        _session.Type("2+3)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("5")));
    }

    [Test]
    public void GroupingMissingOnlyItsInnermostEndIsAcceptedAndEvaluated()
    {
        _session.Type("(2+(3*4)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("14")));
    }

    [Test]
    public void UserTypedZeroMinusGroupIsEvaluatedCorrectly()
    {
        _session.Type("(0-3*4)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("-12")));
    }

    [Test]
    public void EmptyGroupingIsRejected()
    {
        _session.Type("()");

        Assert.That(_session.Error(), Is.TypeOf<ExpressionContainsEmptyGrouping>());
    }

    [Test]
    public void ExpressionEndingWithAnOpenGroupingIsRejected()
    {
        _session.Type("2+3*(");

        Assert.That(_session.Error(), Is.TypeOf<InvalidExpressionEnd>());
    }

    [Test]
    public void ExpressionEndingWithAClosedGroupingIsEvaluated()
    {
        _session.Type("2+3)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("5")));
    }

    [Test]
    public void FactorialOfAWholeNumberIsEvaluated()
    {
        _session.Type("5!");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("120")));
    }

    [Test]
    public void StackedFactorialsAreEvaluatedFromTheInsideOut()
    {
        _session.Type("3!!");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("720")));
    }

    [Test]
    public void FactorialAppliesToAGroupedExpression()
    {
        _session.Type("(3+2)!");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("120")));
    }

    [Test]
    public void FactorialOfALargeWholeNumberIsEvaluated()
    {
        _session.Type("20!");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("2.43290200817664E+18")));
    }

    [Test]
    public void FactorialAboveTheOverflowBoundaryIsRejected()
    {
        Number.Ten
            .MultiplyWith(Number.Ten)
            .SelfMap(result => result.MultiplyWith(Number.Ten))
            .Switch(result => _session.Type($"{result}!"), _ => _session.Error());
        Assert.That(_session.Error(), Is.TypeOf<NumberOverflow>());
    }

    [Test]
    public void UnaryOperationAboveTheWholeDigitLimitIsRejected()
    {
        _session.Type("sinh(2000)");

        Assert.That(_session.Error(), Is.TypeOf<NumberOverflow>());
    }

    [Test]
    public void FactorialOfANegativeNumberIsRejected()
    {
        _session.Type("(-3)!");

        Assert.That(_session.Error(), Is.TypeOf<InvalidOperation>());
    }

    [Test]
    public void FactorialOfANegativeFractionIsRejected()
    {
        _session.Type("0-0.5!");

        Assert.That(_session.Error(), Is.TypeOf<InvalidOperation>());
    }

    [Test]
    public void FactorialOfAFractionIsRejected()
    {
        _session.Type("0.5!");

        Assert.That(_session.Error(), Is.TypeOf<InvalidOperation>());
    }

    [Test]
    public void FactorialOfPiIsRejected()
    {
        _session.Type("p!");

        Assert.That(_session.Error(), Is.TypeOf<InvalidOperation>());
    }

    [Test]
    public void FactorialWithoutAnNumberIsRejected()
    {
        _session.Type("!");

        Assert.That(_session.Error(), Is.TypeOf<InvalidRightSideUnaryOperatorUse>());
    }

    [Test]
    public void StackedFactorialsWithoutAnNumberAreRejected()
    {
        _session.Type("!!");

        Assert.That(_session.Error(), Is.TypeOf<InvalidRightSideUnaryOperatorUse>());
    }

    [Test]
    public void SquareRootIsEvaluated()
    {
        _session.Type("sqrt(9)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("3")));
    }

    [Test]
    public void SquareRootOfANegativeNumberIsRejected()
    {
        _session.Type("sqrt(0-1)");

        Assert.That(_session.Error(), Is.TypeOf<InvalidOperation>());
    }

    [Test]
    public void CubeRootOfANegativeNumberIsEvaluated()
    {
        _session.Type("cbrt(0-8)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("-2")));
    }

    [Test]
    public void CubeRootIsEvaluated()
    {
        _session.Type("cbrt(27)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("3")));
    }

    [Test]
    public void NaturalLogarithmOfTheEConstantIsOne()
    {
        _session.Type("ln(E)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("1")));
    }

    [Test]
    public void NaturalLogarithmIsEvaluated()
    {
        _session.Type("ln(100)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("4.60517018598809136804")));
    }

    [Test]
    public void NaturalLogarithmOfZeroIsRejected()
    {
        _session.Type("ln(0)");

        Assert.That(_session.Error(), Is.TypeOf<InvalidOperation>());
    }

    [Test]
    public void NaturalLogarithmOfANegativeNumberIsRejected()
    {
        _session.Type("ln(0-1)");

        Assert.That(_session.Error(), Is.TypeOf<InvalidOperation>());
    }

    [Test]
    public void LogarithmIsEvaluatedInBaseTen()
    {
        _session.Type("log(1000)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("3")));
    }

    [Test]
    public void LogarithmOfTwoIsEvaluated()
    {
        _session.Type("log(2)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("0.30102999566398119521")));
    }

    [Test]
    public void LogarithmOfZeroIsRejected()
    {
        _session.Type("log(0)");

        Assert.That(_session.Error(), Is.TypeOf<InvalidOperation>());
    }

    [Test]
    public void LogarithmOfANegativeNumberIsRejected()
    {
        _session.Type("log(0-1)");

        Assert.That(_session.Error(), Is.TypeOf<InvalidOperation>());
    }

    [Test]
    public void LogarithmIsAppliedAfterTheExponentiationInsideIt()
    {
        _session.Type("log(10^3)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("3")));
    }

    [Test]
    public void InverseTrigonometricOperatorsAreEvaluatedInRadiansByDefault()
    {
        _session.Type("asin(1)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("1.57079632679489661923")));
    }

    [TestCase("asin(2)")]
    [TestCase("acos(2)")]
    [TestCase("asec(0.5)")]
    [TestCase("acsc(0.5)")]
    [TestCase("acosh(0.5)")]
    [TestCase("atanh(1)")]
    [TestCase("atanh(2)")]
    [TestCase("cot(0)")]
    [TestCase("csch(0)")]
    public void OperatorOutsideItsDomainIsRejected(string source)
    {
        _session.Type(source);

        Assert.That(_session.Error(), Is.TypeOf<InvalidOperation>());
    }

    [Test]
    public void ArcsecantOfTwoIsSixtyDegreesInDegreeMode()
    {
        _session.Mode = AngleMode.DEG;
        _session.Type("asec(2)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("60")));
    }

    [Test]
    public void NestedLeftSideUnaryOperatorsAreEvaluatedFromTheInsideOut()
    {
        _session.Type("sinsin0");

        Assert.That(_session.Value(), Is.EqualTo(Number.Zero));
    }

    [Test]
    public void SquareRootsThatMultiplyBackToTheNumberRoundCleanly()
    {
        _session.Type("sqrt(2)*sqrt(2)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("2")));
    }

    [Test]
    public void SquaringASquareRootGivesTheOriginalNumber()
    {
        _session.Type("sqrt(2)^2");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("2")));
    }

    [Test]
    public void SineOfThirtyDegreesIsAHalfInDegreeMode()
    {
        _session.Mode = AngleMode.DEG;
        _session.Type("sin(30)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("0.5")));
    }

    [Test]
    public void SineOfThirtyRadiansIsNotAHalf()
    {
        _session.Type("sin(30)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("-0.98803162409286178999")));
    }

    [Test]
    public void TangentOfFortyFiveDegreesIsOneInDegreeMode()
    {
        _session.Mode = AngleMode.DEG;
        _session.Type("tan(45)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("1")));
    }

    [Test]
    public void CosecantOfThirtyDegreesIsTwoInDegreeMode()
    {
        _session.Mode = AngleMode.DEG;
        _session.Type("csc(30)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("2")));
    }

    [Test]
    public void CotangentOfFortyFiveDegreesIsOneInDegreeMode()
    {
        _session.Mode = AngleMode.DEG;
        _session.Type("cot(45)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("1")));
    }

    [Test]
    public void SecantOfSixtyDegreesIsTwoInDegreeMode()
    {
        _session.Mode = AngleMode.DEG;
        _session.Type("sec(60)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("2")));
    }

    [Test]
    public void InverseOperatorsReturnTheirResultInDegreesInDegreeMode()
    {
        _session.Mode = AngleMode.DEG;
        _session.Type("atan(1)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("45")));
    }

    [TestCase("asin(0.5)", 3, "30")]
    [TestCase("acos(0.5)", 3, "60")]
    [TestCase("atan(1)", 3, "45")]
    [TestCase("asec(2)", 3, "60")]
    [TestCase("acsc(2)", 3, "30")]
    [TestCase("sin(30)", 3, "0.5")]
    [TestCase("cos(60)", 3, "0.5")]
    [TestCase("tan(45)", 3, "1")]
    [TestCase("sin(90)", 3, "1")]
    [TestCase("cos(90)", 3, "0")]
    [TestCase("sin(180)", 3, "0")]
    [TestCase("cos(180)", 3, "-1")]
    [TestCase("tan(180)", 3, "0")]
    public void CircularTrigonometryInDegreeModeIsAccurateAtLowPrecision(string expression, int precision, string expected)
    {
        Number.Precision = precision;
        _session.Mode = AngleMode.DEG;
        _session.Type(expression);

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse(expected, CultureInfo.InvariantCulture)));
    }

    [Test]
    public void Tan90DegreesIsInvalidOperation()
    {
        _session.Mode = AngleMode.DEG;
        _session.Type("tan(90)");

        Assert.That(_session.Error(), Is.TypeOf<InvalidOperation>());
    }

    [Test]
    public void InverseOperatorsReadTheirNumberAsARatioInDegreeMode()
    {
        _session.Mode = AngleMode.DEG;
        _session.Type("asin(0.5)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("30")));
    }

    [Test]
    public void InverseOperatorsInterpretTheirNumberAsRadiansInRadianMode()
    {
        _session.Type("atan(1)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("0.78539816339744830962")));
    }

    [Test]
    public void ArcsecantOfTwoIsEvaluatedInRadiansInRadianMode()
    {
        _session.Type("asec(2)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("1.04719755119659774615")));
    }

    [Test]
    public void ArccotangentOfOneIsEvaluatedInDegreesInDegreeMode()
    {
        _session.Mode = AngleMode.DEG;
        _session.Type("acot(1)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("45")));
    }

    [Test]
    public void ArccotangentOfOneIsEvaluatedInRadiansInRadianMode()
    {
        _session.Type("acot(1)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("0.78539816339744830962")));
    }

    [Test]
    public void HyperbolicOperatorGivesTheSameValueInEitherAngleMode()
    {
        _session.Type("sinh(1)");
        var radians = _session.Value();

        _session = new TokenSession { Mode = AngleMode.DEG };
        _session.Type("sinh(1)");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(_session.Value(), Is.EqualTo(Number.Parse("1.17520119364380145688")));
            Assert.That(_session.Value(), Is.EqualTo(radians));
        }
    }

    [Test]
    public void PiConstantIsEvaluated()
    {
        _session.Type("p");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("3.14159265358979323846")));
    }

    [Test]
    public void PiConstantMultipliedByAnNumberIsEvaluated()
    {
        _session.Type("2p");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("6.28318530717958647692")));
    }

    [Test]
    public void UnaryOperatorBeforeChainedNumbersAppliesToTheFirstOneOnly()
    {
        _session.Type("sin2p");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("2.85664211604366420006")));
    }

    [Test]
    public void EConstantIsEvaluated()
    {
        _session.Type("E");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("2.71828182845904523536")));
    }

    [Test]
    public void CosineOfPiIsMinusOneInRadianMode()
    {
        _session.Type("cos(p)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("-1")));
    }

    [Test]
    public void CosineOfOneHundredEightyDegreesIsMinusOneInDegreeMode()
    {
        _session.Mode = AngleMode.DEG;
        _session.Type("cos(180)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("-1")));
    }

    [Test]
    public void PiSquaredThroughTheExponentiationOperatorIsEvaluated()
    {
        _session.Type("p^2");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("9.86960440108935861882")));
    }

    [Test]
    public void ConstantsSeparatedByAnOperatorAreNotFoldedBeforeEvaluation()
    {
        _session.Type("p+p");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("6.28318530717958647692")));
    }

    [Test]
    public void NaturalLogarithmOfTheSquaredEConstantIsTwo()
    {
        _session.Type("ln(E^2)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("2")));
    }

    [Test]
    public void ModuloKeepsTheSignOfTheLeftNumber()
    {
        _session.Type("0-7%3");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("-1")));
    }

    [Test]
    public void ModuloOfZeroByANonZeroNumberIsZero()
    {
        _session.Type("0%5");

        Assert.That(_session.Value(), Is.EqualTo(Number.Zero));
    }

    [Test]
    public void ModuloOfAValueSmallerThanTheDivisorIsTheValueItself()
    {
        _session.Type("3%7");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("3")));
    }

    [Test]
    public void ModuloByAFractionalDivisorIsEvaluated()
    {
        _session.Type("5%2.5");

        Assert.That(_session.Value(), Is.EqualTo(Number.Zero));
    }

    [Test]
    public void ExponentiationAboveTheWholeDigitLimitIsRejected()
    {
        _session.Type("9^999");

        Assert.That(_session.Error(), Is.TypeOf<NumberOverflow>());
    }

    [Test]
    public void AdditionAboveTheWholeDigitLimitIsRejected()
    {
        _session.Type($"{new string('9', Number.MaxWholeDigits)}+1");

        Assert.That(_session.Error(), Is.TypeOf<NumberOverflow>());
    }

    [Test]
    public void ZeroToANegativeExponentIsRejected()
    {
        _session.Type("0^-1");

        Assert.That(_session.Error(), Is.TypeOf<InvalidOperation>());
    }

    [Test]
    public void EmptyInputIsRejected()
    {
        Assert.That(_session.Error(), Is.TypeOf<EmptyExpression>());
    }

    [Test]
    public void ExpressionEndingWithABinaryOperatorIsRejected()
    {
        _session.Type("4+");

        Assert.That(_session.Error(), Is.TypeOf<InvalidExpressionEnd>());
    }

    [Test]
    public void ExpressionThatCannotCollapseIsReportedAsNotEvaluated()
    {
        _session.Type(")");

        Assert.That(_session.Error(), Is.TypeOf<ExpressionNotEvaluated>());
    }

    [Test]
    public void GroupingEndFollowedByAnNumberStillCannotCollapse()
    {
        _session.Type(")2");

        Assert.That(_session.Error(), Is.TypeOf<ExpressionNotEvaluated>());
    }

    [Test]
    public void RepeatedGroupingEndsCannotCollapse()
    {
        _session.Type("))");

        Assert.That(_session.Error(), Is.TypeOf<ExpressionNotEvaluated>());
    }

    [Test]
    public void EmptyExpressionCannotBeEvaluated()
    {
        var expression = Expression.Create(AngleMode.RAD);

        var error = default(Error);

        ExpressionEvaluatingExtensions.EvaluateAndRound(expression)
            .Switch(_ => { }, failure => error = failure);

        Assert.That(error, Is.TypeOf<ExpressionNotEvaluated>());
    }

    [Test]
    public void SingleNumberIsEvaluatedEvenWhenItIsFifteenDigitsLong()
    {
        _session.Type("123456789012345*1");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("123456789012345")));
    }

    [Test]
    public void DigitsBeyondTheLimitAreNotAccepted()
    {
        _session.Type(new string('1', Number.MaxWholeDigits + 1));

        Assert.That(_session.Error(), Is.TypeOf<EmptyExpression>());
    }

    [Test]
    public void TheDigitLimitAppliesToTheWholePartOnly()
    {
        var fraction = new string('1', Number.MaxWholeDigits);
        _session.Type($"1.{fraction}*1");

        Assert.That(_session.RenderedText(), Is.EqualTo($"1.{fraction}×1"));
    }

    [Test]
    public void TwoNumbersMayEachUseTheWholeDigitLimit()
    {
        _session.Type("1234567890+1234567890");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("2469135780")));
    }

    [Test]
    public void FloatingPointNoiseIsRoundedAway()
    {
        _session.Type("0.1+0.2");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("0.3")));
    }

    [Test]
    public void SignificantDigitsAreNotRoundedAway()
    {
        _session.Type("1/3");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("0.33333333333333333333")));
    }

    [Test]
    public void AccumulatedNoiseIsRoundedAway()
    {
        _session.Type("0.1+0.2+0.3");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("0.6")));
    }

    [Test]
    public void SineOfPiOverSixIsAHalf()
    {
        _session.Type("sin(p/6)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("0.5")));
    }

    [Test]
    public void ExactZeroStaysZero()
    {
        _session.Type("0*5");

        Assert.That(_session.Value(), Is.EqualTo(Number.Zero));
    }

    [Test]
    public void SubtractingZeroFromZeroStaysZero()
    {
        _session.Type("0-0");

        Assert.That(_session.Value(), Is.EqualTo(Number.Zero));
    }

    [Test]
    public void LargeExactProductsAreNotDisturbed()
    {
        _session.Type("1000000*1000000");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("1000000000000")));
    }

    [Test]
    public void SmallNegativeResultsKeepTheirSign()
    {
        _session.Type("0-0.0000001");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("-1E-07")));
    }

    [Test]
    public void MultiplicationByZeroIsEvaluatedToZero()
    {
        _session.Type("6*0");

        Assert.That(_session.Value(), Is.EqualTo(Number.Zero));
    }

    [Test]
    public void MultiplicationByZeroInAChainStaysZero()
    {
        _session.Type("6*0*2");

        Assert.That(_session.Value(), Is.EqualTo(Number.Zero));
    }

    [Test]
    public void DivisionByZeroIsRejected()
    {
        _session.Type("6/0");

        Assert.That(_session.Error(), Is.TypeOf<InvalidOperation>());
    }

    [Test]
    public void ModuloOperatorTreatsAZeroModulusAsANoOp()
    {
        _session.Type("6%0");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("6")));
    }

    [Test]
    public void NestedArithmeticExpressionIsEvaluated()
    {
        _session.Type("((1+2)*(3+4)+(5-6)^2)*(2^3+1)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("198")));
    }

    [Test]
    public void LongExpressionMixingRootsPowersAndFactorialsIsEvaluated()
    {
        _session.Type("sqrt(2)*sqrt(2)+cbrt(27)+(2^10)/4-5!");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("141")));
    }

    [Test]
    public void LongExpressionMixingLogarithmsAndRootsIsEvaluated()
    {
        _session.Type("ln(E^5)-log(1000)+sqrt(16)");

        Assert.That(_session.Value(), Is.EqualTo(Number.Parse("6")));
    }

    [Test]
    public void LongTrigonometricIdentityIsEvaluated()
    {
        _session.Type("sin(p/6)^2+cos(p/6)^2");

        Assert.That(_session.Value(), Is.EqualTo(Number.One));
    }

    [Test]
    public void LongIntegerExpressionIsExactAtMaximumPrecision()
    {
        Number.Precision = Number.MaxPrecision;
        _session.Type("((2^10+24)*(3^5-3))+123456789012345-(10^6)");

        Assert.That(_session.Value().ToString("G", CultureInfo.InvariantCulture),
            Is.EqualTo("123456788263865"));
    }

    [TestCase("sqrt(9)+(2^3)*2", "19")]
    [TestCase("(5!)/(3+2)", "24")]
    [TestCase("(2+3)^2-(4^2)", "9")]
    public void MixedOperatorsRespectGroupingAndPrecedence(string expression, string expected)
    {
        _session.Type(expression);

        Assert.That(_session.Value().ToString("G", CultureInfo.InvariantCulture),
            Is.EqualTo(expected));
    }

}
