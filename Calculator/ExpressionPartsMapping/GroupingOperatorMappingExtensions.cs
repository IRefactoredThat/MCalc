using System.Collections.Immutable;
using System.Globalization;
using Calculator.Operators;
using Essentials.Calculator;
using Essentials.ErrorType;
using Essentials.ResultType;

namespace Calculator.ExpressionPartsMapping;

/// <summary>
/// Represents a class containing a method used for mapping a token
/// to an <see cref="IGroupingOperator"/> instance.
/// </summary>
internal static class GroupingOperatorMappingExtensions
{
    /// <summary>
    /// Maps a <paramref name="token"/> into an <see cref="IGroupingOperator"/> instance
    /// or <see cref="ImplicitMultiplicationOperator"/> instance wrapping
    /// <see cref="IGroupingOperator"/> instance
    /// based on <see cref="IExpressionPart"/> type and <paramref name="token"/> formed
    /// by previous invocation of <see cref="ExpressionPartsMapping"/> method chain
    /// or delegates this request to the
    /// <see cref="UnaryOperatorMappingExtensions"/> class if the <paramref name="token"/>
    /// does not represent one of the aforementioned instances. This method represent the
    /// start of the chain.
    /// </summary>
    /// <param name="token">An assumed <see cref="IExpressionPart"/> to map.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="IExpressionPart"/> either being:
    /// <para>
    /// 1) An <see cref="IGroupingOperator"/> instance or
    /// <see cref="ImplicitMultiplicationOperator"/> instance,
    /// when the mapping was successful.
    /// </para>
    /// <para>
    /// 2) An <see cref="Error"/> listed here: <see cref="InvalidGroupingEndOperatorUse"/>
    /// | <see cref="ExpressionContainsEmptyGrouping"/>, when the mapping was successful,
    /// but the resulting instance cannot be preceded by <see cref="IExpressionPart"/>.
    /// </para>
    /// 3) A <see cref="Result{T}"/> of type <see cref="IExpressionPart"/> formed by
    /// delegating the request to the <see cref="UnaryOperatorMappingExtensions"/> class,
    /// when the mapping was not successful.
    /// </returns>
    public static Result<ImmutableArray<IExpressionPart>> MapToProperPart(ImmutableArray<IExpressionPart> parts,
        NumberFormatInfo info, IToken token) => (token, parts[^1]) switch
        {
            (OpenBracketToken, IOperand or GroupingEndOperator or
                IRightSideUnaryOperator or
                ImplicitMultiplicationOperator(IOperand))
                => parts.Add(new ImplicitMultiplicationOperator(new GroupingStartOperator())),

            (ClosedBracketToken, IBinaryOperator or ILeftSideUnaryOperator)
                => new InvalidGroupingEndOperatorUse(),
            (ClosedBracketToken, GroupingStartOperator)
                => new ExpressionContainsEmptyGrouping(),
            (ClosedBracketToken, _) => parts.Add(new GroupingEndOperator()),
            (OpenBracketToken, _) => parts.Add(new GroupingStartOperator()),

            _ => UnaryOperatorMappingExtensions.
                   UnaryOperatorMappings(parts, info, token),
        };
}
