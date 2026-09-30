using System.Collections.Immutable;
using Calculator.Operands;
using Calculator.Operators;
using Essentials.Calculator;

namespace Calculator.ExpressionComposition;

/// <summary>
/// Represents an extension class containing methods that are supposed to be executed
/// before an <see cref="Expression"/> is evaluated.
/// </summary>
internal static class ExpressionPartsFinalizingExtensions
{
    /// <summary>
    /// Substitutes each instance of <see cref="ImplicitMultiplicationOperator"/>
    /// inside of <paramref name="parts"/> sequence
    /// with a <see cref="MultiplicationOperator"/> followed by <see cref="IExpressionPart"/>
    /// wrapped inside that instance. 
    /// </summary>
    /// <param name="parts">The sequence to expand.</param>
    /// <returns>
    /// A <see cref="IEnumerable{T}"/> of type <see cref="IExpressionPart"/>
    /// with each instance of <see cref="ImplicitMultiplicationOperator"/> being replaced.
    /// </returns>
    public static IEnumerable<IExpressionPart> SubstituteImplicitMultiplications(
        this IEnumerable<IExpressionPart> parts) =>
            parts.SelectMany<IExpressionPart, IExpressionPart>(part =>
                part is ImplicitMultiplicationOperator({ } innerPart)
                    ? [new MultiplicationOperator(), innerPart] : [part]
            );

    /// <summary>
    /// Evaluates each <see cref="IOperand"/> chain inside of <paramref name="parts"/> 
    /// sequence by multiplying <see cref="IOperand"/> values, effectively removing
    /// all constants from sequence and converting these chains into single
    /// <see cref="Number"/> instances.
    /// </summary>
    /// <param name="parts">The <see cref="ImmutableArray"/> of type <see cref="IExpressionPart"/> to evaluate.</param>
    /// <returns>
    /// A new <see cref="ImmutableList{T}"/> of type <see cref="IExpressionPart"/> 
    /// with all constants evaluated.
    /// </returns>
    public static ImmutableList<IExpressionPart> EvaluateChainedOperands(
        this ImmutableArray<IExpressionPart> parts) =>
        parts.IsDefaultOrEmpty ? ImmutableList<IExpressionPart>.Empty : parts[1..]
             .Aggregate(ImmutableList.Create(parts[0]), (newParts, part) =>
                (part, newParts[^1]) switch
                {
                     (IOperand first, IOperand second) => newParts.SetItem
                        (newParts.Count - 1, new Number(first.Value * second.Value)),
                     _ => newParts.Add(part)
                });
}