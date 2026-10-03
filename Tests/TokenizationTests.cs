using Calculator.ExpressionParsing;
using Essentials.Calculator;

namespace Tests;

[TestFixture]
public class TokenizationTests
{
    private Tokens _tokens;
    private DecimalSeparator _decimalSeparator;
    private ThousandSeparator _thousandSeparator;

    [SetUp]
    public void Setup()
    {
        _tokens = new Tokens();
        _decimalSeparator = DecimalSeparator.Period;
        _thousandSeparator = ThousandSeparator.Space;
    }

    private static readonly NumberToken[] DigitTokens =
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

    private static readonly OperatorToken[] AllOperatorTokens =
    [
        PiToken.Instance,
        EToken.Instance,
        PlusToken.Instance,
        MinusToken.Instance,
        TimesToken.Instance,
        DivToken.Instance,
        CaretToken.Instance,
        PercentToken.Instance,
        ExclamationMarkToken.Instance,
        OpenBracketToken.Instance,
        ClosedBracketToken.Instance,
        SinToken.Instance,
        CosToken.Instance,
        TanToken.Instance,
        CotToken.Instance,
        SecToken.Instance,
        CscToken.Instance,
        AsinToken.Instance,
        AcosToken.Instance,
        AtanToken.Instance,
        AcotToken.Instance,
        AsecToken.Instance,
        AcscToken.Instance,
        SinhToken.Instance,
        CoshToken.Instance,
        TanhToken.Instance,
        CothToken.Instance,
        SechToken.Instance,
        CschToken.Instance,
        AsinhToken.Instance,
        AcoshToken.Instance,
        AtanhToken.Instance,
        AcothToken.Instance,
        AsechToken.Instance,
        AcschToken.Instance,
        SqrtToken.Instance,
        CbrtToken.Instance,
        LogToken.Instance,
        LnToken.Instance
    ];

    private static IEnumerable<TestCaseData> AllOperators()
    {
        return AllOperatorTokens.Select(op => new TestCaseData(op)
            .SetName($"{{m}}(Operator:{op.Value})"));
    }

    private NumberToken Number(string value) =>
        new(value, _decimalSeparator);

    private NumberToken DecimalSeparatorToken() =>
        Number(((char)_decimalSeparator).ToString());

    private void TypeNumber(string digits)
    {
        foreach (var digit in digits)
        {
            Add(DigitTokens[digit - '0']);
        }
    }

    private (string Text, int Position) Add(IToken token)
    {
        var logicalPosition = _tokens.AddToken(token, _decimalSeparator);

        return Render(logicalPosition);
    }

    private (string Text, int Position) Remove()
    {
        var logicalPosition = _tokens.RemoveToken(_decimalSeparator);

        return Render(logicalPosition);
    }

    private (string Text, int Position) Paste(IReadOnlyList<IToken> sequence)
    {
        var logicalPosition = _tokens.AddTokens(sequence, _decimalSeparator);

        return Render(logicalPosition);
    }

    private string CurrentText =>
        _tokens.GetTextInfo(0, _thousandSeparator, _decimalSeparator).Text;

    private (string Text, int Position) Render(int logicalPosition)
    {
        var (text, position) = _tokens.GetTextInfo(logicalPosition, _thousandSeparator,
            _decimalSeparator);

        _tokens.UpdateTokenPosition(position, IsAfterThousandSeparator(text, position));

        return (text, position);
    }

    private void MoveCursorTo(int displayPosition)
    {
        var text = CurrentText;
        _tokens.UpdateTokenPosition(displayPosition, IsAfterThousandSeparator(text, displayPosition));
    }

    private bool IsAfterThousandSeparator(string text, int position) =>
        position > 0 && text[position - 1] == (char)_thousandSeparator;

