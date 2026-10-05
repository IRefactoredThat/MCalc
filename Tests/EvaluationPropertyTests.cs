using Essentials.Calculator;
using Essentials.ErrorType;
using Essentials.ResultType;

namespace Tests;

[TestFixture]
public class EvaluationPropertyTests
{
    private const int SeedCount = 60;
    private const int StepsPerSeed = 14;

    private static readonly IToken[] OperatorPool =
    [
        PlusToken.Instance,
        MinusToken.Instance,
        TimesToken.Instance,
        DivToken.Instance,
        PercentToken.Instance,
        CaretToken.Instance,
        ExclamationMarkToken.Instance,
        OpenBracketToken.Instance,
        ClosedBracketToken.Instance,
        SinToken.Instance,
        CosToken.Instance,
        TanToken.Instance,
        SqrtToken.Instance,
        CbrtToken.Instance,
        LogToken.Instance,
        LnToken.Instance,
        PiToken.Instance,
        EToken.Instance
    ];

    private static readonly IToken[] DigitPool =
    [
        NumberToken.Zero,
        NumberToken.One,
        NumberToken.Two,
        NumberToken.Three,
        NumberToken.Four,
        NumberToken.Five,
        NumberToken.Six,
        NumberToken.Seven,
        NumberToken.Eight,
        NumberToken.Nine
    ];

    private static List<IToken> GenerateWellFormed(Random random)
    {
        var tokens = new List<IToken>();

        void Operand(int depth)
        {
            if (depth > 0 && random.Next(4) == 0)
            {
                tokens.Add(OpenBracketToken.Instance);
                Expression(depth - 1);
                tokens.Add(ClosedBracketToken.Instance);
                return;
            }

            if (depth > 0 && random.Next(3) == 0)
            {
                tokens.Add(LeftSideUnary(random));
                Operand(depth - 1);
                return;
            }

            if (random.Next(6) == 0)
            {
                tokens.Add(random.Next(2) == 0 ? PiToken.Instance : EToken.Instance);
                return;
            }

            tokens.Add(Number(random));
        }

        void Expression(int depth)
        {
            Operand(depth);

            for (var step = random.Next(1, 5); step > 0; step--)
            {
                if (random.Next(5) == 0)
                {
                    tokens.Add(ExclamationMarkToken.Instance);
                    continue;
                }

                tokens.Add(BinaryOperator(random));
                Operand(depth);
            }
        }

        Expression(random.Next(3));

        return tokens;
    }

    private static IToken Number(Random random) =>
        random.Next(3) == 0
            ? new NumberToken($"{random.Next(1, 10)}.{random.Next(100, 999)}",
                DecimalSeparator.Period)
            : DigitPool[random.Next(DigitPool.Length)];

    private static IToken BinaryOperator(Random random) => random.Next(6) switch
    {
        0 => PlusToken.Instance,
        1 => MinusToken.Instance,
        2 => TimesToken.Instance,
        3 => DivToken.Instance,
        4 => PercentToken.Instance,
        _ => CaretToken.Instance
    };

    private static IToken LeftSideUnary(Random random) => random.Next(6) switch
    {
        0 => SqrtToken.Instance,
        1 => CbrtToken.Instance,
        2 => LogToken.Instance,
        3 => LnToken.Instance,
        4 => SinToken.Instance,
        _ => CosToken.Instance
    };

    private static IEnumerable<TestCaseData> Seeds()
    {
        for (var seed = 0; seed < SeedCount; seed++)
        {
            yield return new TestCaseData(seed).SetName($"{{m}}(Seed:{seed})");
        }
    }

    private static void PlayAll(IReadOnlyList<IToken> tokens, TokenSession session)
    {
        foreach (var token in tokens)
        {
            session.Add(token);
        }
    }

    private static List<IToken> GenerateArbitrary(int seed)
    {
        var random = new Random(seed);
        var tokens = new List<IToken>();

        for (var step = 0; step < StepsPerSeed; step++)
        {
            tokens.Add(random.Next(2) == 0
                ? DigitPool[random.Next(DigitPool.Length)]
                : OperatorPool[random.Next(OperatorPool.Length)]);
        }

        return tokens;
    }

