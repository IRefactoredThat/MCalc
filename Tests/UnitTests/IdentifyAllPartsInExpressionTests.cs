using Calculator.ExpressionComposition;
using Calculator.Operands;
using Calculator.Operators;
using Essentials.ResultType;
using Essentials.Calculator;
using Xunit;

namespace Tests.UnitTests;

public class IdentifyAllPartsInExpressionTests
{
    [Theory]
    [InlineData("2(2)2sin5sin-tan√∛ln5")]
    public void GetsProperExpressionString1_ShouldIdentifyAllParts(string str)
    {
        var expressionResult = ExpressionParsingExtensions.Parse(str, AngleMode.RAD);

        expressionResult.Switch
        (
            value => Assert.Collection(value.Parts,
                part => Assert.IsType<Number>(part),
                part => Assert.IsType<MultiplicationOperator>(part),
                part => Assert.IsType<GroupingStartOperator>(part),
                part => Assert.IsType<Number>(part),
                part => Assert.IsType<GroupingEndOperator>(part),
                part => Assert.IsType<MultiplicationOperator>(part),
                part => Assert.IsType<Number>(part),
                part => Assert.IsType<MultiplicationOperator>(part),
                part => Assert.IsType<SineOperator>(part),
                part => Assert.IsType<Number>(part),
                part => Assert.IsType<MultiplicationOperator>(part),
                part => Assert.IsType<SineOperator>(part),
                part => Assert.IsType<NegationOperator>(part),
                part => Assert.IsType<TangentOperator>(part),
                part => Assert.IsType<SquareRootOperator>(part),
                part => Assert.IsType<CubeRootOperator>(part),
                part => Assert.IsType<NaturalLogarithmOperator>(part),
                part => Assert.IsType<Number>(part)
            ),

            error => Assert.Fail()
        );
    }

    [InlineData("1+2*3+4*5")]
    [Theory]
    public void GetsProperExpressionString2_ShouldIdentifyAllParts(string str)
    {
        var expressionResult = ExpressionParsingExtensions.Parse(str, AngleMode.RAD);
        expressionResult.Switch
        (
            value =>
            {
                Assert.Collection(value.Parts,
                    part => Assert.IsType<Number>(part),
                    part => Assert.IsType<AdditionOperator>(part),
                    part => Assert.IsType<Number>(part),
                    part => Assert.IsType<MultiplicationOperator>(part),
                    part => Assert.IsType<Number>(part),
                    part => Assert.IsType<AdditionOperator>(part),
                    part => Assert.IsType<Number>(part),
                    part => Assert.IsType<MultiplicationOperator>(part),
                    part => Assert.IsType<Number>(part)
                );
            },

            error => Assert.Fail()
        );
    }
}