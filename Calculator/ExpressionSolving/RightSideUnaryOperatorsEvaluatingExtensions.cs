using System.Collections.Immutable;
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
    /// A new <see cref="Expression"/> object formed by evaluating 
    /// <see cref="IRightSideUnaryOperator"/> instances in the 
    /// <paramref name="expression"/> or one <see cref="Error"/> from 
    /// <see cref="UnaryOperatorsExtensions.Apply(IUnaryOperator, IOperand, AngleMode)"/>
    /// method.
    /// </returns>
    public static Result<Expression> EvaluateRightSideUnaryOperators(this Expression expression)
    {
        var resultParts = expression.Parts
            .Aggregate(
                (ImmutableList<IExpressionPart>.Empty.ToResult(), new EmptyPart().ToResult<IExpressionPart>()),
                ((Result<ImmutableList<IExpressionPart>> simplifiedParts, Result<IExpressionPart> lastPart) info,
                    IExpressionPart part) =>
                {
                    var newParts = info.simplifiedParts.Map(parts =>
                        part switch
                        {
                            IRightSideUnaryOperator unaryOperator => info.lastPart
                                .Map(last =>
                                    last switch
                                    {
                                        Expression innerExpression => parts.Evaluate(innerExpression.EvaluateAllOperators(),
                                            unaryOperator, innerExpression.Mode),
                                        IOperand operand => parts.Evaluate(operand.ToResult(), unaryOperator,
                                            expression.Mode),
                                        _ => new InvalidRightSideUnaryOperatorUse(unaryOperator.ToToken())
                                    }),
                            _ => parts.Add(part).ToResult()
                        });

                    var newLastPart = newParts.Map(parts => parts.Count > 0
                        ? parts[^1].ToResult()
                        : new InvalidRightSideUnaryOperatorUse((part as IRightSideUnaryOperator)!.ToToken()));

                    return (newParts, newLastPart);
                });

        return resultParts.Item1.Map(parts =>
            parts.AttachAngleMode(expression.Mode).ToResult());
    }

    /// <summary>
    /// Applies all <see cref="IRightSideUnaryOperator"/> instances inside of the 
    /// <paramref name="parts"/> list from its start (since aggregation works on a queue
    /// principle) to the <paramref name="operand"/>.
    /// </summary>
    /// <param name="parts">The <see cref="Expression"/> used for evaluation.</param>
    /// <param name="operand">The <see cref="Result{T}"/> of type <see cref="IOperand"/>
    /// either representing a <see cref="IOperand"/> instance used in 
    /// <see cref="IRightSideUnaryOperator"/> application, or an <see cref="Error"/> from
    /// <see cref="ExpressionEvaluatingExtensions.EvaluateAllOperators(Expression)"/>
    /// .</param>
    /// <param name="unaryOperator">The <see cref="IRightSideUnaryOperator"/> to apply.</param>
    /// <param name="mode">The <see cref="AngleMode"/> used for trigonometric 
    /// functions.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="ImmutableList{T}"/> of type
    /// <see cref="IExpressionPart"/> either representing a new list where the
    /// computed value after evaluation is wrapped in a new <see cref="IOperand"/> instance
    /// and list's portion is substituted with this value or an <see cref="Error"/> from 
    /// <see cref="UnaryOperatorsExtensions.Apply(IUnaryOperator, IOperand, AngleMode)"/>.
    /// </returns>
    private static Result<ImmutableList<IExpressionPart>> Evaluate
    (this ImmutableList<IExpressionPart> parts,
        Result<IOperand> operand, IRightSideUnaryOperator unaryOperator, AngleMode mode)
        => operand.Map(currentOperand => unaryOperator.Apply(currentOperand, mode))
            .Map(currentOperand => parts.SetItem(parts.Count - 1, currentOperand).ToResult());
}