    [TestCaseSource(nameof(Seeds))]
    public void RandomKeystrokesNeverMakeTheEvaluatorThrow(int seed)
    {
        var session = new TokenSession();

        PlayAll(GenerateArbitrary(seed), session);

        Assert.DoesNotThrow(() => session.Evaluate());
    }

    [TestCaseSource(nameof(Seeds))]
    public void RandomKeystrokesOnlyEverFailWithAnError(int seed)
    {
        var session = new TokenSession();

        PlayAll(GenerateArbitrary(seed), session);

        session.Evaluate()
            .Switch(_ => { }, error => Assert.That(error, Is.InstanceOf<Error>()));
    }

    [TestCaseSource(nameof(Seeds))]
    public void EvaluatingTheSameTokensTwiceGivesTheSameAnswer(int seed)
    {
        var random = new Random(seed);
        var tokens = GenerateWellFormed(random);

        var first = new TokenSession();
        var second = new TokenSession();

        PlayAll(tokens, first);
        PlayAll(tokens, second);

        double? firstValue = null;
        double? secondValue = null;
        var firstFailed = false;
        var secondFailed = false;

        first.Evaluate().Switch(value => firstValue = value, _ => firstFailed = true);
        second.Evaluate().Switch(value => secondValue = value, _ => secondFailed = true);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(firstFailed, Is.EqualTo(secondFailed));
            Assert.That(firstValue, Is.EqualTo(secondValue));
        }
    }

    [TestCaseSource(nameof(Seeds))]
    public void WellFormedExpressionsAreNeverRejectedAsAnInvalidStructure(int seed)
    {
        var random = new Random(seed);
        var session = new TokenSession();

        PlayAll(GenerateWellFormed(random), session);

        session.Evaluate().Switch(_ => { }, error => Assert.That(error,
            Is.Not.TypeOf<EmptyExpression>().And.Not.TypeOf<InvalidExpressionEnd>().And
            .Not.TypeOf<InvalidBinaryOperatorUse>().And.Not.TypeOf<InvalidUnaryOperatorUse>()
            .And.Not.TypeOf<InvalidGroupingEndOperatorUse>().And
            .Not.TypeOf<ExpressionContainsEmptyGrouping>().And
            .Not.TypeOf<InvalidRightSideUnaryOperatorUse>().And
            .Not.TypeOf<ExpressionNotEvaluated>().And.Not.TypeOf<NumberFormatError>()));
    }

    [TestCaseSource(nameof(Seeds))]
    public void EvaluationNeverReportsANonFiniteValue(int seed)
    {
        var random = new Random(seed);
        var session = new TokenSession();

        PlayAll(GenerateWellFormed(random), session);

        session.Evaluate().Switch(
            value => Assert.That(double.IsFinite(value), Is.True),
            _ => { });
    }

    [Test]
    public void AnEmptySessionAlwaysReportsNoExpression()
    {
        var session = new TokenSession();

        Assert.That(session.Error(), Is.TypeOf<EmptyExpression>());
    }

    [Test]
    public void ASingleDigitIsNeverAnExpression()
    {
        for (var digit = 0; digit < 10; digit++)
        {
            var session = new TokenSession();
            session.TypeNumber(digit.ToString());

            Assert.That(session.Error(), Is.TypeOf<EmptyExpression>());
        }
    }

    [Test]
    public void ASingleConstantIsAnExpression()
    {
        foreach (var constant in new IToken[] { PiToken.Instance, EToken.Instance })
        {
            var session = new TokenSession();
            session.Add(constant);

            Assert.That(double.IsFinite(session.Value()), Is.True);
        }
    }

    [Test]
    public void GroupingCountAlwaysEndsUpBalancedForGeneratedInput()
    {
        for (var seed = 0; seed < SeedCount; seed++)
        {
            var tokens = GenerateWellFormed(new Random(seed));

            var opens = tokens.Count(token => token is OpenBracketToken);
            var closes = tokens.Count(token => token is ClosedBracketToken);

            Assert.That(opens, Is.EqualTo(closes));
        }
    }
}
