using Calculator.Operands;
using Calculator.Operators;
using Essentials.Calculator;
using Essentials.ErrorType;
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
    public static Result<double> EvaluateAndRound(Expression expression) =>
        expression.MatchAllParentheses().SubstituteAllParentheses()
            .Map(simplifiedExpression => simplifiedExpression.EvaluateAllOperators()).Map(operand =>
            operand.SmartRound().ToResult());

    /// <summary>
    /// Evaluates all operators inside the <paramref name="expression"/> with no
    /// parentheses in order to determine its result.
    /// </summary>
    /// <param name="expression">The <see cref="Expression"/> to evaluate.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="IOperand"/> either
    /// representing an <see cref="IOperand"/> with an <see cref="IOperand.Value"/>
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
    public static Result<IOperand> EvaluateAllOperators(this Expression expression) =>
        expression
                .SubstituteAllExponentiationOperators()
                .EvaluateRightSideUnaryOperators()
                .Map(simplifiedExpression => simplifiedExpression
                    .EvaluateLeftSideUnaryOperators())
                .Map(simplifiedExpression => simplifiedExpression.EvaluateBinaryOperators
                    <ExponentiationOperator, NegativeExponentiationOperator>())
                .Map(simplifiedExpression => simplifiedExpression.EvaluateBinaryOperators
                    <MultiplicationOperator, DivisionOperator, ModuloOperator>())
                .Map(simplifiedExpression => simplifiedExpression.EvaluateBinaryOperators
                    <AdditionOperator, SubtractionOperator>())
                .Map(value => value.Parts is
                    [IOperand operand]
                    ? operand.ToResult()
                    : new ExpressionNotEvaluated());
}