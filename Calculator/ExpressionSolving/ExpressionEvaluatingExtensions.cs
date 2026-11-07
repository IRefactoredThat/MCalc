using Calculator.Operators;
using Essentials.Calculator;
using Essentials.ErrorType;
using Essentials.Numerics;
using Essentials.ResultType;

namespace Calculator.ExpressionSolving;

/// <summary>
/// Represents an extension class containing methods for
/// evaluating an <see cref="Expression"/> completely.
/// </summary>
public static class ExpressionEvaluatingExtensions
{
    /// <summary>
    /// Evaluates an <see cref="Expression"/>, trying to determine the result. Then, the result is rounded.
    /// </summary>
    /// <param name="expression">The <see cref="Expression"/> to evaluate.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="double"/> either
    /// representing the rounded computed result, or an <see cref="Error"/> object.
    /// </returns>
    public static Result<Number> EvaluateAndRound(Expression expression) =>
        expression.MatchAllParentheses().SubstituteAllParentheses()
            .Map(simplifiedExpression => simplifiedExpression.EvaluateAllOperators());

    /// <summary>
    /// Evaluates all operators inside the <paramref name="expression"/> with no
    /// parentheses in order to determine its result.
    /// </summary>
    /// <param name="expression">The <see cref="Expression"/> to evaluate.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="Number"/> either
    /// representing an <see cref="Number"/> with an <see cref="Number.Value"/>
    /// property set to the computed result, or an <see cref="Error"/>
    /// from evaluating one of the expression operators
    /// </returns>
    /// <remarks>
    /// <b>Note</b>: Invoke
    /// <see cref="ParenthesesSubstitutionExtensions.SubstituteAllParentheses(Expression)"/>
    /// on the starting <see cref="Expression"/>
    /// before invoking this method or make sure there are no parentheses in
    /// <paramref name="expression"/>.
    /// </remarks>
    public static Result<Number> EvaluateAllOperators(this Expression expression) =>
        expression
                .EvaluateRightSideUnaryOperators()
                .Map(simplifiedExpression => simplifiedExpression
                    .EvaluateLeftSideUnaryOperators())
                .Map(simplifiedExpression => simplifiedExpression
                    .EvaluateRightAssociativeBinaryOperators<ExponentiationOperator>())
                .Map(simplifiedExpression => simplifiedExpression.EvaluateLeftAssociateBinaryOperators
                    <MultiplicationOperator, DivisionOperator, ModuloOperator>())
                .Map(simplifiedExpression => simplifiedExpression.EvaluateLeftAssociateBinaryOperators
                    <AdditionOperator, SubtractionOperator>())
                .Map(value => value.Parts is
                    [Number operand]
                    ? operand.ToResult()
                    : new ExpressionNotEvaluated());
}
