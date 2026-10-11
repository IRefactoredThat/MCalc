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
        expression.ReducePortions<T1, T2, T3>(fromRight: false);

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
        expression.ReducePortions<T, T, T>(fromRight: true);

    /// <summary>
    /// Reduces every <typeparamref name="T1"/>, <typeparamref name="T2"/> and
    /// <typeparamref name="T3"/> operation that has evaluatable neighbors in a
    /// single pass over the parts of the <paramref name="expression"/>. Operations
    /// are reduced from the left when <paramref name="fromRight"/> is
    /// <see langword="false"/> and from the right otherwise, which is what makes an
    /// operator associative to the left or to the right respectively.
    /// </summary>
    /// <typeparam name="T1">The type of the first operator to reduce.</typeparam>
    /// <typeparam name="T2">The type of the second operator to reduce.</typeparam>
    /// <typeparam name="T3">The type of the third operator to reduce.</typeparam>
    /// <param name="expression">The expression whose parts are reduced.</param>
    /// <param name="fromRight">
    /// Whether to reduce the operations right-to-left instead of left-to-right.
    /// </param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="Expression"/> representing
    /// either the reduced expression or one <see cref="Error"/> from the
    /// <see cref="BinaryOperatorsExtensions.Apply(IBinaryOperator, IOperand, IOperand)"/>
    /// method.
    /// </returns>
    private static Result<Expression> ReducePortions<T1, T2, T3>(this
        Expression expression, bool fromRight)
        where T1 : IBinaryOperator
        where T2 : IBinaryOperator
        where T3 : IBinaryOperator =>
        fromRight
            ? expression.ReducePortionsFromRight<T1, T2, T3>()
            : expression.ReducePortionsFromLeft<T1, T2, T3>();

    /// <summary>
    /// Folds the <typeparamref name="T1"/>, <typeparamref name="T2"/> and
    /// <typeparamref name="T3"/> operations of the <paramref name="expression"/>
    /// left-to-right in a single pass, so that grouped operations are evaluated
    /// from the left.
    /// </summary>
    /// <typeparam name="T1">The type of the first operator to reduce.</typeparam>
    /// <typeparam name="T2">The type of the second operator to reduce.</typeparam>
    /// <typeparam name="T3">The type of the third operator to reduce.</typeparam>
    /// <param name="expression">The expression whose parts are reduced.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="Expression"/> representing
    /// either the reduced expression or one <see cref="Error"/> produced while
    /// evaluating an operand.
    /// </returns>
    private static Result<Expression> ReducePortionsFromLeft<T1, T2, T3>(this Expression expression)
        where T1 : IBinaryOperator
        where T2 : IBinaryOperator
        where T3 : IBinaryOperator
    {
        var parts = expression.Parts;
        var newExpression = Expression.Create(expression.Mode);
        var result = newExpression.ToResult();
        var index = 0;

        while (index < parts.Count)
        {
            if (parts[index] is not (IOperand or Expression))
            {
                newExpression.AddPart(parts[index]);
                index++;
                continue;
            }

            var operand = parts[index].EvaluateOperand();
            index++;

            while (index + 1 < parts.Count &&
                (parts[index] is T1 or T2 or T3) &&
                (parts[index + 1] is IOperand or Expression))
            {
                var @operator = (IBinaryOperator)parts[index];
                var right = parts[index + 1].EvaluateOperand();
                operand = operand.Map(left =>
                    right.Map(rightOperand => @operator.Apply(left, rightOperand)));
                index += 2;
            }

            result = result.Map(current =>
                operand.Map(value => current.AddPart(value).ToResult()));
        }

        return result;
    }

    /// <summary>
    /// Folds the <typeparamref name="T1"/>, <typeparamref name="T2"/> and
    /// <typeparamref name="T3"/> operations of the <paramref name="expression"/>
    /// right-to-left in a single pass, so that grouped operations are evaluated
    /// from the right.
    /// </summary>
    /// <typeparam name="T1">The type of the first operator to reduce.</typeparam>
    /// <typeparam name="T2">The type of the second operator to reduce.</typeparam>
    /// <typeparam name="T3">The type of the third operator to reduce.</typeparam>
    /// <param name="expression">The expression whose parts are reduced.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="Expression"/> representing
    /// either the reduced expression or one <see cref="Error"/> produced while
    /// evaluating an operand.
    /// </returns>
    private static Result<Expression> ReducePortionsFromRight<T1, T2, T3>(this Expression expression)
        where T1 : IBinaryOperator
        where T2 : IBinaryOperator
        where T3 : IBinaryOperator
    {
        var parts = expression.Parts;
        var reversedParts = new List<IExpressionPart>();
        var result = reversedParts.ToResult();
        var index = parts.Count - 1;

        while (index >= 0)
        {
            if (parts[index] is not (IOperand or Expression))
            {
                reversedParts.Add(parts[index]);
                index--;
                continue;
            }

            var operand = parts[index].EvaluateOperand();
            index--;

            while (index - 1 >= 0 &&
                (parts[index] is T1 or T2 or T3) &&
                (parts[index - 1] is IOperand or Expression))
            {
                var @operator = (IBinaryOperator)parts[index];
                var left = parts[index - 1].EvaluateOperand();
                operand = operand.Map(right =>
                    left.Map(leftOperand => @operator.Apply(leftOperand, right)));
                index -= 2;
            }

            result = result.Map(current => operand.Map(value =>
            {
                current.Add(value);
                return current.ToResult();
            }));
        }

        return result.Map(finalParts =>
        {
            finalParts.Reverse();
            var newExpression = Expression.Create(expression.Mode);
            foreach (var part in finalParts)
            {
                newExpression.AddPart(part);
            }
            return newExpression.ToResult();
        });
    }

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
