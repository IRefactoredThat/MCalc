using Calculator.ExpressionSolving;
using Essentials.ResultType;
using Essentials.Calculator;
using Xunit;

namespace Tests.UnitTests;

public class ResultTests
{
    [Theory]
    [InlineData("(((4)*(2+3(5+6)5)))")]
    public void GetsAExpression1_ShouldReturn668(string str)
    {
        var result = ExpressionEvaluatingExtensions.Evaluate(str, AngleMode.RAD);

        Assert.True(result.BoolMap(operand => operand.Value == 668));
    }

    [Theory]
    [InlineData("(2)(3)^5+(3-3)*(2/6)+(12+2*3)^6")]
    public void GetsAExpression2_ShouldReturn34012710(string str)
    {
        var result = ExpressionEvaluatingExtensions.Evaluate(str, AngleMode.RAD);
        Assert.True(result.BoolMap(operand => operand.Value == 34012710));
    }

    [Theory]
    [InlineData("8+2*(3^2-4)+16/4-5*(6-2)+10")]
    public void GetsAComplexExpression_ShouldReturn12(string str)
    {
        var result = ExpressionEvaluatingExtensions.Evaluate(str, AngleMode.RAD);
        Assert.True(result.BoolMap(operand => operand.Value == 12));
    }

    [Theory]
    [InlineData("sin(π/3)cos(π/3)/cos(π/6)(π/6)(1+0.5-2/4)")]
    public void GetsASimpleExpression_ShouldReturn(string str)
    {
        var result = ExpressionEvaluatingExtensions.Evaluate(str, AngleMode.RAD);
        result.Switch(
            operand => Assert.Equal(0.26179938779, operand.Value, 1e-11),
            _ => Assert.Fail()
        );
    }

    [Theory]
    [InlineData("-1^-2")]
    public void GetsASimpleExpression_ShouldReturnMinusOne(string str)
    {
        var result = ExpressionEvaluatingExtensions.Evaluate(str, AngleMode.RAD);
        result.Switch(
            operand => Assert.Equal(-1, operand.Value),
            _ => Assert.Fail()
        );
    }
}