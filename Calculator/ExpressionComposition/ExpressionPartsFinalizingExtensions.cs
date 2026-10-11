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
    /// inside of the <paramref name="expression"/> with a <see cref="MultiplicationOperator"/>
    /// followed by the <see cref="IExpressionPart"/> wrapped inside that instance.
    /// </summary>
    /// <param name="expression">The <see cref="Expression"/> to expand.</param>
    /// <returns>
    /// A new <see cref="Expression"/> containing the parts of the
    /// <paramref name="expression"/> with each instance of
    /// <see cref="ImplicitMultiplicationOperator"/> being replaced.
    /// </returns>
    public static Expression SubstituteImplicitMultiplications(this Expression expression)
    {
        var newExpression = Expression.Create(expression.Mode);

        foreach (var part in expression.Parts)
        {
            if (part is ImplicitMultiplicationOperator implicitMultiplication)
            {
                newExpression.AddPart(new MultiplicationOperator());
                newExpression.AddPart(implicitMultiplication.InnerPart);
            }
            else
            {
                newExpression.AddPart(part);
            }
        }

        return newExpression;
    }

    /// <summary>
    /// Expands each <see cref="IOperand"/> chain inside of the <paramref name="expression"/>
    /// by inserting <see cref="MultiplicationOperator"/>s in between
    /// <see cref="IOperand"/> values.
    /// </summary>
    /// <param name="expression">The <see cref="Expression"/> to expand.</param>
    /// <returns>
    /// A new <see cref="Expression"/> containing the parts of the
    /// <paramref name="expression"/> with explicit multiplication chains.
    /// </returns>
    public static Expression ExpandChainedOperands(this Expression expression)
    {
        var newExpression = Expression.Create(expression.Mode);

        foreach (var part in expression.Parts)
        {
            if (part is IOperand && newExpression.Parts.Count > 0 &&
                newExpression.Parts[^1] is IOperand)
            {
                newExpression.AddPart(new MultiplicationOperator());
            }

            newExpression.AddPart(part);
        }

        return newExpression;
    }

    /// <summary>
    /// Closes each signed number grouping right before any binary operator except the
    /// <see cref="ExponentiationOperator"/> that appears on the same nesting level as
    /// that grouping's start. A signed number grouping is a <see cref="GroupingStartOperator"/>
    /// followed by a <see cref="Number"/> with the value of zero and a
    /// <see cref="SubtractionOperator"/> or <see cref="AdditionOperator"/>.
    /// </summary>
    /// <param name="expression">The <see cref="Expression"/> to finalize.</param>
    /// <returns>
    /// A new <see cref="Expression"/> with each grouping closed
    /// before the binary operator that follows its operand.
    /// </returns>
    public static Expression CloseSignedNumberGroups(this Expression expression)
    {
        var oldParts = expression.Parts;
        var newParts = Expression.Create(expression.Mode);
        var groupings = new Stack<bool>();

        var index = 0;
        while (index < oldParts.Count)
        {
            var part = oldParts[index];

            switch (part)
            {
                case NegativeValueOperator:
                    newParts.AddPart(new GroupingStartOperator());
                    newParts.AddPart(new Number(0));
                    newParts.AddPart(new SubtractionOperator());
                    groupings.Push(true);
                    index++;
                    break;

                case PositiveValueOperator:
                    newParts.AddPart(new GroupingStartOperator());
                    newParts.AddPart(new Number(0));
                    newParts.AddPart(new AdditionOperator());
                    groupings.Push(true);
                    index++;
                    break;

                case GroupingStartOperator when index + 2 < oldParts.Count
                    && oldParts[index + 1] is Number { Value: 0 }
                    && oldParts[index + 2] is AdditionOperator or SubtractionOperator:
                    groupings.Push(true);
                    newParts.AddPart(oldParts[index]);
                    newParts.AddPart(oldParts[index + 1]);
                    newParts.AddPart(oldParts[index + 2]);
                    index += 3;
                    break;

                case GroupingStartOperator:
                    groupings.Push(false);
                    newParts.AddPart(part);
                    index++;
                    break;

                case GroupingEndOperator:
                    if (groupings.Count > 0)
                    {
                        groupings.Pop();
                    }
                    newParts.AddPart(part);
                    index++;
                    break;

                case AdditionOperator or SubtractionOperator or MultiplicationOperator
                    or DivisionOperator or ModuloOperator when
                    groupings.Count > 0 && groupings.Peek():
                    groupings.Pop();
                    newParts.AddPart(new GroupingEndOperator());
                    newParts.AddPart(part);
                    index++;
                    break;

                default:
                    newParts.AddPart(part);
                    index++;
                    break;
            }
        }

        return newParts;
    }
}
