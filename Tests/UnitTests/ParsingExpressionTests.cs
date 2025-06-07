using Calculator.ExpressionComposition;
using Xunit;

namespace Tests.UnitTests;

public class ParsingExpressionTests
{
    [Theory]
    [InlineData("1//0+2+3+4")]
    public void GetsExpressionString1_ShouldParseAllParts(string str)
    {
        var parts = ExpressionPartsParsingExtensions.Symbols(str);

        Assert.Collection(parts,
            part => Assert.Equal("1", part),
            part => Assert.Equal("/", part),
            part => Assert.Equal("/", part),
            part => Assert.Equal("0", part),
            part => Assert.Equal("+", part),
            part => Assert.Equal("2", part),
            part => Assert.Equal("+", part),
            part => Assert.Equal("3", part),
            part => Assert.Equal("+", part),
            part => Assert.Equal("4", part)
        );
    }

    [Theory]
    [InlineData("sin cos tan cot 5.5 555 5.55 1 2")]
    public void GetsExpressionString2_ShouldParseAllParts(string str)
    {
        var parts = ExpressionPartsParsingExtensions.Symbols(str);

        Assert.Collection(parts,
            part => Assert.Equal("sin", part),
            part => Assert.Equal("cos", part),
            part => Assert.Equal("tan", part),
            part => Assert.Equal("cot", part),
            part => Assert.Equal("5.5", part),
            part => Assert.Equal("555", part),
            part => Assert.Equal("5.55", part),
            part => Assert.Equal("1", part),
            part => Assert.Equal("2", part)
        );
    }
}