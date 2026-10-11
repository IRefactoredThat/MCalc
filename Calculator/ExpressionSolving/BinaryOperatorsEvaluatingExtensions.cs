using System.Collections.Immutable;
using Calculator.ExpressionPartConversions;
using Calculator.Operators;
using Essentials.Calculator;
using Essentials.ErrorType;
using Essentials.ResultType;

namespace Calculator.ExpressionSolving;

/// <summary>
/// Represents an extension class containing methods for evaluating
/// <see cref="IBinaryOperator"/> instances in an <see cref="Expression"/>.
/// </summary>
internal static class BinaryOperatorEvaluatingExtensions
{
    /// <summary>
    /// Evaluates <typeparamref name="T1"/> and <typeparamref name="T2"/>
    /// inside of the <paramref name="expression"/> left-to-right simultaneously.
    /// </summary>
    /// <typeparam name="T1">The type of the first operator to evaluate.</typeparam>
    /// <typeparam name="T2">The type of the second operator to evaluate</typeparam>
    /// <param name="expression">The expression to evaluate.</param>
    /// <returns>
    /// A new <see cref="Expression"/> object formed by evaluating
    /// <see cref="IBinaryOperator"/> instances in the <paramref name="expression"/>
    /// or one <see cref="Error"/> from
    /// <see cref="BinaryOperatorsExtensions.Apply(IBinaryOperator, IOperand, IOperand)"/>
    /// method.
    /// </returns>
    public static Result<Expression> EvaluateLeftAssociateBinaryOperators<T1, T2>
        (this Expression expression)
        where T1 : IBinaryOperator
        where T2 : IBinaryOperator
        => expression.EvaluateLeftAssociateBinaryOperators<T1, T2, T2>();

    /// <summary>
    /// Evaluates <typeparamref name="T1"/>, <typeparamref name="T2"/>
    /// and <typeparamref name="T3"/> inside of the
    /// <paramref name="expression"/> left-to-right simultaneously.
    /// </summary>
    /// <typeparam name="T1">The type of the first operator to evaluate.</typeparam>
    /// <typeparam name="T2">The type of the second operator to evaluate</typeparam>
    /// <typeparam name="T3">The type of the third parameter to evaluate.</typeparam>
    /// <param name="expression">The expression to evaluate.</param>
    /// <returns>
    /// A new <see cref="Expression"/> object formed by evaluating
    /// <see cref="IBinaryOperator"/> instances in the <paramref name="expression"/>
    /// or one <see cref="Error"/> from
    /// <see cref="BinaryOperatorsExtensions.Apply(IBinaryOperator, IOperand, IOperand)"/>
    /// method.
    /// </returns>
    public static Result<Expression> EvaluateLeftAssociateBinaryOperators<T1, T2, T3>
        (this Expression expression)
        where T1 : IBinaryOperator
        where T2 : IBinaryOperator
        where T3 : IBinaryOperator =>
        expression.Parts.ReducePortions<T1, T2, T3>(fromRight: false)
            .Map(parts => parts.AttachAngleMode(expression.Mode).ToResult());

    /// <summary>
    /// Evaluates <typeparamref name="T"/> inside of the <paramref name="expression"/> right-to-left.
    /// </summary>
    /// <typeparam name="T">The type of the operator to evaluate.</typeparam>
    /// <param name="expression">The expression to evaluate.</param>
    /// <returns>
    /// A new <see cref="Expression"/> object formed by evaluating
    /// <typeparamref name="T"/> instances in the <paramref name="expression"/> or one
    /// <see cref="Error"/> from the
    /// <see cref="BinaryOperatorsExtensions.Apply(IBinaryOperator, IOperand, IOperand)"/>
    /// method.
    /// </returns>
    public static Result<Expression> EvaluateRightAssociativeBinaryOperators<T>
        (this Expression expression) where T : IBinaryOperator =>
        expression.Parts.ReducePortions<T, T, T>(fromRight: true)
            .Map(parts => parts.AttachAngleMode(expression.Mode).ToResult());

