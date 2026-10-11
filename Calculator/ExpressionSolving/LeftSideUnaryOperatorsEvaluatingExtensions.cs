using Calculator.Operators;
using Essentials.Calculator;
using Essentials.ErrorType;
using Essentials.ResultType;

namespace Calculator.ExpressionSolving;

/// <summary>
/// Represents an extension class containing methods for evaluating
/// <see cref="ILeftSideUnaryOperator"/> instances in an <see cref="Expression"/>.
/// </summary>
internal static class LeftSideUnaryOperatorsEvaluatingExtensions
{
    /// <summary>
    /// Evaluates all <see cref="ILeftSideUnaryOperator"/> inside of the
    /// <paramref name="expression"/>, effectively removing these instances.
    /// </summary>
    /// <param name="expression">An <see cref="Expression"/> object to evaluate.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="Expression"/> formed by
    /// evaluating <see cref="ILeftSideUnaryOperator"/> instances in the
    /// <paramref name="expression"/> or one <see cref="Error"/> from
    /// <see cref="UnaryOperatorsExtensions.Apply(IUnaryOperator, IOperand, AngleMode)"/>
    /// method.
    /// </returns>
    public static Result<Expression> EvaluateLeftSideUnaryOperators(this Expression expression)
    {
        var newExpression = Expression.Create(expression.Mode);
        var pendingOperators = new List<ILeftSideUnaryOperator>();
        var result = newExpression.ToResult();

        foreach (var part in expression.Parts)
        {
            if (part is ILeftSideUnaryOperator leftSideOperator)
            {
                pendingOperators.Add(leftSideOperator);
                continue;
            }

            if ((part is Expression or IOperand) && pendingOperators.Count > 0)
            {
                var mode = part is Expression inner ? inner.Mode : expression.Mode;

                var computedOperand = part switch
                {
                    Expression innerExpression => innerExpression.EvaluateAllOperators(),
                    IOperand operand => operand.ToResult(),
                    _ => new ExpressionNotEvaluated()
                };

                // Operators closest to the operand are applied first.
                for (var offset = pendingOperators.Count - 1; offset >= 0; offset--)
                {
                    var unaryOperator = pendingOperators[offset];
                    computedOperand = computedOperand.Map(
                        operand => unaryOperator.Apply(operand, mode));
                }

                result = result.Map(current => computedOperand.Map(value =>
                {
                    current.AddPart(value);
                    return current.ToResult();
                }));
                pendingOperators.Clear();
                continue;
            }

            // A left-side unary operator that is not immediately followed by an
            // operand cannot be applied yet, so it is preserved as it was.
            foreach (var unaryOperator in pendingOperators)
            {
                newExpression.AddPart(unaryOperator);
            }
            pendingOperators.Clear();

            newExpression.AddPart(part);
        }

        foreach (var unaryOperator in pendingOperators)
        {
            newExpression.AddPart(unaryOperator);
        }

        return result;
    }
}
