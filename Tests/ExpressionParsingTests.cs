using System.Globalization;
using Calculator.Operators;
using Essentials.Calculator;
using Essentials.ErrorType;
using Essentials.Numerics;
using Essentials.ResultType;

namespace Tests;

[TestFixture]
public class ExpressionParsingTests
{
    private TokenSession _session;

    [SetUp]
    public void Setup() => _session = new TokenSession();

    private static readonly (OperatorToken Token, Type Part)[] BinaryOperators =
    [
        (PlusToken.Instance, typeof(AdditionOperator)),
        (MinusToken.Instance, typeof(SubtractionOperator)),
        (TimesToken.Instance, typeof(MultiplicationOperator)),
        (DivToken.Instance, typeof(DivisionOperator)),
        (CaretToken.Instance, typeof(ExponentiationOperator)),
        (PercentToken.Instance, typeof(ModuloOperator))
    ];

    private static readonly (OperatorToken Token, Type Part)[] LeftSideUnaryOperators =
    [
        (SinToken.Instance, typeof(SineOperator)),
        (CosToken.Instance, typeof(CosineOperator)),
        (TanToken.Instance, typeof(TangentOperator)),
        (CotToken.Instance, typeof(CotangentOperator)),
        (SecToken.Instance, typeof(SecantOperator)),
        (CscToken.Instance, typeof(CosecantOperator)),
        (AsinToken.Instance, typeof(ArcsineOperator)),
        (AcosToken.Instance, typeof(ArccosineOperator)),
        (AtanToken.Instance, typeof(ArctangentOperator)),
        (AcotToken.Instance, typeof(ArccotangentOperator)),
        (AsecToken.Instance, typeof(ArcsecantOperator)),
        (AcscToken.Instance, typeof(ArccosecantOperator)),
        (SinhToken.Instance, typeof(HyperbolicSineOperator)),
        (CoshToken.Instance, typeof(HyperbolicCosineOperator)),
        (TanhToken.Instance, typeof(HyperbolicTangentOperator)),
        (CothToken.Instance, typeof(HyperbolicCotangentOperator)),
        (SechToken.Instance, typeof(HyperbolicSecantOperator)),
        (CschToken.Instance, typeof(HyperbolicCosecantOperator)),
        (AsinhToken.Instance, typeof(HyperbolicArcsineOperator)),
        (AcoshToken.Instance, typeof(HyperbolicArccosineOperator)),
        (AtanhToken.Instance, typeof(HyperbolicArctangentOperator)),
        (AcothToken.Instance, typeof(HyperbolicArccotangentOperator)),
        (AsechToken.Instance, typeof(HyperbolicArcsecantOperator)),
        (AcschToken.Instance, typeof(HyperbolicArccosecantOperator)),
        (SqrtToken.Instance, typeof(SquareRootOperator)),
        (CbrtToken.Instance, typeof(CubeRootOperator)),
        (LogToken.Instance, typeof(LogarithmOperator)),
        (LnToken.Instance, typeof(NaturalLogarithmOperator))
    ];

    private static IEnumerable<TestCaseData> BinaryOperatorCases() =>
        BinaryOperators.Select(entry => new TestCaseData(entry.Token, entry.Part)
            .SetName($"{{m}}(Operator:{entry.Token.Value})"));

    private static IEnumerable<TestCaseData> LeftSideUnaryOperatorCases() =>
        LeftSideUnaryOperators.Select(entry => new TestCaseData(entry.Token, entry.Part)
            .SetName($"{{m}}(Operator:{entry.Token.Value})"));

    private static IEnumerable<TestCaseData> MultiplicativeOperators() =>
        BinaryOperators
            .Where(entry => entry.Token is not (PlusToken or MinusToken))
            .Select(entry => new TestCaseData(entry.Token)
                .SetName($"{{m}}(Operator:{entry.Token.Value})"));

    private NumberToken NumberPart(string value) => _session.Number(value);

    private static Number Double(double value) =>
        Number.Parse(value.ToString("R", CultureInfo.InvariantCulture),
            CultureInfo.InvariantCulture);

    private void TypeNumber(string digits) => _session.TypeNumber(digits);

    private void Add(IToken token) => _session.Add(token);

