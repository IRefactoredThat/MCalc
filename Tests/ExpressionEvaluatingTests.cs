using Calculator.ExpressionSolving;
using Essentials.Calculator;
using Essentials.ErrorType;
using Essentials.ResultType;

namespace Tests;

[TestFixture]
public class ExpressionEvaluatingTests
{
    private TokenSession _session;

    [SetUp]
    public void Setup() => _session = new TokenSession();

    [Test]
    public void AdditionOfTwoOperandsIsEvaluated()
    {
        _session.Type("2+3");

        Assert.That(_session.Value(), Is.EqualTo(5));
    }

    [Test]
    public void SubtractionOfTwoOperandsIsEvaluated()
    {
        _session.Type("7-3");

        Assert.That(_session.Value(), Is.EqualTo(4));
    }

    [Test]
    public void MultiplicationOfTwoOperandsIsEvaluated()
    {
        _session.Type("6*7");

        Assert.That(_session.Value(), Is.EqualTo(42));
    }

    [Test]
    public void DivisionOfTwoOperandsIsEvaluated()
    {
        _session.Type("7/2");

        Assert.That(_session.Value(), Is.EqualTo(3.5));
    }

    [Test]
    public void ModuloOfTwoOperandsIsEvaluated()
    {
        _session.Type("7%3");

        Assert.That(_session.Value(), Is.EqualTo(1));
    }

    [Test]
    public void DecimalOperandsAreEvaluated()
    {
        _session.Type("1.5+2.25");

        Assert.That(_session.Value(), Is.EqualTo(3.75));
    }

    [Test]
    public void DecimalSeparatorSettingIsHonouredWhenEvaluating()
    {
        _session.DecimalSeparator = DecimalSeparator.Comma;
        _session.Info.NumberDecimalSeparator = ",";
        _session.Type("1,5+2,5");

        Assert.That(_session.Value(), Is.EqualTo(4));
    }

    [Test]
    public void MultiplicationAppliedBeforeAddition()
    {
        _session.Type("2+3*4");

        Assert.That(_session.Value(), Is.EqualTo(14));
    }

    [Test]
    public void MultiplicationAppliedBeforeSubtraction()
    {
        _session.Type("2*3-4");

        Assert.That(_session.Value(), Is.EqualTo(2));
    }

    [Test]
    public void DivisionAppliedBeforeAddition()
    {
        _session.Type("2+6/3");

        Assert.That(_session.Value(), Is.EqualTo(4));
    }

    [Test]
    public void ModuloAppliedBeforeAddition()
    {
        _session.Type("1+5%3");

        Assert.That(_session.Value(), Is.EqualTo(3));
    }

    [Test]
    public void MultiplicativeOperatorsShareOnePrecedenceLevel()
    {
        _session.Type("2+3*4/2%3");

        Assert.That(_session.Value(), Is.EqualTo(2));
    }

    [Test]
    public void ExponentiationAppliedBeforeMultiplication()
    {
        _session.Type("2*3^2");

        Assert.That(_session.Value(), Is.EqualTo(18));
    }

    [Test]
    public void ExponentiationAppliedBeforeDivision()
    {
        _session.Type("36/3^2");

        Assert.That(_session.Value(), Is.EqualTo(4));
    }

    [Test]
    public void MultiplicationAppliedBeforeExponentiationOnBothSides()
    {
        _session.Type("2^2*3");

        Assert.That(_session.Value(), Is.EqualTo(12));
    }

    [Test]
    public void SubtractionAndAdditionAppliedLast()
    {
        _session.Type("2+3*4-6/2");

        Assert.That(_session.Value(), Is.EqualTo(11));
    }

    [Test]
    public void LeftSideUnaryOperatorAppliedBeforeMultiplication()
    {
        _session.Type("2*sqrt(9)");

        Assert.That(_session.Value(), Is.EqualTo(6));
    }

    [Test]
    public void RightSideUnaryOperatorAppliedBeforeExponentiation()
    {
        _session.Type("3!^2");

        Assert.That(_session.Value(), Is.EqualTo(36));
    }

