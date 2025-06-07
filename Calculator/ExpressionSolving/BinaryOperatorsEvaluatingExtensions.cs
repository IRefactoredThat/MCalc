using Calculator.ExpressionPartConversions;
using Calculator.Operators;
using Essentials.Calculator;
using Essentials.ErrorType;
using Essentials.ResultType;
using Essentials.ImmutableList;
using System.Collections.Immutable;

namespace Calculator.ExpressionSolving;

/// <summary>
/// Represents an extension class containing methods for evaluating
/// <see cref="IBinaryOperator"/> instances in an <see cref="Expression"/>.
/// </summary>
internal static class BinaryOperatorEvaluatingExtensions
{
    /// <summary>
    /// Evaluates <typeparamref name="T1"/> and <typeparamref name="T2"/>
    /// inside of the <paramref name="expression"/> simultaneously.
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
    public static Result<Expression> EvaluateBinaryOperators<T1, T2>
        (this Expression expression)
        where T1 : IBinaryOperator
        where T2 : IBinaryOperator
        => expression.EvaluateBinaryOperators<T1, T2, T2>();

    /// <summary>
    /// Evaluates <typeparamref name="T1"/>, <typeparamref name="T2"/> 
    /// and <typeparamref name="T3"/> inside of the 
    /// <paramref name="expression"/> simultaneously.
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
    public static Result<Expression> EvaluateBinaryOperators<T1, T2, T3>
        (this Expression expression) 
        where T1 : IBinaryOperator 
        where T2 : IBinaryOperator
        where T3 : IBinaryOperator
    {
        var parts = expression.Parts
            .Aggregate(ImmutableList<IExpressionPart>.Empty.ToResult(),
                (simplifiedParts, current) => simplifiedParts.Map(parts =>
                {
                    if (parts.Count < 2)
                    {
                        return parts.Add(current);
                    }
                    var (penultimate, last) = (parts[^2], parts[^1]);

                    return (penultimate, last, current) switch
                    {
                        (IOperand first, T1 @operator, IOperand second) =>
                            parts.SimplifyPortion(@operator, first, second),

                        (IOperand first, T2 @operator, IOperand second) =>
                            parts.SimplifyPortion(@operator, first, second),

                        (IOperand first, T3 @operator, IOperand second) =>
                            parts.SimplifyPortion(@operator, first, second),

                        (IOperand first, T1 @operator, Expression second) =>
                            parts.SimplifyPortion(@operator, second, first),

                        (IOperand first, T2 @operator, Expression second) =>
                            parts.SimplifyPortion(@operator, second, first),

                        (IOperand first, T3 @operator, Expression second) =>
                            parts.SimplifyPortion(@operator, second, first),

                        (Expression first, T1 @operator, IOperand second) =>
                            parts.SimplifyPortion(@operator, first, second),

                        (Expression first, T2 @operator, IOperand second) =>
                            parts.SimplifyPortion(@operator, first, second),

                        (Expression first, T3 @operator, IOperand second) =>
                            parts.SimplifyPortion(@operator, first, second),

                        (Expression first, T1 @operator, Expression second) =>
                            parts.SimplifyPortion(@operator, first, second),

                        (Expression first, T2 @operator, Expression second) =>
                            parts.SimplifyPortion(@operator, first, second),

                        (Expression first, T3 @operator, Expression second) =>
                            parts.SimplifyPortion(@operator, first, second),

                        _ => parts.Add(current)
                    };
                })
            );

        return parts.Map(parts => parts
            .AttachAngleMode(expression.Mode)
            .ToResult());
    }

    /// <summary>
    /// Evaluates a portion of the <paramref name="parts"/> list by applying the
    /// binary <paramref name="operator"/> to the <paramref name="first"/> 
    /// and <paramref name="second"/> operands.
    /// </summary>
    /// <typeparam name="T">The type of the operator used in evaluation.</typeparam>
    /// <param name="parts">The list being evaluated.</param>
    /// <param name="binaryOperator">The binary operator applied to the operands.</param>
    /// <param name="first">The first operand in the evaluation.</param>
    /// <param name="second">The second operand in the evaluation.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> representing either a new <see cref="ImmutableList{T}"/>
    /// of type <see cref="IExpressionPart"/>, formed by evaluating the two operands
    /// and replacing the operation members with the computed result, or an 
    /// <see cref="Error"/> from the 
    /// <see cref="BinaryOperatorsExtensions.Apply(IBinaryOperator, IOperand, IOperand)"/>
    /// method.
    /// </returns>
    private static Result<ImmutableList<IExpressionPart>> SimplifyPortion<T>(this 
        ImmutableList<IExpressionPart> parts, T @operator, IOperand first, IOperand second)
        where T : IBinaryOperator => 
            @operator.Apply(first, second)
              .Map(number => parts.RemoveLast(2).Add(number).ToResult());

    /// <summary>
    /// Evaluates a portion of the <paramref name="parts"/> list by applying the
    /// binary <paramref name="operator"/> to the <paramref name="first"/> operand
    /// and the result of the <paramref name="second"/> expression.
    /// </summary>
    /// <typeparam name="T">The type of the operator used in evaluation.</typeparam>
    /// <param name="parts">The list being evaluated.</param>
    /// <param name="operator">The binary operator applied to the operands.</param>
    /// <param name="first">The first operand in the evaluation.</param>
    /// <param name="second">The second expression in the evaluation.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> representing either a new <see cref="ImmutableList{T}"/>
    /// of type <see cref="IExpressionPart"/>, formed by evaluating the two operands
    /// and replacing the operation members with the computed result, or an 
    /// <see cref="Error"/> from the 
    /// <see cref="BinaryOperatorsExtensions.Apply(IBinaryOperator, IOperand, IOperand)"/>
    /// method.
    /// </returns>
    private static Result<ImmutableList<IExpressionPart>> SimplifyPortion<T>(this
        ImmutableList<IExpressionPart> parts, T @operator, Expression first, 
        IOperand second) where T : IBinaryOperator =>
            first.EvaluateAllOperators()
              .Map(number => parts.SimplifyPortion(@operator, number, second));

    /// <summary>
    /// Evaluates a portion of the <paramref name="parts"/> list by applying the
    /// binary <paramref name="operator"/> to the result of the <paramref name="first"/>
    /// and the result of the <paramref name="second"/> expression.
    /// </summary>
    /// <typeparam name="T">The type of the operator used in evaluation.</typeparam>
    /// <param name="parts">The list being evaluated.</param>
    /// <param name="operator">The binary operator applied to the operands.</param>
    /// <param name="first">The first expression in the evaluation.</param>
    /// <param name="second">The second expression in the evaluation.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> representing either a new <see cref="ImmutableList{T}"/>
    /// of type <see cref="IExpressionPart"/>, formed by evaluating the two operands
    /// and replacing the operation members with the computed result, or an 
    /// <see cref="Error"/> from the 
    /// <see cref="BinaryOperatorsExtensions.Apply(IBinaryOperator, IOperand, IOperand)"/>
    /// method.
    /// </returns>
    private static Result<ImmutableList<IExpressionPart>> SimplifyPortion<T>(this
        ImmutableList<IExpressionPart> parts, T @operator, Expression first, 
        Expression second) where T : IBinaryOperator =>
            first.EvaluateAllOperators()
              .Map(firstNumber => second.EvaluateAllOperators()
                   .Map(secondNumber => parts.SimplifyPortion
                        (@operator, firstNumber, secondNumber)));
}