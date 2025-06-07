using Calculator.ExpressionPartConversions;
using Calculator.Operators;
using Essentials.Calculator;
using Essentials.ImmutableList;
using Essentials.ResultType;
using Essentials.ErrorType;
using System.Collections.Immutable;

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
    /// A new <see cref="Expression"/> object formed by evaluating 
    /// <see cref="ILeftSideUnaryOperator"/> instances in the 
    /// <paramref name="expression"/> or one <see cref="Error"/> from 
    /// <see cref="UnaryOperatorsExtensions.Apply(IUnaryOperator, IOperand, AngleMode)"/>
    /// method.
    /// </returns>
    public static Result<Expression> EvaluateLeftSideUnaryOperators
        (this Expression expression)
    {
        var resultParts = expression.Parts
            .Aggregate(ImmutableList<IExpressionPart>.Empty.ToResult(),
            (simplifiedParts, part) => simplifiedParts.Map(parts =>
                part switch
                {
                    Expression expression => parts.Evaluate
                        (expression.EvaluateAllOperators(), expression.Mode),
                    IOperand operand => parts.Evaluate(operand.ToResult(), expression.Mode),
                    var part => parts.Add(part)
                }
            ));

        return resultParts.Map(parts =>
                parts.AttachAngleMode(expression.Mode)
                .ToResult());
    }

    /// <summary>
    /// Applies all <see cref="ILeftSideUnaryOperator"/> instances inside of the 
    /// <paramref name="parts"/> list from its end (since aggregation works on a stack
    /// principle) to the <paramref name="operand"/>.
    /// </summary>
    /// <param name="parts">The <see cref="Expression"/> used for evaluation.</param>
    /// <param name="operand">The <see cref="Result{T}"/> of type <see cref="IOperand"/>
    /// either representing a <see cref="IOperand"/> instance used in 
    /// <see cref="ILeftSideUnaryOperator"/> application, or an <see cref="Error"/> from
    /// <see cref="ExpressionEvaluatingExtensions.EvaluateAllOperators(Expression)"/>
    /// .</param>
    /// <param name="mode">The <see cref="AngleMode"/> used for trigonometric 
    /// functions.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="ImmutableList{T}"/> of type
    /// <see cref="IExpressionPart"/> either representing a new list where the
    /// computed value after evaluation is wrapped in a new <see cref="IOperand"/> instance
    /// and added to the list's end or an <see cref="Error"/> from 
    /// <see cref="UnaryOperatorsExtensions.Apply(IUnaryOperator, IOperand, AngleMode)"/>.
    /// </returns>
    private static Result<ImmutableList<IExpressionPart>> Evaluate
        (this ImmutableList<IExpressionPart> parts, Result<IOperand> operand, 
        AngleMode mode)
    {
        var unaryOperatorsFromEnd = Enumerable.Reverse(parts)
            .TakeWhile(part => part is ILeftSideUnaryOperator)
            .OfType<ILeftSideUnaryOperator>()
            .ToImmutableList();

        var finalOperand = unaryOperatorsFromEnd.Aggregate(operand, 
            (currentOperand, @operator) => currentOperand.Map(operand => 
                @operator.Apply(operand, mode)));

        return finalOperand.Map(operand => 
            parts.RemoveLast(unaryOperatorsFromEnd.Count)
                 .Add(operand).ToResult());
    }
}