    [Test]
    public void FactorialAppliedBeforeAddition()
    {
        _session.Type("2+3!");

        Assert.That(_session.Value(), Is.EqualTo(8));
    }

    [Test]
    public void NegationAppliedAfterThanExponentiation()
    {
        _session.Type("-2^2");

        Assert.That(_session.Value(), Is.EqualTo(-4));
    }

    [Test]
    public void NegationAppliesToTheWholeOperandItPrecedes()
    {
        _session.Type("-sqrt(4)");

        Assert.That(_session.Value(), Is.EqualTo(-2));
    }

    [Test]
    public void SubtractionIsEvaluatedLeftToRight()
    {
        _session.Type("7-2-3");

        Assert.That(_session.Value(), Is.EqualTo(2));
    }

    [Test]
    public void DivisionIsEvaluatedLeftToRight()
    {
        _session.Type("8/2/2");

        Assert.That(_session.Value(), Is.EqualTo(2));
    }

    [Test]
    public void ExponentiationIsEvaluatedLeftToRight()
    {
        _session.Type("2^2^3");

        Assert.That(_session.Value(), Is.EqualTo(64));
    }

    [Test]
    public void ExponentiationChainIsEvaluatedAppliedLeftToRight()
    {
        _session.Type("2^3^2");

        Assert.That(_session.Value(), Is.EqualTo(64));
    }

    [Test]
    public void NegativeExponentIsSubstitutedBeforeTheChainIsEvaluated()
    {
        _session.Type("2^-3");

        Assert.That(_session.Value(), Is.EqualTo(0.125));
    }

    [Test]
    public void NegationInsideAChainAppliesToTheExponentOnly()
    {
        _session.Type("2^-2^2");

        Assert.That(_session.Value(), Is.EqualTo(0.0625));
    }

    [Test]
    public void GroupedExponentKeepsItsOwnValue()
    {
        _session.Type("2^(0-3)");

        Assert.That(_session.Value(), Is.EqualTo(0.125));
    }

    [Test]
    public void RepeatedNegationCancelsOut()
    {
        _session.Type("--5");

        Assert.That(_session.Value(), Is.EqualTo(5));
    }

    [Test]
    public void RepeatedNegationInAChainIsEvaluatedLeftToRight()
    {
        _session.Type("0-0-5");

        Assert.That(_session.Value(), Is.EqualTo(-5));
    }

    [Test]
    public void NegationBeforeAnAdditiveOperatorYieldsTheNegatedOperand()
    {
        _session.Type("+-5");

        Assert.That(_session.Value(), Is.EqualTo(-5));
    }

    [Test]
    public void GroupingOverridesPrecedence()
    {
        _session.Type("(2+3)*4");

        Assert.That(_session.Value(), Is.EqualTo(20));
    }

    [Test]
    public void NestedGroupingIsSubstitutedFromTheInsideOut()
    {
        _session.Type("((2+3))*4");

        Assert.That(_session.Value(), Is.EqualTo(20));
    }

    [Test]
    public void GroupingAroundASingleOperandIsRemoved()
    {
        _session.Type("(((5)))");

        Assert.That(_session.Value(), Is.EqualTo(5));
    }

    [Test]
    public void RedundantGroupingDoesNotChangeTheResult()
    {
        _session.Type("2*(3*4)");

        Assert.That(_session.Value(), Is.EqualTo(24));
    }

    [Test]
    public void UnclosedGroupingIsAcceptedAndEvaluated()
    {
        _session.Type("(2+3");

        Assert.That(_session.Value(), Is.EqualTo(5));
    }

    [Test]
    public void UnopenedGroupingIsAcceptedAndEvaluated()
    {
        _session.Type("2+3)");

        Assert.That(_session.Value(), Is.EqualTo(5));
    }

    [Test]
    public void GroupingMissingOnlyItsInnermostEndIsAcceptedAndEvaluated()
    {
        _session.Type("(2+(3*4)");

        Assert.That(_session.Value(), Is.EqualTo(14));
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

        Assert.That(_session.Value(), Is.EqualTo(5));
    }

