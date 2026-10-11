using Calculator.ExpressionPartConversions;
using Calculator.Operators;
using Essentials.Calculator;
using Essentials.ErrorType;
using Essentials.ResultType;

namespace Calculator.ExpressionSolving;

/// <summary>
/// Represents an extension class containing methods for evaluating
/// <see cref="IRightSideUnaryOperator"/> instances in an <see cref="Expression"/>.
/// </summary>
internal static class RightSideUnaryOperatorsEvaluatingExtensions
{
    /// <summary>
    /// Evaluates all <see cref="IRightSideUnaryOperator"/> inside of the
    /// <paramref name="expression"/>, effectively removing these instances.
    /// </summary>
    /// <param name="expression">An <see cref="Expression"/> object to evaluate.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="Expression"/> formed by
    /// evaluating <see cref="IRightSideUnaryOperator"/> instances in the
    /// <paramref name="expression"/> or one <see cref="Error"/> from
    /// <see cref="UnaryOperatorsExtensions.Apply(IUnaryOperator, IOperand, AngleMode)"/>
    /// method.
    /// </returns>
    public static Result<Expression> EvaluateRightSideUnaryOperators(this Expression expression)
    {
        var newExpression = Expression.Create(expression.Mode);
        var result = newExpression.ToResult();

        foreach (var part in expression.Parts)
        {
            if (part is not IRightSideUnaryOperator unaryOperator)
            {
                newExpression.AddPart(part);
                continue;
            }

            if (newExpression.Parts.Count == 0)
            {
                return new InvalidRightSideUnaryOperatorUse(unaryOperator.ToToken());
            }

            var computedOperand = newExpression.Parts[^1] switch
            {
                Expression innerExpression => innerExpression.EvaluateAllOperators()
                    .Map(operand => unaryOperator.Apply(operand, innerExpression.Mode)),
                IOperand operand => unaryOperator.Apply(operand, expression.Mode),
                _ => new InvalidRightSideUnaryOperatorUse(unaryOperator.ToToken())
            };

            result = result.Map(current => computedOperand.Map(value =>
            {
                current.RemovePart(current.Parts.Count - 1).AddPart(value);
                return current.ToResult();
            }));
        }

        return result;
    }
}