    private void Paste(IReadOnlyList<IToken> sequence) => _session.Paste(sequence);

    private IReadOnlyList<IExpressionPart> Parts()
    {
        IReadOnlyList<IExpressionPart>? parts = null;
        string? failure = null;

        Parse().Switch(expression => parts = expression.Parts,
            error => failure = error.GetType().Name);

        Assert.That(failure, Is.Null, "Expected parts");

        return parts!;
    }

    private Error Error()
    {
        Error? error = null;
        var succeeded = false;

        Parse().Switch(_ => succeeded = true, parsedError => error = parsedError);

        Assert.That(succeeded, Is.False, "Expected an error");

        return error!;
    }

    private Result<Expression> Parse() => _session.Parse();

    private static IExpressionPart Part(Type partType) =>
        (IExpressionPart)Activator.CreateInstance(partType)!;

    [Test]
    public void EmptyInputIsNotAnExpression() =>
        Assert.That(Error(), Is.TypeOf<EmptyExpression>());

    [Test]
    public void SingleNumberIsNotAnExpression()
    {
        TypeNumber("5");

        Assert.That(Error(), Is.TypeOf<EmptyExpression>());
    }

    [Test]
    public void SingleDecimalNumberIsNotAnExpression()
    {
        TypeNumber("15");
        Add(NumberPart("."));
        TypeNumber("5");

        Assert.That(Error(), Is.TypeOf<EmptyExpression>());
    }

