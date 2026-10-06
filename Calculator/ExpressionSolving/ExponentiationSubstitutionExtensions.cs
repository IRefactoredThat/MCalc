using System.Collections.Immutable;
using Calculator.ExpressionPartConversions;
using Calculator.Operators;
using Essentials.Calculator;

namespace Calculator.ExpressionSolving;

internal static class ExponentiationSubstitutionExtensions
{
    /// <summary>
    /// Substitutes each <see cref="ExponentiationOperator"/> preceded by a specific
    /// sequence of <see cref="IExpressionPart"/> instances with a
    /// <see cref="NegativeExponentiationOperator"/> inside <paramref name="expression"/>
    /// and inner expressions to prevent errors in calculations.
    /// For example, without invoking this method, -1^-2 would be equivalent to
    /// (-1)^(-2), but when you do invoke it, it would be equivalent to -(1^-2),
    /// which is the order used by most calculators.
    /// </summary>
    /// <param name="expression">The <see cref="Expression"/> used for substitution.</param>
    /// <returns>
    /// A new <see cref="Expression"/> object with zero or more
    /// <see cref="ExponentiationOperator"/> instances replaced with a
    /// <see cref="NegativeExponentiationOperator"/> instances.
    /// </returns>
    public static Expression SubstituteAllExponentiationOperators(this Expression expression)
        => expression.Parts
            .Aggregate(ImmutableList<IExpressionPart>.Empty,
            (newParts, part) =>
            {
                if (part is ExponentiationOperator)
                {
                    var shouldTransform = false;
                    if (newParts is [.., NegationOperator, IUnaryOperator, IOperand] or
                        [.., NegationOperator, IUnaryOperator, Expression])
                    {
                        if (newParts is [.., not ExponentiationOperator, _, _, _])
                        {
                            shouldTransform = true;
                        }
                    }
                    else if (newParts is ([.., NegationOperator, IOperand] or
                             [.., NegationOperator, Expression]) and [.., not ExponentiationOperator, _, _])
                    {
                        shouldTransform = true;
                    }
                    if (shouldTransform)
                    {
                        return newParts.Add(new NegativeExponentiationOperator());
                    }
                    return newParts.Add(part);
                }
                if (part is Expression innerExpression)
                {
                    return newParts.Add(innerExpression
                        .SubstituteAllExponentiationOperators());
                }
                return newParts.Add(part);
            }, finalParts => finalParts.AttachAngleMode(expression.Mode));

}
