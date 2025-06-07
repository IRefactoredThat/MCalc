using Calculator.ExpressionComposition;
using Essentials.ErrorType;
using Essentials.Calculator;
using Essentials.ResultType;
using Xunit;

namespace Tests.UnitTests;

public class ErrorsInExpressionTests
{
    [Theory]
    [InlineData("1//0+2+3+4")]
    public void GetsInproperExpressionString_ShouldHaveDivisionError(string str)
    {
        var expressionResult = ExpressionParsingExtensions.Parse(str, AngleMode.RAD);

        Assert.IsType<InvalidBinaryOperatorUse>(
            expressionResult.GetErrorOrNoError());
    }

    [Theory]
    [InlineData("213@+323")]
    public void GetsInproperExpressionString_ShouldHaveSymbolError(string str)
    {
        var expressionResult = ExpressionParsingExtensions.Parse(str, AngleMode.RAD);
        Assert.IsType<InvalidSymbolsInExpression>
            (expressionResult.GetErrorOrNoError());
    }
}