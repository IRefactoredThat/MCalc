using Calculator.Operators;
using Essentials.Calculator;
using Essentials.ErrorType;
using Essentials.ResultType;
using static Calculator.ExpressionPartsMapping.ExpressionPartSymbols;

namespace Calculator.ExpressionPartsMapping;

/// <summary>
/// Represents a class containing a method used for mapping a symbol
/// to an <see cref="IGroupingOperator"/> instance.
/// </summary>
internal class GroupingOperatorMappingExtensions
{
    /// <summary>
    /// Maps a <paramref name="symbol"/> into an <see cref="IGroupingOperator"/> instance
    /// or <see cref="ImplicitMultiplicationOperator"/> instance wrapping 
    /// <see cref="IGroupingOperator"/> instance
    /// based on <see cref="string"/> contents and <paramref name="precedingPart"/> formed
    /// by previous invocation of <see cref="ExpressionPartsMapping"/> method chain
    /// or delegates this request to the
    /// <see cref="UnaryOperatorMappingExtensions"/> class if the <paramref name="symbol"/>
    /// does not represent one of the aforementioned instances. This method represent the
    /// start of the chain.
    /// </summary>
    /// <param name="symbol">A <see cref="string"/> to map.</param>
    /// <param name="precedingPart">An <see cref="IExpressionPart"/> formed 
    /// by previous invocation.</param>
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
    /// but the resulting instance cannot be preceded by <paramref name="precedingPart"/>.
    /// </para>
    /// 3) A <see cref="Result{T}"/> of type <see cref="IExpressionPart"/> formed by
    /// delegating the request to the <see cref="UnaryOperatorMappingExtensions"/> class,
    /// when the mapping was not successful.
    /// </returns>
    public static Result<IExpressionPart> MapToProperPart(string symbol,
        IExpressionPart precedingPart) => (symbol, precedingPart) switch
        {
            (Open, IOperand or GroupingEndOperator or
                IRightSideUnaryOperator or
                ImplicitMultiplicationOperator(IOperand) or 
                ImplicitMultiplicationOperator(GroupingEndOperator) or
                ImplicitMultiplicationOperator(IRightSideUnaryOperator))
                => new ImplicitMultiplicationOperator(new GroupingStartOperator()),
            (Open, _)
                => new GroupingStartOperator(),

            (Closed, IBinaryOperator or ILeftSideUnaryOperator)
                => new InvalidGroupingEndOperatorUse(),
            (Closed, GroupingStartOperator)
                => new ExpressionContainsEmptyGrouping(),
            (Closed, _)
                => new GroupingEndOperator(),

            _ => UnaryOperatorMappingExtensions.
                   UnaryOperatorMappings(symbol, precedingPart),
        };
}