    [Test]
    public void FactorialOfAWholeNumberIsEvaluated()
    {
        _session.Type("5!");

        Assert.That(_session.Value(), Is.EqualTo(120));
    }

    [Test]
    public void StackedFactorialsAreEvaluatedFromTheInsideOut()
    {
        _session.Type("3!!");

        Assert.That(_session.Value(), Is.EqualTo(720));
    }

    [Test]
    public void FactorialAppliesToAGroupedExpression()
    {
        _session.Type("(3+2)!");

        Assert.That(_session.Value(), Is.EqualTo(120));
    }

    [Test]
    public void FactorialOfALargeWholeNumberIsEvaluated()
    {
        _session.Type("20!");

        Assert.That(_session.Value(), Is.EqualTo(2.43290200817664E+18));
    }

    [Test]
    public void FactorialAtTheOverflowBoundaryIsStillEvaluated()
    {
        _session.Type("170!");

        Assert.That(_session.Value(), Is.EqualTo(7.257415615307994E+306));
    }

    [Test]
    public void FactorialAboveTheOverflowBoundaryIsRejected()
    {
        _session.Type("171!");

        Assert.That(_session.Error(), Is.TypeOf<NumberOverflow>());
    }

    [Test]
    public void FactorialOfANegativeOperandIsRejected()
    {
        _session.Type("(-3)!");

        Assert.That(_session.Error(), Is.TypeOf<UndefinedOperation>());
    }

    [Test]
    public void FactorialOfANegativeFractionIsEvaluatedToItsGammaValue()
    {
        _session.Type("0-0.5!");

        Assert.That(_session.Value(), Is.EqualTo(-0.8862269254527586));
    }

    [Test]
    public void FactorialOfAFractionIsEvaluatedToItsGammaValue()
    {
        _session.Type("0.5!");

        Assert.That(_session.Value(), Is.EqualTo(0.8862269254527586));
    }

    [Test]
    public void FactorialOfPiIsEvaluatedToItsGammaValue()
    {
        _session.Type("p!");

        Assert.That(_session.Value(), Is.EqualTo(7.188082728976035));
    }

    [Test]
    public void FactorialWithoutAnOperandIsRejected()
    {
        _session.Type("!");

        Assert.That(_session.Error(), Is.TypeOf<InvalidRightSideUnaryOperatorUse>());
    }

    [Test]
    public void StackedFactorialsWithoutAnOperandAreRejected()
    {
        _session.Type("!!");

        Assert.That(_session.Error(), Is.TypeOf<InvalidRightSideUnaryOperatorUse>());
    }

    [Test]
    public void SquareRootIsEvaluated()
    {
        _session.Type("sqrt(9)");

        Assert.That(_session.Value(), Is.EqualTo(3));
    }

    [Test]
    public void SquareRootOfANegativeOperandIsRejected()
    {
        _session.Type("sqrt(0-1)");

        Assert.That(_session.Error(), Is.TypeOf<InvalidOperation>());
    }

    [Test]
    public void CubeRootOfANegativeOperandIsEvaluated()
    {
        _session.Type("cbrt(0-8)");

        Assert.That(_session.Value(), Is.EqualTo(-2));
    }

    [Test]
    public void CubeRootIsEvaluated()
    {
        _session.Type("cbrt(27)");

        Assert.That(_session.Value(), Is.EqualTo(3));
    }

    [Test]
    public void NaturalLogarithmOfTheEConstantIsOne()
    {
        _session.Type("ln(E)");

        Assert.That(_session.Value(), Is.EqualTo(1));
    }

    [Test]
    public void NaturalLogarithmIsEvaluated()
    {
        _session.Type("ln(100)");

        Assert.That(_session.Value(), Is.EqualTo(4.605170185988092));
    }

    [Test]
    public void NaturalLogarithmOfZeroIsRejected()
    {
        _session.Type("ln(0)");

        Assert.That(_session.Error(), Is.TypeOf<InvalidOperation>());
    }

