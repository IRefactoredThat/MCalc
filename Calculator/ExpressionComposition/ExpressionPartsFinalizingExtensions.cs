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
    /// Expands each <see cref="IOperand"/> chain inside of <paramref name="parts"/>
    /// sequence by inserting <see cref="MultiplicationOperator"/>s in between
    /// <see cref="IOperand"/> values.
    /// </summary>
    /// <param name="parts">The <see cref="ImmutableArray"/> of type <see cref="IExpressionPart"/> to expand.</param>
    /// <returns>
    /// A new <see cref="ImmutableList{T}"/> of type <see cref="IExpressionPart"/>
    /// with explicit multiplication chains.
    /// </returns>
    public static ImmutableList<IExpressionPart> ExpandChainedOperands(
        this ImmutableArray<IExpressionPart> parts) =>
        parts.IsDefaultOrEmpty ? ImmutableList<IExpressionPart>.Empty : parts[1..]
            .Aggregate(ImmutableList.Create(parts[0]), (newParts, part) =>
                (part, newParts[^1]) switch
                {
                     (IOperand operand, IOperand) => newParts
                         .Add(new MultiplicationOperator()).Add(operand),
                     var (anyPart, _) => newParts.Add(anyPart)
                });

    /// <summary>
    /// Closes each signed number grouping right before any binary operator except the
    /// <see cref="ExponentiationOperator"/> that appears on the same nesting level as
    /// that grouping's start. A signed number grouping is a <see cref="GroupingStartOperator"/>
    /// followed by a <see cref="Number"/> with the value of zero and a
    /// <see cref="SubtractionOperator"/> or <see cref="AdditionOperator"/>.
    /// </summary>
    /// <param name="parts">The <see cref="ImmutableArray"/> of type <see cref="IExpressionPart"/> to finalize.</param>
    /// <returns>
    /// A new <see cref="ImmutableArray{T}"/> of type <see cref="IExpressionPart"/> with
    /// each grouping closed before the binary operator that follows its operand.
    /// </returns>
    public static ImmutableArray<IExpressionPart> CloseSignedNumberGroups(
        this ImmutableArray<IExpressionPart> parts)
    {
        var newParts = ImmutableArray.CreateBuilder<IExpressionPart>();
        var groupings = new Stack<bool>();

        var index = 0;
        while (index < parts.Length)
        {
            var part = parts[index];

            switch (part)
            {
                case NegativeValueOperator:
                    newParts.Add(new GroupingStartOperator());
                    newParts.Add(new Number(0));
                    newParts.Add(new SubtractionOperator());
                    groupings.Push(true);
                    index++;
                    break;

                case PositiveValueOperator:
                    newParts.Add(new GroupingStartOperator());
                    newParts.Add(new Number(0));
                    newParts.Add(new AdditionOperator());
                    groupings.Push(true);
                    index++;
                    break;

                case GroupingStartOperator when index + 2 < parts.Length
                    && parts[index + 1] is Number { Value: 0 }
                    && parts[index + 2] is AdditionOperator or SubtractionOperator:
                    groupings.Push(true);
                    newParts.Add(parts[index]);
                    newParts.Add(parts[index + 1]);
                    newParts.Add(parts[index + 2]);
                    index += 3;
                    break;

                case GroupingStartOperator:
                    groupings.Push(false);
                    newParts.Add(part);
                    index++;
                    break;

                case GroupingEndOperator:
                    if (groupings.Count > 0)
                    {
                        groupings.Pop();
                    }
                    newParts.Add(part);
                    index++;
                    break;

                case AdditionOperator or SubtractionOperator or MultiplicationOperator
                    or DivisionOperator or ModuloOperator when
                    groupings.Count > 0 && groupings.Peek():
                    groupings.Pop();
                    newParts.Add(new GroupingEndOperator());
                    newParts.Add(part);
                    index++;
                    break;

                default:
                    newParts.Add(part);
                    index++;
                    break;
            }
        }

        return newParts.ToImmutable();
    }
}
