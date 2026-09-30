using System.Collections.Immutable;
using System.Globalization;
using Calculator.ExpressionPartConversions;
using Calculator.Operators;
using Essentials.Calculator;
using Essentials.ErrorType;
using Essentials.ResultType;
using static Calculator.ExpressionPartsMapping.GroupingOperatorMappingExtensions;

namespace Calculator.ExpressionComposition;

/// <summary>
/// Represents an extension class containing a method for parsing a <see cref="string"/>
/// into an <see cref="Expression"/>.
/// </summary>
internal static class ExpressionParsingExtensions
{
    /// <summary>
    /// Parses an <see cref="IEnumerable{T}"/> of type <see cref="IToken"/>
    /// into an <see cref="Expression"/> object, which will have specified <see cref="AngleMode"/>.
    /// </summary>
    /// <param name="tokens">Tokens to parse.</param>
    /// <param name="mode">An <see cref="AngleMode"/> for <see cref="Expression"/>.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="Expression"/> either representing
    /// a new, partially validated <see cref="Expression"/> object 
    /// or one <see cref="Error"/> listed here:
    /// <see cref="InvalidBinaryOperatorUse"/> |
    /// <see cref="InvalidGroupingEndOperatorUse"/> |
    /// <see cref="ExpressionContainsEmptyGrouping"/> |
    /// <see cref="InvalidExpressionEnd"/>.
    /// </returns>
    /// <remarks>
    /// <para><b>Note</b>: This method does not find all possible errors -
    /// other errors are tracked down upon evaluation.
    /// </para>
    /// </remarks>
    public static Result<Expression> Parse(IReadOnlyList<IToken> tokens, NumberFormatInfo info, AngleMode mode)
    {
        if (tokens is [NumberToken])
        {
            return new EmptyExpression();
        }
        var validatedParts = tokens
            .Aggregate(ImmutableArray.Create<IExpressionPart>(
                new EmptyPart()).ToResult(),
            (accumulatedParts, token) => accumulatedParts
                .SelfMap(parts => MapToProperPart(parts, info, token)),
            finalParts => finalParts.SelfMap(parts => parts.RemoveAt(0)));

        return validatedParts
            .Map(parts => parts
                .SubstituteImplicitMultiplications()
                .ToImmutableArray()
                .EvaluateChainedOperands()
                .AttachAngleMode(mode)
                .ToResult())
            .SelfMap(finalExpression => finalExpression.Parts switch
            {
                [] => new EmptyExpression(),
                // Expression can only end with ), some number or !, if that's not the case...
                [.., not (GroupingEndOperator or IOperand or FactorialOperator)] => new InvalidExpressionEnd(),
                _ => finalExpression
            });
    }
}