    [Test]
    public void NaturalLogarithmOfANegativeOperandIsRejected()
    {
        _session.Type("ln(0-1)");

        Assert.That(_session.Error(), Is.TypeOf<InvalidOperation>());
    }

    [Test]
    public void LogarithmIsEvaluatedInBaseTen()
    {
        _session.Type("log(1000)");

        Assert.That(_session.Value(), Is.EqualTo(3));
    }

    [Test]
    public void LogarithmOfTwoIsEvaluated()
    {
        _session.Type("log(2)");

        Assert.That(_session.Value(), Is.EqualTo(0.3010299956639812));
    }

    [Test]
    public void LogarithmOfZeroIsRejected()
    {
        _session.Type("log(0)");

        Assert.That(_session.Error(), Is.TypeOf<InvalidOperation>());
    }

    [Test]
    public void LogarithmOfANegativeOperandIsRejected()
    {
        _session.Type("log(0-1)");

        Assert.That(_session.Error(), Is.TypeOf<InvalidOperation>());
    }

    [Test]
    public void LogarithmIsAppliedAfterTheExponentiationInsideIt()
    {
        _session.Type("log(10^3)");

        Assert.That(_session.Value(), Is.EqualTo(3));
    }

    [Test]
    public void InverseTrigonometricOperatorsAreEvaluatedInRadiansByDefault()
    {
        _session.Type("asin(1)");

        Assert.That(_session.Value(), Is.EqualTo(1.5707963267948966));
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

        Assert.That(_session.Value(), Is.EqualTo(60));
    }

    [Test]
    public void ArcsecantOfOneHundredDegreesIsInsideItsDomainInDegreeMode()
    {
        _session.Mode = AngleMode.DEG;
        _session.Type("asec(100)");

        Assert.That(double.IsFinite(_session.Value()), Is.True);
    }

    [Test]
    public void NestedLeftSideUnaryOperatorsAreEvaluatedFromTheInsideOut()
    {
        _session.Type("sinsin0");

        Assert.That(_session.Value(), Is.Zero);
    }

    [Test]
    public void SquareRootsThatMultiplyBackToTheOperandRoundCleanly()
    {
        _session.Type("sqrt(2)*sqrt(2)");

        Assert.That(_session.Value(), Is.EqualTo(2));
    }

    [Test]
    public void SquaringASquareRootGivesTheOriginalOperand()
    {
        _session.Type("sqrt(2)^2");

        Assert.That(_session.Value(), Is.EqualTo(2));
    }

    [Test]
    public void SineOfThirtyDegreesIsAHalfInDegreeMode()
    {
        _session.Mode = AngleMode.DEG;
        _session.Type("sin(30)");

        Assert.That(_session.Value(), Is.EqualTo(0.5));
    }

    [Test]
    public void SineOfThirtyRadiansIsNotAHalf()
    {
        _session.Type("sin(30)");

        Assert.That(_session.Value(), Is.EqualTo(-0.9880316240928618));
    }

    [Test]
    public void TangentOfFortyFiveDegreesIsOneInDegreeMode()
    {
        _session.Mode = AngleMode.DEG;
        _session.Type("tan(45)");

        Assert.That(_session.Value(), Is.EqualTo(1));
    }

    [Test]
    public void CosecantOfThirtyDegreesIsTwoInDegreeMode()
    {
        _session.Mode = AngleMode.DEG;
        _session.Type("csc(30)");

        Assert.That(_session.Value(), Is.EqualTo(2));
    }

    [Test]
    public void CotangentOfFortyFiveDegreesIsOneInDegreeMode()
    {
        _session.Mode = AngleMode.DEG;
        _session.Type("cot(45)");

        Assert.That(_session.Value(), Is.EqualTo(1));
    }

    [Test]
    public void SecantOfSixtyDegreesIsTwoInDegreeMode()
    {
        _session.Mode = AngleMode.DEG;
        _session.Type("sec(60)");

        Assert.That(_session.Value(), Is.EqualTo(2));
    }

