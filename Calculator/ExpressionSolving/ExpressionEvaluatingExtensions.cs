using Calculator.ExpressionComposition;
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
internal static class ExpressionEvaluatingExtensions
{
    /// <summary>
    /// Evaluates a <see cref="string"/>, trying to
    /// determine the result of the underlaying expression, 
    /// using the specified <see cref="AngleMode"/>.
    /// </summary>
    /// <param name="expression">The <see cref="string"/> to evaluate.</param>
    /// <param name="mode">The <see cref="AngleMode"/> to use.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="IOperand"/> either
    /// representing an <see cref="IOperand"/> with an <see cref="IOperand.Value"/>
    /// property set to the computed result, or an <see cref="Error"/> object.
    /// </returns>
    public static Result<IOperand> Evaluate(string expression, AngleMode mode) =>
        ExpressionParsingExtensions.Parse(expression, mode)
            .Map(expression => expression.SubstituteAllParentheses())
            .Map(expression => expression.EvaluateAllOperators());

    /// <summary>
    /// Evaluates a <see cref="string"/>, trying to
    /// determine the result of the underlaying expression, 
    /// using the specified <see cref="AngleMode"/>. Then, the result is rounded.
    /// </summary>
    /// <param name="expression">The <see cref="string"/> to evaluate.</param>
    /// <param name="mode">The <see cref="AngleMode"/> to use.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="double"/> either
    /// representing the rounded computed result, or an <see cref="Error"/> object.
    /// </returns>
    public static Result<double> EvaluateAndRound(string expression, AngleMode mode) =>
        Evaluate(expression, mode).Map(operand => 
            operand.SmartRound().ToResult());

    /// <summary>
    /// Evaluates all operators inside the <paramref name="expression"/> with no
    /// parentheses (denoted by <see cref="GroupingStartOperator"/> and 
    /// <see cref="GroupingEndOperator"/>) in order to determine its result.
    /// </summary>
    /// <param name="expression">The <see cref="Expression"/> for evaluation.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="IOperand"/> either
    /// representing an <see cref="IOperand"/> with an <see cref="IOperand.Value"/>
    /// property set to the computed result, or one <see cref="Error"/>
    /// from <see cref="LeftSideUnaryOperatorsEvaluatingExtensions
    /// .EvaluateLeftSideUnaryOperators(Expression)"/>,
    /// <see cref="RightSideUnaryOperatorsEvaluatingExtensions
    /// .EvaluateRightSideUnaryOperators(Expression)"/>
    /// or <see cref="BinaryOperatorEvaluatingExtensions
    /// .EvaluateBinaryOperators(Expression)"/> method.
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
                .Map(expression => expression
                    .EvaluateLeftSideUnaryOperators())
                .Map(expression => expression.EvaluateBinaryOperators
                    <ExponentiationOperator, NegativeExponentiationOperator>())
                .Map(expression => expression.EvaluateBinaryOperators
                    <MultiplicationOperator, DivisionOperator, ModuloOperator>())
                .Map(expression => expression.EvaluateBinaryOperators
                    <AdditionOperator, SubtractionOperator>())
                .Map(value => value.Parts is 
                      [IOperand operand] ? operand.ToResult() :
                      new ExpressionNotEvaluated());
}