    [Test]
    public void SingleConstantIsAnExpression()
    {
        Add(PiToken.Instance);

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>([Number.Pi]));
    }

    [Test]
    public void SingleECoIsAnExpression()
    {
        Add(EToken.Instance);

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>([Number.E]));
    }

    [Test]
    public void SingleFactorialIsAnExpression()
    {
        Add(ExclamationMarkToken.Instance);

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>([new FactorialOperator()]));
    }

    [Test]
    public void SingleClosedBracketIsAnExpression()
    {
        Add(ClosedBracketToken.Instance);

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>([new GroupingEndOperator()]));
    }

    [TestCase(DecimalSeparator.Period, "1.5", 1.5)]
    [TestCase(DecimalSeparator.Comma, "1,5", 1.5)]
    [TestCase(DecimalSeparator.Period, "1234.5", 1234.5)]
    [TestCase(DecimalSeparator.Comma, "1234,5", 1234.5)]
    public void DecimalSeparatorIsTakenFromTheSuppliedFormat(
        DecimalSeparator decimalSeparator, string value, double expected)
    {
        _session.DecimalSeparator = decimalSeparator;
        _session.Info.NumberDecimalSeparator = ((char)decimalSeparator).ToString();
        Paste([NumberPart(value)]);
        Add(PlusToken.Instance);
        TypeNumber("2");

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>(
            [Double(expected), new AdditionOperator(), Number.CreateUnsafe(2)]));
    }

    [Test]
    public void NumberAndOperatorBecomeTheirOwnParts()
    {
        TypeNumber("1");
        Add(PlusToken.Instance);
        TypeNumber("2");

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>(
            [Number.CreateUnsafe(1), new AdditionOperator(), Number.CreateUnsafe(2)]));
    }

    [TestCaseSource(nameof(BinaryOperatorCases))]
    public void BinaryOperatorBetweenNumbersBecomesItsOwnPart(
        OperatorToken op, Type part)
    {
        TypeNumber("12");
        Add(op);
        TypeNumber("34");

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>(
            [Number.CreateUnsafe(12), Part(part), Number.CreateUnsafe(34)]));
    }

    [Test]
    public void DecimalNumberBecomesNumberPartWithParsedValue()
    {
        TypeNumber("1");
        Add(NumberPart("."));
        TypeNumber("5");
        Add(PlusToken.Instance);
        TypeNumber("2");

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>(
            [Double(1.5), new AdditionOperator(), Number.CreateUnsafe(2)]));
    }

    [Test]
    public void ExponentNotationIsParsedToItsValue()
    {
        Paste([NumberPart("1E+5")]);
        Add(PlusToken.Instance);
        TypeNumber("1");

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>(
            [Number.CreateUnsafe(100000), new AdditionOperator(), Number.CreateUnsafe(1)]));
    }

    [Test]
    public void LeadingZerosAreParsedToTheirNumericValue()
    {
        TypeNumber("00012");
        Add(PlusToken.Instance);
        TypeNumber("1");

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>(
            [Number.CreateUnsafe(12), new AdditionOperator(), Number.CreateUnsafe(1)]));
    }

    [Test]
    public void ExponentOverflowIsRejectedAsUnparsableNumber()
    {
        Paste([NumberPart($"1E+{Number.MaxWholeDigits + 1}")]);
        Add(PlusToken.Instance);
        TypeNumber("1");

        Assert.That(Error(), Is.TypeOf<NumberFormatError>());
    }

    [Test]
    public void NumberTooLongForDoubleIsRejectedAsUnparsableNumber()
    {
        Paste([NumberPart("1.2.3")]);
        Add(PlusToken.Instance);
        TypeNumber("1");

        Assert.That(Error(), Is.TypeOf<NumberFormatError>());
    }

    [Test]
    public void DecimalSeparatorWithoutDigitsIsRejectedAsUnparsableNumber()
    {
        Add(NumberPart("."));
        Add(PlusToken.Instance);
        TypeNumber("1");

        Assert.That(Error(), Is.TypeOf<NumberFormatError>());
    }

    [Test]
    public void TrailingDecimalSeparatorIsAcceptedAsWholeNumber()
    {
        TypeNumber("5");
        Add(NumberPart("."));
        Add(PlusToken.Instance);
        TypeNumber("1");

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>(
            [Number.CreateUnsafe(5), new AdditionOperator(), Number.CreateUnsafe(1)]));
    }

    [TestCaseSource(nameof(BinaryOperatorCases))]
    public void ExpressionEndingWithBinaryOperatorIsRejected(OperatorToken op, Type part)
    {
        TypeNumber("1");
        Add(op);

        Assert.That(Error(), Is.TypeOf<InvalidExpressionEnd>());
    }

    [Test]
    public void SingleAdditiveOperatorIsRejectedAsExpressionEnd()
    {
        Add(PlusToken.Instance);

        Assert.That(Error(), Is.TypeOf<InvalidExpressionEnd>());
    }

    [Test]
    public void SingleNegationOperatorIsRejectedAsExpressionEnd()
    {
        Add(MinusToken.Instance);

        Assert.That(Error(), Is.TypeOf<InvalidExpressionEnd>());
    }

    [TestCaseSource(nameof(MultiplicativeOperators))]
    public void SingleMultiplicativeOperatorIsRejectedAsInvalidUse(OperatorToken op)
    {
        Add(op);

        Assert.That(Error(), Is.EqualTo(new InvalidBinaryOperatorUse(op)));
    }

    [Test]
    public void SingleOpenBracketIsRejectedAsExpressionEnd()
    {
        Add(OpenBracketToken.Instance);

        Assert.That(Error(), Is.TypeOf<InvalidExpressionEnd>());
    }

    [TestCaseSource(nameof(LeftSideUnaryOperatorCases))]
    public void SingleLeftSideUnaryOperatorIsRejectedAsExpressionEnd(
        OperatorToken op, Type part)
    {
        Add(op);

        Assert.That(Error(), Is.TypeOf<InvalidExpressionEnd>());
    }

    [TestCaseSource(nameof(LeftSideUnaryOperatorCases))]
    public void LeftSideUnaryOperatorBeforeNumberBecomesItsOwnPart(
        OperatorToken op, Type part)
    {
        Add(op);
        TypeNumber("2");

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>(
            [Part(part), Number.CreateUnsafe(2)]));
    }

    [TestCaseSource(nameof(LeftSideUnaryOperatorCases))]
    public void LeftSideUnaryOperatorAfterNumberBecomesExplicitMultiplication(
        OperatorToken op, Type part)
    {
        TypeNumber("2");
        Add(op);
        TypeNumber("3");

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>(
            [Number.CreateUnsafe(2), new MultiplicationOperator(), Part(part), Number.CreateUnsafe(3)]));
    }

    [TestCaseSource(nameof(MultiplicativeOperators))]
    public void MultiplicativeOperatorAfterLeftSideUnaryIsRejectedAsMisuseOfTheUnary(
        OperatorToken op)
    {
        Add(SinToken.Instance);
        Add(op);

        Assert.That(Error(), Is.EqualTo(new InvalidUnaryOperatorUse(SinToken.Instance)));
    }

    [Test]
    public void AdditiveOperatorAfterLeftSideUnaryIsRejectedAsExpressionEnd()
    {
        Add(SinToken.Instance);
        Add(PlusToken.Instance);

        Assert.That(Error(), Is.TypeOf<InvalidExpressionEnd>());
    }

    [TestCaseSource(nameof(MultiplicativeOperators))]
    public void MultiplicativeOperatorAfterBinaryOperatorIsRejectedAsInvalidUse(OperatorToken op)
    {
        TypeNumber("1");
        Add(TimesToken.Instance);
        Add(op);

        Assert.That(Error(), Is.EqualTo(new InvalidBinaryOperatorUse(op)));
    }

    [Test]
    public void GroupingOperatorsAreKeptAsTheirOwnParts()
    {
        TypeNumber("1");
        Add(PlusToken.Instance);
        Add(OpenBracketToken.Instance);
        TypeNumber("2");
        Add(PlusToken.Instance);
        TypeNumber("3");
        Add(ClosedBracketToken.Instance);
        Add(TimesToken.Instance);
        TypeNumber("4");

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>(
            [Number.CreateUnsafe(1), new AdditionOperator(), new GroupingStartOperator(), Number.CreateUnsafe(2),
                new AdditionOperator(), Number.CreateUnsafe(3), new GroupingEndOperator(),
                new MultiplicationOperator(), Number.CreateUnsafe(4)]));
    }

    [Test]
    public void NestedGroupingIsKeptAsSeparateParts()
    {
        Add(OpenBracketToken.Instance);
        Add(OpenBracketToken.Instance);
        TypeNumber("1");
        Add(ClosedBracketToken.Instance);
        Add(ClosedBracketToken.Instance);

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>(
            [new GroupingStartOperator(), new GroupingStartOperator(), Number.CreateUnsafe(1),
                new GroupingEndOperator(), new GroupingEndOperator()]));
    }

    [Test]
    public void OpenBracketAfterNumberBecomesExplicitMultiplication()
    {
        TypeNumber("2");
        Add(OpenBracketToken.Instance);
        TypeNumber("3");
        Add(ClosedBracketToken.Instance);

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>(
            [Number.CreateUnsafe(2), new MultiplicationOperator(), new GroupingStartOperator(), Number.CreateUnsafe(3),
                new GroupingEndOperator()]));
    }

    [Test]
    public void NumberAfterClosedBracketBecomesExplicitMultiplication()
    {
        TypeNumber("2");
        Add(ClosedBracketToken.Instance);
        TypeNumber("3");

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>(
            [Number.CreateUnsafe(2), new GroupingEndOperator(), new MultiplicationOperator(), Number.CreateUnsafe(3)]));
    }

    [Test]
    public void ClosedBracketAfterBinaryOperatorIsRejected()
    {
        TypeNumber("1");
        Add(PlusToken.Instance);
        Add(ClosedBracketToken.Instance);

        Assert.That(Error(), Is.TypeOf<InvalidGroupingEndOperatorUse>());
    }

    [Test]
    public void EmptyGroupingIsRejected()
    {
        TypeNumber("1");
        Add(PlusToken.Instance);
        Add(OpenBracketToken.Instance);
        Add(ClosedBracketToken.Instance);

        Assert.That(Error(), Is.TypeOf<ExpressionContainsEmptyGrouping>());
    }

    [Test]
    public void NestedEmptyGroupingIsRejected()
    {
        Add(OpenBracketToken.Instance);
        Add(OpenBracketToken.Instance);
        Add(ClosedBracketToken.Instance);

        Assert.That(Error(), Is.TypeOf<ExpressionContainsEmptyGrouping>());
    }

    [Test]
    public void ClosedBracketAfterLeftSideUnaryOperatorIsRejected()
    {
        Add(SinToken.Instance);
        Add(ClosedBracketToken.Instance);

        Assert.That(Error(), Is.TypeOf<InvalidGroupingEndOperatorUse>());
    }

    [Test]
    public void ClosedBracketAfterLeftSideUnaryOperatorInExpressionIsRejected()
    {
        TypeNumber("2");
        Add(PlusToken.Instance);
        Add(SinToken.Instance);
        Add(ClosedBracketToken.Instance);

        Assert.That(Error(), Is.TypeOf<InvalidGroupingEndOperatorUse>());
    }

    [Test]
    public void LeftSideUnaryOperatorAfterClosedBracketIsRejectedAsExpressionEnd()
    {
        Add(OpenBracketToken.Instance);
        TypeNumber("2");
        Add(ClosedBracketToken.Instance);
        Add(SinToken.Instance);

        Assert.That(Error(), Is.TypeOf<InvalidExpressionEnd>());
    }

    [Test]
    public void GroupingAfterImplicitlyMultipliedNumberBecomesExplicitMultiplication()
    {
        Add(ClosedBracketToken.Instance);
        TypeNumber("2");
        Add(OpenBracketToken.Instance);
        TypeNumber("3");
        Add(ClosedBracketToken.Instance);

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>(
            [new GroupingEndOperator(), new MultiplicationOperator(), Number.CreateUnsafe(2),
                new MultiplicationOperator(), new GroupingStartOperator(), Number.CreateUnsafe(3),
                new GroupingEndOperator()]));
    }

    [Test]
    public void GroupingAfterImplicitlyMultipliedUnaryOperatorBecomesExplicitMultiplication()
    {
        TypeNumber("2");
        Add(SinToken.Instance);
        Add(OpenBracketToken.Instance);
        TypeNumber("3");
        Add(ClosedBracketToken.Instance);

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>(
            [Number.CreateUnsafe(2), new MultiplicationOperator(), new SineOperator(),
                new GroupingStartOperator(), Number.CreateUnsafe(3), new GroupingEndOperator()]));
    }

    [Test]
    public void UnclosedGroupingIsAcceptedUntilEvaluation()
    {
        TypeNumber("1");
        Add(PlusToken.Instance);
        Add(OpenBracketToken.Instance);
        TypeNumber("2");

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>(
            [Number.CreateUnsafe(1), new AdditionOperator(), new GroupingStartOperator(), Number.CreateUnsafe(2)]));
    }

    [Test]
    public void UnopenedGroupingIsAcceptedUntilEvaluation()
    {
        TypeNumber("2");
        Add(TimesToken.Instance);
        TypeNumber("3");
        Add(ClosedBracketToken.Instance);

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>(
            [Number.CreateUnsafe(2), new MultiplicationOperator(), Number.CreateUnsafe(3),
                new GroupingEndOperator()]));
    }

    [Test]
    public void FactorialAfterNumberBecomesItsOwnPart()
    {
        TypeNumber("2");
        Add(ExclamationMarkToken.Instance);

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>(
            [Number.CreateUnsafe(2), new FactorialOperator()]));
    }

    [Test]
    public void RepeatedFactorialStacksItsOwnParts()
    {
        TypeNumber("2");
        Add(ExclamationMarkToken.Instance);
        Add(ExclamationMarkToken.Instance);

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>(
            [Number.CreateUnsafe(2), new FactorialOperator(), new FactorialOperator()]));
    }

    [Test]
    public void FactorialAfterClosedBracketIsAcceptedAsExpressionEnd()
    {
        Add(OpenBracketToken.Instance);
        TypeNumber("2");
        Add(ClosedBracketToken.Instance);
        Add(ExclamationMarkToken.Instance);

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>(
            [new GroupingStartOperator(), Number.CreateUnsafe(2), new GroupingEndOperator(),
                new FactorialOperator()]));
    }

    [Test]
    public void FactorialBeforeGroupingBecomesExplicitMultiplication()
    {
        TypeNumber("2");
        Add(ExclamationMarkToken.Instance);
        Add(OpenBracketToken.Instance);
        TypeNumber("3");
        Add(ClosedBracketToken.Instance);

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>(
            [Number.CreateUnsafe(2), new FactorialOperator(), new MultiplicationOperator(),
                new GroupingStartOperator(), Number.CreateUnsafe(3), new GroupingEndOperator()]));
    }

    [Test]
    public void FactorialBeforeMultiplicativeOperatorBecomesExplicitMultiplication()
    {
        Add(ExclamationMarkToken.Instance);
        Add(TimesToken.Instance);
        TypeNumber("1");

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>(
            [new FactorialOperator(), new MultiplicationOperator(), Number.CreateUnsafe(1)]));
    }

    [Test]
    public void MultiplicativeOperatorBeforeFactorialIsAcceptedAsExpressionEnd()
    {
        TypeNumber("2");
        Add(TimesToken.Instance);
        Add(ExclamationMarkToken.Instance);

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>(
            [Number.CreateUnsafe(2), new MultiplicationOperator(), new FactorialOperator()]));
    }

    [Test]
    public void NumberAfterFactorialIsKeptWithoutMultiplication()
    {
        TypeNumber("2");
        Add(ExclamationMarkToken.Instance);
        TypeNumber("3");

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>(
            [Number.CreateUnsafe(2), new FactorialOperator(), Number.CreateUnsafe(3)]));
    }

    [Test]
    public void ConstantAfterNumberBecomesExplicitMultiplication()
    {
        TypeNumber("2");
        Add(PiToken.Instance);

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>(
            [Number.CreateUnsafe(2), new MultiplicationOperator(), Number.Pi]));
    }

    [Test]
    public void ConstantBeforeNumberBecomesExplicitMultiplication()
    {
        Add(PiToken.Instance);
        TypeNumber("2");

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>(
            [Number.Pi, new MultiplicationOperator(), Number.CreateUnsafe(2)]));
    }

    [Test]
    public void ConsecutiveConstantsBecomeExplicitMultiplication()
    {
        Add(PiToken.Instance);
        Add(PiToken.Instance);

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>(
            [Number.Pi, new MultiplicationOperator(), Number.Pi]));
    }

    [Test]
    public void NumberChainAroundConstantBecomesExplicitMultiplication()
    {
        TypeNumber("2");
        Add(PiToken.Instance);
        TypeNumber("3");

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>(
            [Number.CreateUnsafe(2), new MultiplicationOperator(), Number.Pi,
                new MultiplicationOperator(), Number.CreateUnsafe(3)]));
    }

    [Test]
    public void ConstantChainBecomesExplicitMultiplication()
    {
        TypeNumber("3");
        Add(EToken.Instance);
        Add(EToken.Instance);
        Add(PlusToken.Instance);
        TypeNumber("1");

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>(
            [Number.CreateUnsafe(3), new MultiplicationOperator(), Number.E,
                new MultiplicationOperator(), Number.E,
                new AdditionOperator(), Number.CreateUnsafe(1)]));
    }

    [Test]
    public void ConstantsSeparatedByOperatorAreNotMultiplied()
    {
        Add(PiToken.Instance);
        Add(PlusToken.Instance);
        Add(PiToken.Instance);

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>(
            [Number.Pi, new AdditionOperator(), Number.Pi]));
    }

    [Test]
    public void ConstantAfterExplicitMultiplicationIsNotFolded()
    {
        TypeNumber("2");
        Add(TimesToken.Instance);
        Add(PiToken.Instance);

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>(
            [Number.CreateUnsafe(2), new MultiplicationOperator(), Number.Pi]));
    }

    [Test]
    public void LeftSideUnaryOperatorBetweenOperatorsIsKeptWithoutMultiplication()
    {
        TypeNumber("2");
        Add(MinusToken.Instance);
        Add(SinToken.Instance);
        TypeNumber("1");

        Assert.That(Parts(), Is.EqualTo<IExpressionPart[]>(
            [Number.CreateUnsafe(2), new SubtractionOperator(), new SineOperator(), Number.CreateUnsafe(1)]));
    }

    [Test]
    public void ParseAttachesGivenAngleMode()
    {
        _session.Mode = AngleMode.DEG;
        TypeNumber("1");
        Add(PlusToken.Instance);
        TypeNumber("2");

        Expression? expression = null;
        Parse().Switch(parsed => expression = parsed, _ => Assert.Fail("Expected parts."));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(expression!.Mode, Is.EqualTo(AngleMode.DEG));
            Assert.That(expression.Parts, Is.EqualTo<IExpressionPart[]>(
                [Number.CreateUnsafe(1), new AdditionOperator(), Number.CreateUnsafe(2)]));
        }
    }
}