    [Test]
    public void InverseOperatorsReturnTheirResultInDegreesInDegreeMode()
    {
        _session.Mode = AngleMode.DEG;
        _session.Type("atan(1)");

        Assert.That(_session.Value(), Is.EqualTo(45));
    }

    [Test]
    public void InverseOperatorsReadTheirOperandAsARatioInDegreeMode()
    {
        _session.Mode = AngleMode.DEG;
        _session.Type("asin(0.5)");

        Assert.That(_session.Value(), Is.EqualTo(30));
    }

    [Test]
    public void InverseOperatorsInterpretTheirOperandAsRadiansInRadianMode()
    {
        _session.Type("atan(1)");

        Assert.That(_session.Value(), Is.EqualTo(0.7853981633974483));
    }

    [Test]
    public void ArcsecantOfTwoIsEvaluatedInRadiansInRadianMode()
    {
        _session.Type("asec(2)");

        Assert.That(_session.Value(), Is.EqualTo(1.0471975511965979));
    }

    [Test]
    public void ArccotangentOfOneIsEvaluatedInDegreesInDegreeMode()
    {
        _session.Mode = AngleMode.DEG;
        _session.Type("acot(1)");

        Assert.That(_session.Value(), Is.EqualTo(45));
    }

    [Test]
    public void ArccotangentOfOneIsEvaluatedInRadiansInRadianMode()
    {
        _session.Type("acot(1)");

        Assert.That(_session.Value(), Is.EqualTo(0.7853981633974483));
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
            Assert.That(_session.Value(), Is.EqualTo(1.1752011936438014));
            Assert.That(_session.Value(), Is.EqualTo(radians));
        }
    }

    [Test]
    public void PiConstantIsEvaluated()
    {
        _session.Type("p");

        Assert.That(_session.Value(), Is.EqualTo(3.141592653589793));
    }

    [Test]
    public void PiConstantMultipliedByAnOperandIsEvaluated()
    {
        _session.Type("2p");

        Assert.That(_session.Value(), Is.EqualTo(6.283185307179586));
    }

    [Test]
    public void EConstantIsEvaluated()
    {
        _session.Type("E");

        Assert.That(_session.Value(), Is.EqualTo(2.718281828459045));
    }

    [Test]
    public void CosineOfPiIsMinusOneInRadianMode()
    {
        _session.Type("cos(p)");

        Assert.That(_session.Value(), Is.EqualTo(-1));
    }

    [Test]
    public void CosineOfOneHundredEightyDegreesIsMinusOneInDegreeMode()
    {
        _session.Mode = AngleMode.DEG;
        _session.Type("cos(180)");

        Assert.That(_session.Value(), Is.EqualTo(-1));
    }

    [Test]
    public void PiSquaredThroughTheExponentiationOperatorIsEvaluated()
    {
        _session.Type("p^2");

        Assert.That(_session.Value(), Is.EqualTo(9.869604401089358));
    }

    [Test]
    public void ConstantsSeparatedByAnOperatorAreNotFoldedBeforeEvaluation()
    {
        _session.Type("p+p");

        Assert.That(_session.Value(), Is.EqualTo(6.283185307179586));
    }

    [Test]
    public void NaturalLogarithmOfTheSquaredEConstantIsTwo()
    {
        _session.Type("ln(E^2)");

        Assert.That(_session.Value(), Is.EqualTo(2));
    }

    [Test]
    public void ModuloKeepsTheSignOfTheLeftOperand()
    {
        _session.Type("0-7%3");

        Assert.That(_session.Value(), Is.EqualTo(-1));
    }

    [Test]
    public void ModuloOfZeroByANonZeroOperandIsZero()
    {
        _session.Type("0%5");

        Assert.That(_session.Value(), Is.Zero);
    }

    [Test]
    public void ModuloOfAValueSmallerThanTheDivisorIsTheValueItself()
    {
        _session.Type("3%7");

        Assert.That(_session.Value(), Is.EqualTo(3));
    }

    [Test]
    public void ModuloByAFractionalDivisorIsEvaluated()
    {
        _session.Type("5%2.5");

        Assert.That(_session.Value(), Is.Zero);
    }

    [Test]
    public void ExponentiationBeyondDoubleRangeIsRejected()
    {
        _session.Type("9^999");

        Assert.That(_session.Error(), Is.TypeOf<NumberOverflow>());
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
    public void GroupingEndFollowedByAnOperandStillCannotCollapse()
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
        var expression = new Expression([], AngleMode.RAD);

        var error = default(Error);

        ExpressionEvaluatingExtensions.EvaluateAndRound(expression)
            .Switch(_ => { }, failure => error = failure);

        Assert.That(error, Is.TypeOf<ExpressionNotEvaluated>());
    }

    [Test]
    public void SingleOperandIsEvaluatedEvenWhenItIsFifteenDigitsLong()
    {
        _session.Type("123456789012345*1");

        Assert.That(_session.Value(), Is.EqualTo(123456789012345));
    }

    [Test]
    public void DigitsBeyondTheLimitAreNotAccepted()
    {
        _session.Type("1234567890123456");

        Assert.That(_session.Error(), Is.TypeOf<EmptyExpression>());
    }

    [Test]
    public void TheDigitLimitCountsTheDecimalSeparator()
    {
        _session.Type("1.23456789012345*1");

        Assert.That(_session.RenderedText(), Is.EqualTo("1.2345678901234×1"));
    }

    [Test]
    public void TwoOperandsMayEachUseTheWholeDigitLimit()
    {
        _session.Type("1234567890+1234567890");

        Assert.That(_session.Value(), Is.EqualTo(2469135780));
    }

    [Test]
    public void FloatingPointNoiseIsRoundedAway()
    {
        _session.Type("0.1+0.2");

        Assert.That(_session.Value(), Is.EqualTo(0.3));
    }

    [Test]
    public void SignificantDigitsAreNotRoundedAway()
    {
        _session.Type("1/3");

        Assert.That(_session.Value(), Is.EqualTo(0.3333333333333333));
    }

    [Test]
    public void AccumulatedNoiseIsRoundedAway()
    {
        _session.Type("0.1+0.2+0.3");

        Assert.That(_session.Value(), Is.EqualTo(0.6));
    }

    [Test]
    public void SineOfPiOverSixIsAHalf()
    {
        _session.Type("sin(p/6)");

        Assert.That(_session.Value(), Is.EqualTo(0.5));
    }

    [Test]
    public void ExactZeroStaysZero()
    {
        _session.Type("0*5");

        Assert.That(_session.Value(), Is.Zero);
    }

    [Test]
    public void SubtractingZeroFromZeroStaysZero()
    {
        _session.Type("0-0");

        Assert.That(_session.Value(), Is.Zero);
    }

    [Test]
    public void LargeExactProductsAreNotDisturbed()
    {
        _session.Type("1000000*1000000");

        Assert.That(_session.Value(), Is.EqualTo(1000000000000));
    }

    [Test]
    public void SmallNegativeResultsKeepTheirSign()
    {
        _session.Type("0-0.0000001");

        Assert.That(_session.Value(), Is.EqualTo(-1E-07));
    }

    [TestCase('*', 24)]
    [TestCase('/', 1.5)]
    [TestCase('%', 2)]
    public void EveryMultiplicativeOperatorIsEvaluatedBetweenOperands(char typed, double expected)
    {
        _session.Type($"6{typed}4");

        Assert.That(_session.Value(), Is.EqualTo(expected));
    }

    [Test]
    public void MultiplicationByZeroIsEvaluatedToZero()
    {
        _session.Type("6*0");

        Assert.That(_session.Value(), Is.Zero);
    }

    [Test]
    public void MultiplicationByZeroInAChainStaysZero()
    {
        _session.Type("6*0*2");

        Assert.That(_session.Value(), Is.Zero);
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

        Assert.That(_session.Value(), Is.EqualTo(6));
    }
}
