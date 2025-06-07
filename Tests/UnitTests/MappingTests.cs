using Calculator.ExpressionPartsMapping;
using Calculator.Operands;
using Calculator.Operators;
using Essentials.ErrorType;
using Essentials.ResultType;
using Xunit;

namespace Tests.UnitTests;

public class MappingTests
{
    [Fact]
    public void NumberFollowedByPlus()
    {
        var (symbol, partInFront) = ("+", new Number());
        var partResult = GroupingOperatorMappingExtensions
            .MapToProperPart(symbol, partInFront);

        partResult.Switch
            (
                part => Assert.IsType<AdditionOperator>(part),
                error => Assert.Fail()
            );
    }

    [Fact]
    public void SubtractionOperatorFollowedByMinus()
    {
        var (symbol, partInFront) = ("-", new SubtractionOperator());
        var partResult = GroupingOperatorMappingExtensions
            .MapToProperPart(symbol, partInFront);

        partResult.Switch
            (
                part => Assert.IsType<NegationOperator>(part),
                error => Assert.Fail()
            );
    }

    [Fact]
    public void DivisionOperatorFollowedBySlash()
    {
        var (symbol, partInFront) = ("/", new DivisionOperator());
        var error = GroupingOperatorMappingExtensions
            .MapToProperPart(symbol, partInFront)
            .GetErrorOrNoError();
        Assert.IsType<InvalidBinaryOperatorUse>(error);
    }

    [Fact]
    public void NumberFollowedBySin()
    {
        var (symbol, partInFront) = ("sin", new Number());
        var partResult = GroupingOperatorMappingExtensions
            .MapToProperPart(symbol, partInFront);

        partResult.Switch
            (
                part => Assert.IsType<ImplicitMultiplicationOperator>(part),
                error => Assert.Fail()
            );
    }

    [Fact]
    public void PiConstantFollowedByNumber()
    {
        var (symbol, partInFront) = ("4", new PiConstant());
        var partResult = GroupingOperatorMappingExtensions
            .MapToProperPart(symbol, partInFront);

        partResult.Switch
            (
                part => Assert.IsType<Number>(part),
                error => Assert.Fail()
            );
    }

    [Fact]
    public void NumberFollowedByNumber()
    {
        var (symbol, partInFront) = ("4", new Number());
        var partResult = GroupingOperatorMappingExtensions
            .MapToProperPart(symbol, partInFront);

        partResult.Switch
            (
                part => Assert.IsType<Number>(part),
                error => Assert.Fail()
            );
    }

    [Fact]
    public void OpenParenthesesFollowedByClosedParentheses()
    {
        var (symbol, partInFront) = (")", new GroupingStartOperator());
        var error = GroupingOperatorMappingExtensions
            .MapToProperPart(symbol, partInFront)
            .GetErrorOrNoError();
        Assert.IsType<ExpressionContainsEmptyGrouping>(error);
    }

    [Fact]
    public void NumberFollowedByOpenParentheses()
    {
        var (symbol, partInFront) = ("(", new Number());
        var partResult = GroupingOperatorMappingExtensions
            .MapToProperPart(symbol, partInFront);

        partResult.Switch
            (
                part => Assert.IsType<ImplicitMultiplicationOperator>(part),
                error => Assert.Fail()
            );
    }
}