using Calculator.ExpressionComposition;
using Calculator.Operands;
using Calculator.Operators;
using Essentials.Calculator;
using Essentials.ResultType;
using static Calculator.ExpressionSolving.ParenthesesSubstitutionExtensions;
using Xunit;

namespace Tests.UnitTests;

public class ParenthesesSubstitutionTests
{
    [Theory]
    [InlineData("2+(((2+3)+(((2)+3))))")]
    public static void GetsASetOfParentheses1_ShouldProperlyComposeSubexpressions(string str)
    {
        var expressionResult = ExpressionParsingExtensions.Parse(str, AngleMode.DEG)
            .Map(expression => expression.SubstituteAllParentheses());

        expressionResult.Switch
            (
                expression =>
                {
                    Assert.IsType<Number>(expression.Parts[0]);
                    Assert.IsType<AdditionOperator>(expression.Parts[1]);
                    var expression1 = expression.Parts[2] as Expression;
                    Assert.NotNull(expression1);

                    var expression2 = expression1.Parts[0] as Expression;
                    Assert.NotNull(expression2);
                    Assert.IsType<Number>(expression2.Parts[0]);
                    Assert.IsType<AdditionOperator>(expression2.Parts[1]);
                    Assert.IsType<Number>(expression2.Parts[2]);

                    Assert.IsType<AdditionOperator>(expression1.Parts[1]);

                    var expression3 = expression1.Parts[2] as Expression;
                    Assert.NotNull(expression3);
                    Assert.IsType<Number>(expression3.Parts[0]);
                    Assert.IsType<AdditionOperator>(expression3.Parts[1]);
                    Assert.IsType<Number>(expression3.Parts[2]);
                },
                error => Assert.Fail()
            );
    }

    [Theory]
    [InlineData("(4)*(2+3)")]
    public void GetsASetOfParentheses2_ShouldProperlyComposeSubexpressions(string str)
    {
        var expressionResult = ExpressionParsingExtensions.Parse(str, AngleMode.DEG)
            .Map(expression => expression.SubstituteAllParentheses());

        expressionResult.Switch
            (
                expression =>
                {
                    Assert.IsType<Number>(expression.Parts[0]);
                    Assert.IsType<MultiplicationOperator>(expression.Parts[1]);
                    var expression1 = expression.Parts[2] as Expression;
                    Assert.NotNull(expression1);
                    Assert.IsType<Number>(expression1.Parts[0]);
                    Assert.IsType<AdditionOperator>(expression1.Parts[1]);
                    Assert.IsType<Number>(expression1.Parts[2]);
                },
                error => Assert.Fail()
            );
    }

    [Theory]
    [InlineData("((4)*(2+3(5+6)5))")]
    public void GetsASetOfParentheses3_ShouldProperlyComposeSubexpressions(string str)
    {
        var expressionResult = ExpressionParsingExtensions.Parse(str, AngleMode.RAD)
            .Map(value => value.SubstituteAllParentheses());

        expressionResult.Switch
            (
                expression =>
                {
                    Assert.IsType<Number>(expression.Parts[0]);
                    Assert.IsType<MultiplicationOperator>(expression.Parts[1]);

                    var expression1 = expression.Parts[2] as Expression;
                    Assert.NotNull(expression1);
                    Assert.IsType<Number>(expression1.Parts[0]);
                    Assert.IsType<AdditionOperator>(expression1.Parts[1]);
                    Assert.IsType<Number>(expression1.Parts[2]);
                    Assert.IsType<MultiplicationOperator>(expression1.Parts[3]);

                    var expression2 = expression1.Parts[4] as Expression;
                    Assert.NotNull(expression2);
                    Assert.IsType<Number>(expression2.Parts[0]);
                    Assert.IsType<AdditionOperator>(expression2.Parts[1]);
                    Assert.IsType<Number>(expression2.Parts[2]);

                    Assert.IsType<MultiplicationOperator>(expression1.Parts[5]);
                    Assert.IsType<Number>(expression1.Parts[6]);
                },
                error => Assert.Fail()
            );
    }
}