    [Test]
    public void TokensAreExposedInInsertionOrder()
    {
        TypeNumber("12");
        Add(PlusToken.Instance);
        TypeNumber("34");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(_tokens, Has.Count.EqualTo(3));
            Assert.That(_tokens[0], Is.TypeOf<NumberToken>());
            Assert.That(_tokens[0].Value, Is.EqualTo("12"));
            Assert.That(_tokens[1], Is.SameAs(PlusToken.Instance));
            Assert.That(_tokens[2], Is.TypeOf<NumberToken>());
            Assert.That(_tokens[2].Value, Is.EqualTo("34"));
            Assert.That(_tokens.Select(token => token.Value),
                Is.EqualTo(["12", "+", "34"]));
        }
    }

    [Test]
    public void ClearEmptiesTokenList()
    {
        TypeNumber("12");
        Add(PlusToken.Instance);
        TypeNumber("34");

        _tokens.Clear();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(_tokens, Is.Empty);
            Assert.That(CurrentText, Is.Empty);
        }
    }

    [Test]
    public void TypingDigitsBuildsSingleNumberToken()
    {
        var first = Add(NumberToken.One);
        var second = Add(NumberToken.Two);
        var third = Add(NumberToken.Three);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(first.Text, Is.EqualTo("1"));
            Assert.That(first.Position, Is.EqualTo(1));
            Assert.That(second.Text, Is.EqualTo("12"));
            Assert.That(second.Position, Is.EqualTo(2));
            Assert.That(third.Text, Is.EqualTo("123"));
            Assert.That(third.Position, Is.EqualTo(3));
            Assert.That(_tokens, Has.Count.EqualTo(1));
            Assert.That(_tokens[0].Value, Is.EqualTo("123"));
        }
    }

    [Test]
    public void GroupingCountsBackwardsFromTheEndOfTheIntegerPart()
    {
        TypeNumber("1234");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(CurrentText, Is.EqualTo("1 234"));
            Assert.That(_tokens.DisplayPosition, Is.EqualTo(5));
        }
    }

    [Test]
    public void GroupingKeepsFirstGroupOfOneOrThreeDigits()
    {
        TypeNumber("131231");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(CurrentText, Is.EqualTo("131 231"));
            Assert.That(_tokens.DisplayPosition, Is.EqualTo(7));
            Assert.That(_tokens, Has.Count.EqualTo(1));
        }
    }

    [Test]
    public void TypingInsideNumberInsertsDigitIntoTheSameToken()
    {
        TypeNumber("123456");
        MoveCursorTo(2);

        var (text, position) = Add(NumberToken.Nine);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.EqualTo("1 293 456"));
            Assert.That(position, Is.EqualTo(4));
            Assert.That(_tokens, Has.Count.EqualTo(1));
            Assert.That(_tokens[0].Value, Is.EqualTo("1293456"));
        }
    }

    [Test]
    public void CursorAfterThousandSeparatorSnapsBackBeforeInsertion()
    {
        TypeNumber("123456");
        MoveCursorTo(4);

        Assert.That(_tokens.DisplayPosition, Is.EqualTo(3));

        var (text, position) = Add(NumberToken.Nine);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.EqualTo("1 239 456"));
            Assert.That(position, Is.EqualTo(6));
            Assert.That(_tokens.DisplayPosition, Is.EqualTo(5));
            Assert.That(_tokens[0].Value, Is.EqualTo("1239456"));
        }
    }

    [Test]
    public void TypingAtStartOfNumberExtendsIt()
    {
        TypeNumber("1234");
        MoveCursorTo(0);

        var (text, position) = Add(NumberToken.Nine);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.EqualTo("91 234"));
            Assert.That(position, Is.EqualTo(1));
            Assert.That(_tokens, Has.Count.EqualTo(1));
        }
    }

    [Test]
    public void TypingOperatorInsideNumberSplitsItInTwoNumbers()
    {
        TypeNumber("123456");
        MoveCursorTo(2);
        Add(PlusToken.Instance);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(CurrentText, Is.EqualTo("12+3 456"));
            Assert.That(_tokens, Has.Count.EqualTo(3));
            Assert.That(_tokens[0].Value, Is.EqualTo("12"));
            Assert.That(_tokens[1], Is.SameAs(PlusToken.Instance));
            Assert.That(_tokens[2].Value, Is.EqualTo("3456"));
        }
    }

    [TestCaseSource(nameof(AllOperators))]
    public void OperatorBetweenNumbersIsInsertedWithOperandsOnBothSides(OperatorToken op)
    {
        var beforeOperator = Add(Number("12"));
        var withOperator = Add(op);
        var afterOperator = Add(Number("34"));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(beforeOperator.Position, Is.EqualTo(2));
            Assert.That(withOperator.Text, Is.EqualTo("12" + op.Value));
            Assert.That(withOperator.Position, Is.EqualTo(2 + op.Value.Length));
            Assert.That(afterOperator.Text, Is.EqualTo("12" + op.Value + "34"));
            Assert.That(afterOperator.Position, Is.EqualTo(2 + op.Value.Length + 2));
            Assert.That(_tokens, Has.Count.EqualTo(3));
            Assert.That(_tokens[1], Is.SameAs(op));
        }
    }

    [TestCaseSource(nameof(AllOperators))]
    public void CursorInsideOperatorSnapsToNearestBoundary(OperatorToken op)
    {
        Add(Number("12"));
        Add(op);
        Add(Number("34"));

        for (var displayOffset = 1; displayOffset <= op.Value.Length; displayOffset++)
        {
            MoveCursorTo(2 + displayOffset);

            var snapsBeforeOperator = displayOffset <= op.Value.Length / 2;

            Assert.That(_tokens.DisplayPosition,
                Is.EqualTo(snapsBeforeOperator ? 2 : 2 + op.Value.Length),
                $"cursor {displayOffset} character(s) into '{op.Value}'");
        }
    }

    [Test]
    public void TypingBeforeOperatorMergesIntoPreviousNumber()
    {
        TypeNumber("123");
        Add(PlusToken.Instance);
        MoveCursorTo(3);

        var (text, position) = Add(NumberToken.Four);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.EqualTo("1 234+"));
            Assert.That(position, Is.EqualTo(5));
            Assert.That(_tokens, Has.Count.EqualTo(2));
            Assert.That(_tokens[0].Value, Is.EqualTo("1234"));
        }
    }

    [Test]
    public void TypingBeforeOperatorAtStartOfInputInsertsNewNumberToken()
    {
        Add(PlusToken.Instance);
        MoveCursorTo(0);

        var (text, position) = Add(NumberToken.Four);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.EqualTo("4+"));
            Assert.That(position, Is.EqualTo(1));
            Assert.That(_tokens, Has.Count.EqualTo(2));
            Assert.That(_tokens[0].Value, Is.EqualTo("4"));
        }
    }

    [Test]
    public void OperatorAfterOperatorStartsNegativeOperand()
    {
        TypeNumber("5");
        Add(TimesToken.Instance);

        var afterSign = Add(MinusToken.Instance);
        var afterDigit = Add(NumberToken.Three);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(afterSign.Text, Is.EqualTo("5×-"));
            Assert.That(afterSign.Position, Is.EqualTo(3));
            Assert.That(afterDigit.Text, Is.EqualTo("5×-3"));
            Assert.That(afterDigit.Position, Is.EqualTo(4));
            Assert.That(_tokens.Select(token => token.Value), Is.EqualTo(["5", "×", "-", "3"]));
        }
    }

    [Test]
    public void MultiCharOperatorAfterOperatorStartsNegativeArgument()
    {
        Add(Number("12"));
        Add(SinToken.Instance);

        var afterSign = Add(MinusToken.Instance);
        var afterDigit = Add(NumberToken.Three);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(afterSign.Text, Is.EqualTo("12sin-"));
            Assert.That(afterSign.Position, Is.EqualTo(6));
            Assert.That(afterDigit.Text, Is.EqualTo("12sin-3"));
            Assert.That(afterDigit.Position, Is.EqualTo(7));
            Assert.That(_tokens.Select(token => token.Value), Is.EqualTo(["12", "sin", "-", "3"]));
        }
    }

    [Test]
    public void OperatorTypedOnFirstHalfOfOperatorIsInsertedInFrontOfIt()
    {
        TypeNumber("5");
        Add(TimesToken.Instance);
        MoveCursorTo(1);

        var (text, position) = Add(PlusToken.Instance);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.EqualTo("5+×"));
            Assert.That(position, Is.EqualTo(2));
            Assert.That(_tokens.Select(token => token.Value), Is.EqualTo(["5", "+", "×"]));
        }
    }

    [Test]
    public void DigitTypedBeforeSignStartsNewNumberToken()
    {
        Add(Number("12"));
        Add(SinToken.Instance);
        Add(MinusToken.Instance);
        Add(NumberToken.Three);
        MoveCursorTo(5);

        var (text, position) = Add(NumberToken.Three);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.EqualTo("12sin3-3"));
            Assert.That(position, Is.EqualTo(6));
            Assert.That(_tokens.Select(token => token.Value),
                Is.EqualTo(["12", "sin", "3", "-", "3"]));
        }
    }

    [Test]
    public void DigitTypedInSecondHalfOfOperatorMovesInFrontOfFollowingSign()
    {
        Add(Number("12"));
        Add(SinToken.Instance);
        Add(MinusToken.Instance);
        Add(NumberToken.Three);
        MoveCursorTo(4);

        var (text, position) = Add(NumberToken.Four);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.EqualTo("12sin4-3"));
            Assert.That(position, Is.EqualTo(6));
            Assert.That(_tokens.Select(token => token.Value),
                Is.EqualTo(["12", "sin", "4", "-", "3"]));
        }
    }

    [Test]
    public void DigitTypedInSecondHalfOfOperatorMergesIntoNumberOnTheRight()
    {
        Add(Number("12"));
        Add(SinToken.Instance);
        Add(Number("34"));
        MoveCursorTo(4);

        var (text, position) = Add(NumberToken.Three);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.EqualTo("12sin334"));
            Assert.That(position, Is.EqualTo(6));
            Assert.That(_tokens, Has.Count.EqualTo(3));
            Assert.That(_tokens[2].Value, Is.EqualTo("334"));
        }
    }

    [TestCase(DecimalSeparator.Period, ".")]
    [TestCase(DecimalSeparator.Comma, ",")]
    public void DecimalSeparatorEndsTheIntegerPartThatIsGrouped(
        DecimalSeparator decimalSeparator, string separator)
    {
        _decimalSeparator = decimalSeparator;
        TypeNumber("1234567");
        Add(DecimalSeparatorToken());

        var withSeparator = Add(NumberToken.Five);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(withSeparator.Text, Is.EqualTo($"1 234 567{separator}5"));
            Assert.That(withSeparator.Position, Is.EqualTo(11));
        }
    }

    [TestCase(DecimalSeparator.Period)]
    [TestCase(DecimalSeparator.Comma)]
    public void DecimalSeparatorIsRenderedAccordingToSettings(DecimalSeparator decimalSeparator)
    {
        _decimalSeparator = decimalSeparator;
        TypeNumber("1234567");
        Add(DecimalSeparatorToken());

        var (text, position) = Add(NumberToken.Five);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.EqualTo(decimalSeparator == DecimalSeparator.Comma
                ? "1 234 567,5"
                : "1 234 567.5"));
            Assert.That(position, Is.EqualTo(11));
            Assert.That(_tokens[0].Value, Is.EqualTo("1234567" + (char)decimalSeparator + "5"));
            Assert.That(_tokens, Has.Count.EqualTo(1));
        }
    }

    [TestCase(ThousandSeparator.Space, "1 234 567")]
    [TestCase(ThousandSeparator.Comma, "1,234,567")]
    [TestCase(ThousandSeparator.Period, "1.234.567")]
    public void ThousandSeparatorIsRenderedAccordingToSettings(ThousandSeparator thousandSeparator,
        string expectedText)
    {
        _thousandSeparator = thousandSeparator;
        TypeNumber("1234567");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(CurrentText, Is.EqualTo(expectedText));
            Assert.That(_tokens.DisplayPosition, Is.EqualTo(9));
        }
    }

    [Test]
    public void PastingIntoNumberMergesOnlyTheFirstTokenOfTheSequence()
    {
        TypeNumber("56");

        var (text, position) = Paste([Number("12"), PlusToken.Instance, Number("34")]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.EqualTo("5 612+34"));
            Assert.That(position, Is.EqualTo(8));
            Assert.That(_tokens, Has.Count.EqualTo(3));
            Assert.That(_tokens[0].Value, Is.EqualTo("5612"));
            Assert.That(_tokens[1].Value, Is.EqualTo("+"));
            Assert.That(_tokens[2].Value, Is.EqualTo("34"));
        }
    }

    [Test]
    public void DigitIsRejectedWhenNumberAlreadyHasMaxTotalDigits()
    {
        TypeNumber("123456789012345");
        Add(PlusToken.Instance);
        MoveCursorTo(18);

        var before = CurrentText;
        var (text, position) = Add(NumberToken.Four);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.EqualTo(before));
            Assert.That(position, Is.EqualTo(18));
            Assert.That(_tokens, Has.Count.EqualTo(2));
            Assert.That(_tokens[0].Value, Is.EqualTo("123456789012345"));
        }
    }

    [Test]
    public void DigitIsRejectedInFrontOfOperatorWhenNumberOnTheLeftIsFull()
    {
        TypeNumber("123456789012345");
        Add(PlusToken.Instance);
        MoveCursorTo(19);

        var (text, position) = Add(NumberToken.Four);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.EqualTo("123 456 789 012 345+"));
            Assert.That(position, Is.EqualTo(19));
            Assert.That(_tokens.Select(token => token.Value),
                Is.EqualTo(["123456789012345", "+"]));
        }
    }

    [Test]
    public void DigitIsRejectedInSecondHalfOfOperatorWhenNumberOnTheRightIsFull()
    {
        TypeNumber("12");
        Add(SinToken.Instance);
        TypeNumber("123456789012345");
        MoveCursorTo(4);

        var (text, position) = Add(NumberToken.Four);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.EqualTo("12sin123 456 789 012 345"));
            Assert.That(position, Is.EqualTo(5));
            Assert.That(_tokens.Select(token => token.Value),
                Is.EqualTo(["12", "sin", "123456789012345"]));
        }
    }

    [Test]
    public void PasteOfNumberWithMaxTotalDigitsIsRejected()
    {
        TypeNumber("12");
        Add(PlusToken.Instance);

        var (text, position) = Paste([Number("123456789012345")]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.EqualTo("12+"));
            Assert.That(position, Is.EqualTo(3));
            Assert.That(_tokens.Select(token => token.Value), Is.EqualTo(["12", "+"]));
        }
    }

    [Test]
    public void PasteOfNumberExceedingMaxTotalDigitsIsRejected()
    {
        var tooLong = Number(new string('9', Tokens.MaxTotalDigits + 1));

        var (text, position) = Paste([tooLong]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(_tokens, Is.Empty);
            Assert.That(text, Is.Empty);
            Assert.That(position, Is.Zero);
        }
    }

    [Test]
    public void PasteMergingNumbersIsRejectedWhenResultReachesMaxTotalDigits()
    {
        TypeNumber("12345678");
        Add(SinToken.Instance);
        TypeNumber("87654321");
        Add(SinToken.Instance);
        TypeNumber("87654321");
        MoveCursorTo(12);
        Remove();
        MoveCursorTo(22);
        Remove();
        MoveCursorTo(10);

        var before = CurrentText;
        var (text, position) = Paste([Number("5")]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.EqualTo(before));
            Assert.That(position, Is.EqualTo(10));
            Assert.That(_tokens.Select(token => token.Value),
                Is.EqualTo(["12345678", "87654321", "87654321"]));
        }
    }

    [Test]
    public void RemoveOnEmptyInputKeepsPositionAtZero()
    {
        var (text, position) = Remove();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.Empty);
            Assert.That(position, Is.Zero);
            Assert.That(_tokens.Count, Is.Zero);
        }
    }

    [Test]
    public void RemoveDeletesDigitBeforeCursor()
    {
        TypeNumber("1234");

        var (text, position) = Remove();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.EqualTo("123"));
            Assert.That(position, Is.EqualTo(3));
        }
    }

    [Test]
    public void RemoveDeletesSoleDigit()
    {
        TypeNumber("1");

        var (text, position) = Remove();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.Empty);
            Assert.That(position, Is.Zero);
            Assert.That(_tokens.Count, Is.Zero);
        }
    }

    [Test]
    public void RemoveMergesNumbersSeparatedByOperator()
    {
        TypeNumber("9");
        Add(PlusToken.Instance);
        TypeNumber("34");
        MoveCursorTo(2);

        var (text, position) = Remove();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.EqualTo("934"));
            Assert.That(position, Is.EqualTo(1));
            Assert.That(_tokens, Has.Count.EqualTo(1));
            Assert.That(_tokens[0].Value, Is.EqualTo("934"));
        }
    }

    [Test]
    public void RemoveDoesNotMergeNumbersWhenResultReachesMaxTotalDigits()
    {
        TypeNumber("12345678");
        Add(PlusToken.Instance);
        TypeNumber("12345678");
        MoveCursorTo(11);

        var before = CurrentText;
        var (text, position) = Remove();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.EqualTo(before));
            Assert.That(position, Is.EqualTo(11));
            Assert.That(_tokens.Select(token => token.Value),
                Is.EqualTo(["12345678", "+", "12345678"]));
        }
    }

    [Test]
    public void RemoveAtStartOfNumberAfterNumberRemovesWholePreviousToken()
    {
        TypeNumber("99");
        Add(SinToken.Instance);
        TypeNumber("34");
        MoveCursorTo(4);
        Remove();
        MoveCursorTo(2);

        var (text, position) = Remove();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.EqualTo("34"));
            Assert.That(position, Is.Zero);
            Assert.That(_tokens, Has.Count.EqualTo(1));
        }
    }

    [Test]
    public void RemoveBeforeOperatorDeletesLastDigitOfPreviousNumber()
    {
        TypeNumber("12");
        Add(PlusToken.Instance);
        MoveCursorTo(2);

        var (text, position) = Remove();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.EqualTo("1+"));
            Assert.That(position, Is.EqualTo(1));
            Assert.That(_tokens[0].Value, Is.EqualTo("1"));
        }
    }

    [Test]
    public void RemoveAtEndOfMultiCharOperatorRemovesWholeOperator()
    {
        TypeNumber("12");
        Paste([SinToken.Instance, Number("34")]);

        MoveCursorTo(4);
        var (text, position) = Remove();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.EqualTo("1234"));
            Assert.That(position, Is.EqualTo(2));
            Assert.That(_tokens, Has.Count.EqualTo(2));
            Assert.That(_tokens[1].Value, Is.EqualTo("34"));
        }
    }

    [Test]
    public void RemoveBeforeOperatorRemovesPreviousMultiCharOperator()
    {
        Paste([SinToken.Instance, PlusToken.Instance]);
        MoveCursorTo(3);

        var (text, position) = Remove();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.EqualTo("+"));
            Assert.That(position, Is.Zero);
            Assert.That(_tokens, Has.Count.EqualTo(1));
            Assert.That(_tokens[0], Is.SameAs(PlusToken.Instance));
        }
    }

    [TestCase("1E+15", 2, "1+15", 1)]
    [TestCase("1E+15", 3, "1E15", 2)]
    public void RemoveInFrontOfExponentMarkerSplitsMantissaFromExponent(
        string pastedValue, int removePosition, string expectedText, int expectedPosition)
    {
        Paste([Number(pastedValue)]);
        MoveCursorTo(removePosition);

        var (text, position) = Remove();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.EqualTo(expectedText));
            Assert.That(position, Is.EqualTo(expectedPosition));
        }
    }

    [Test]
    public void ExponentNumbersGroupOnlyTheMantissaIntegerPart()
    {
        var (text, position) = Paste([Number("1234567E+15")]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.EqualTo("1 234 567E+15"));
            Assert.That(position, Is.EqualTo(13));
            Assert.That(_tokens, Has.Count.EqualTo(1));
        }
    }

    [Test]
    public void PasteIntoEmptyInputInsertsAllTokens()
    {
        var (text, position) = Paste([Number("12"), PlusToken.Instance, Number("34")]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.EqualTo("12+34"));
            Assert.That(position, Is.EqualTo(5));
            Assert.That(_tokens, Has.Count.EqualTo(3));
        }
    }

    [Test]
    public void PasteEmptySequenceKeepsCaretPosition()
    {
        TypeNumber("12");

        var (text, position) = Paste([]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.EqualTo("12"));
            Assert.That(position, Is.EqualTo(2));
            Assert.That(_tokens, Has.Count.EqualTo(1));
        }
    }

    [Test]
    public void PasteMergesWithNumberToTheLeft()
    {
        Paste([Number("12"), PlusToken.Instance, Number("34")]);
        MoveCursorTo(4);

        var (text, position) = Paste([Number("5")]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.EqualTo("12+345"));
            Assert.That(position, Is.EqualTo(5));
            Assert.That(_tokens, Has.Count.EqualTo(3));
            Assert.That(_tokens[2].Value, Is.EqualTo("345"));
        }
    }

    [Test]
    public void PasteWithCaretInSecondHalfOfOperatorMergesWithTheNumberOnItsRight()
    {
        TypeNumber("12");
        Add(SinToken.Instance);
        TypeNumber("34");
        MoveCursorTo(4);

        var (text, position) = Paste([Number("5")]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.EqualTo("12sin534"));
            Assert.That(position, Is.EqualTo(6));
            Assert.That(_tokens, Has.Count.EqualTo(3));
            Assert.That(_tokens[0], Is.EqualTo(Number("12")));
            Assert.That(_tokens[1], Is.EqualTo(SinToken.Instance));
            Assert.That(_tokens[2], Is.EqualTo(Number("534")));
        }
    }

    [Test]
    public void PasteMergesWithNumbersOnBothSides()
    {
        Paste([Number("2"), SinToken.Instance, Number("3")]);
        MoveCursorTo(4);
        Remove();
        MoveCursorTo(2);

        var (text, position) = Paste([PlusToken.Instance, Number("7")]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.EqualTo("23+7"));
            Assert.That(position, Is.EqualTo(4));
            Assert.That(_tokens, Has.Count.EqualTo(3));
        }
    }

    [Test]
    public void PastingNextToAFullNumberAppliesOnlyThePartThatFits()
    {
        TypeNumber("12345678901234");

        var (text, position) = Paste([Number("12"), PlusToken.Instance, Number("34")]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.EqualTo("12 345 678 901 234+34"));
            Assert.That(position, Is.EqualTo(18));
            Assert.That(_tokens.Select(token => token.Value),
                Is.EqualTo(["12345678901234", "+", "34"]));
        }
    }

    [Test]
    public void PasteAfterOperatorInsertsNewNumberToken()
    {
        TypeNumber("12");
        Add(SinToken.Instance);

        var (text, position) = Paste([Number("34")]);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.EqualTo("12sin34"));
            Assert.That(position, Is.EqualTo(7));
            Assert.That(_tokens, Has.Count.EqualTo(3));
        }
    }

    [Test]
    public void UpdateTokenPositionOnEmptyInputResetsPositionToZero()
    {
        TypeNumber("12");
        MoveCursorTo(2);

        _tokens.Clear();
        _tokens.UpdateTokenPosition(4, false);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(_tokens.DisplayPosition, Is.Zero);
            Assert.That(CurrentText, Is.Empty);
        }
    }

    [Test]
    public void CursorPastEndOfInputClampsTokenButNotDisplayPosition()
    {
        TypeNumber("12");

        _tokens.UpdateTokenPosition(9, false);

        Assert.That(_tokens.DisplayPosition, Is.EqualTo(9));

        var (text, position) = Add(NumberToken.Three);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(text, Is.EqualTo("123"));
            Assert.That(position, Is.EqualTo(3));
        }
    }

    [Test]
    public void CursorSnapsBehindEveryThousandSeparator()
    {
        TypeNumber("1234567");
        var displayLength = CurrentText.Length;

        for (var displayPosition = 0; displayPosition <= displayLength; displayPosition++)
        {
            MoveCursorTo(displayPosition);

            Assert.That(_tokens.DisplayPosition,
                Is.EqualTo(displayPosition > 0 && IsAfterThousandSeparator(CurrentText,
                    displayPosition)
                    ? displayPosition - 1
                    : displayPosition),
                $"cursor at display position {displayPosition} of '{CurrentText}'");
        }
    }
}
