using Calculator.Operators;
using Calculator.ExpressionPartConversions;
using Essentials.Linq;
using Essentials.Calculator;
using Essentials.ResultType;
using Essentials.ErrorType;
using System.Collections.Immutable;
using static Calculator.ExpressionComposition.ExpressionPartsParsingExtensions;
using static Calculator.ExpressionPartsMapping.GroupingOperatorMappingExtensions;

namespace Calculator.ExpressionComposition;

/// <summary>
/// Represents an extension class containing a method for parsing a <see cref="string"/>
/// into an <see cref="Expression"/>.
/// </summary>
internal static class ExpressionParsingExtensions
{
    /// <summary>
    /// Parses a <see cref="string"/> into an <see cref="Expression"/> object, 
    /// which will have specified <see cref="AngleMode"/>.
    /// </summary>
    /// <param name="expression">A <see cref="string"/> to parse.</param>
    /// <param name="mode">An <see cref="AngleMode"/> for <see cref="Expression"/>.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="Expression"/> either representing
    /// a new, partially validated <see cref="Expression"/> object 
    /// or one <see cref="Error"/> listed here:
    /// <see cref="EmptyExpression"/> |
    /// <see cref="InvalidSymbolsInExpression"/> |
    /// <see cref="InvalidExponentiationOperatorUse"/> |
    /// <see cref="InvalidDivisionOperatorUse"/> |
    /// <see cref="InvalidMultiplicationOperatorUse"/> |
    /// <see cref="InvalidGroupingEndOperatorUse"/> |
    /// <see cref="ExpressionContainsEmptyGrouping"/> |
    /// <see cref="UndefinedExpressionPartMapping"/> |
    /// <see cref="InvalidExpressionEnd"/>.
    /// </returns>
    /// <remarks>
    /// <para><b>Note</b>: This method does not find all possible errors -
    /// other errors are tracked down upon evaluation.
    /// </para>
    /// </remarks>
    public static Result<Expression> Parse(string expression, AngleMode mode)
    {
        expression = expression.Replace(" ", "");

        if(expression == string.Empty)
        {
            return new EmptyExpression();
        }

        var partsAsStrings = Symbols(expression)
            .ToImmutableList();

        if(!string.Concat(partsAsStrings)
            .SequenceEqual(expression))
        {
            return new InvalidSymbolsInExpression();
        }

        var ignoredSymbol = new EmptyPart().ToResult<IExpressionPart>();

        var resultParts = partsAsStrings.ScanWithoutSeed(ignoredSymbol, 
            (partInFront, currentSymbol) => 
                partInFront.Map(part => MapToProperPart(currentSymbol, part))
        );

        return resultParts.ValuesOrFirstError()
                .Map(parts => parts
                        .SubstituteImplicitMultiplications()
                        .EvaluateChainedOperands()
                        .AttachAngleMode(mode)
                        .ToResult())
                .SelfMap(expression => expression.Parts switch
                {
                    [.., not (GroupingEndOperator or IOperand or FactorialOperator)] 
                        => new InvalidExpressionEnd(),
                    _ => expression
                });
    }
}