    /// <summary>
    /// Reduces <typeparamref name="T1"/>, <typeparamref name="T2"/> and
    /// <typeparamref name="T3"/> operations that have evaluatable neighbors in
    /// <paramref name="parts"/>, one at a time, until no such operation remains.
    /// Operations are reduced from the left when <paramref name="fromRight"/> is
    /// <see langword="false"/> and from the right otherwise, which is what makes an
    /// operator associative to the left or to the right respectively.
    /// </summary>
    /// <typeparam name="T1">The type of the first operator to reduce.</typeparam>
    /// <typeparam name="T2">The type of the second operator to reduce.</typeparam>
    /// <typeparam name="T3">The type of the third operator to reduce.</typeparam>
    /// <param name="parts">The list of parts being evaluated.</param>
    /// <param name="fromRight">
    /// Whether to reduce the rightmost operation that has evaluatable neighbors
    /// instead of the leftmost one.
    /// </param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="ImmutableList{T}"/> representing
    /// either the reduced list of parts or one <see cref="Error"/> from the
    /// <see cref="BinaryOperatorsExtensions.Apply(IBinaryOperator, IOperand, IOperand)"/>
    /// method.
    /// </returns>
    private static Result<ImmutableList<IExpressionPart>> ReducePortions<T1, T2, T3>(this
        ImmutableList<IExpressionPart> parts, bool fromRight)
        where T1 : IBinaryOperator
        where T2 : IBinaryOperator
        where T3 : IBinaryOperator
    {
        var step = fromRight ? -1 : 1;

        for (var index = fromRight ? parts.Count - 2 : 1;
            index > 0 && index < parts.Count - 1;
            index += step)
        {
            if (parts[index] is T1 or T2 or T3 &&
                parts[index - 1] is IOperand or Expression &&
                parts[index + 1] is IOperand or Expression)
            {
                return parts.SplicePortion(index, (IBinaryOperator)parts[index])
                    .Map(reducedParts => reducedParts.ReducePortions<T1, T2, T3>(fromRight));
            }
        }

        return parts.ToResult();
    }

    /// <summary>
    /// Replaces the operation formed by the part at <paramref name="operatorIndex"/>
    /// and its two neighboring parts with the evaluated result.
    /// </summary>
    /// <param name="parts">The list containing the operation.</param>
    /// <param name="operatorIndex">The index of the operator in <paramref name="parts"/>.</param>
    /// <param name="operator">The operator applied to the neighboring parts.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="ImmutableList{T}"/> representing
    /// either the list with the operation replaced by its result or one
    /// <see cref="Error"/> from the
    /// <see cref="BinaryOperatorsExtensions.Apply(IBinaryOperator, IOperand, IOperand)"/>
    /// method.
    /// </returns>
    private static Result<ImmutableList<IExpressionPart>> SplicePortion(this
        ImmutableList<IExpressionPart> parts, int operatorIndex, IBinaryOperator @operator) =>
        parts[operatorIndex - 1].EvaluateOperand().Map(left =>
            parts[operatorIndex + 1].EvaluateOperand().Map(right =>
                @operator.Apply(left, right).Map(operand => parts
                    .RemoveAt(operatorIndex + 1)
                    .RemoveAt(operatorIndex)
                    .RemoveAt(operatorIndex - 1)
                    .Insert(operatorIndex - 1, operand).ToResult())));

    /// <summary>
    /// Evaluates <paramref name="part"/> to an <see cref="IOperand"/> by taking it as
    /// is if it already is an operand or evaluating it completely if it is an
    /// <see cref="Expression"/>.
    /// </summary>
    /// <param name="part">The part to evaluate.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="IOperand"/> representing either
    /// the evaluated operand or an <see cref="ExpressionNotEvaluated"/> error.
    /// </returns>
    private static Result<IOperand> EvaluateOperand(this IExpressionPart part) =>
        part switch
        {
            IOperand operand => operand.ToResult(),
            Expression expression => expression.EvaluateAllOperators(),
            _ => new ExpressionNotEvaluated()
        };

}
