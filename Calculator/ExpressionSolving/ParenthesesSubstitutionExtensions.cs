using Calculator.Operators;
using Essentials.Calculator;
using Essentials.ErrorType;
using Essentials.ResultType;

namespace Calculator.ExpressionSolving;

/// <summary>
/// Represents an extension class containing methods for identifying and substituting
/// <see cref="IGroupingOperator"/> instances in an <see cref="Expression"/>.
/// </summary>
internal static class ParenthesesSubstitutionExtensions
{
    /// <summary>
    /// Substitutes each pair or parentheses (denoted by <see cref="GroupingStartOperator"/>
    /// and <see cref="GroupingEndOperator"/>) inside of <paramref name="expression"/>
    /// with a new <see cref="Expression"/> object, allowing for separate evaluation
    /// of these inner expressions. This method allows for <see cref="Expression.Parts"/>
    /// property of any inner expression to have an <see cref="Expression"/> object as
    /// its part, including the starting <paramref name="expression"/>.
    /// </summary>
    /// <param name="expression">The <see cref="Expression"/> object used for
    /// substitution.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="Expression"/> either representing
    /// a new <see cref="Expression"/> with no parentheses or one <see cref="Error"/> listed
    /// here: <see cref="GroupingOperatorsRemainedInExpression"/> |
    /// <see cref="SuitableGroupingEndOperatorNotFound"/>
    /// </returns>
    /// <remarks>
    /// <b>Note</b>: This method evaluates each part a constant number of times, so
    /// substituting <c>n</c> parts runs in <c>O(n)</c>.
    /// </remarks>
    public static Result<Expression> SubstituteAllParentheses(this Expression expression)
    {
        var parts = expression.Parts;
        return SubstituteRange(parts, MatchGroupingOperators(parts), expression.Mode, 0, parts.Count);
    }

    /// <summary>
    /// Match all parentheses (denoted by <see cref="GroupingStartOperator"/>
    /// and <see cref="GroupingEndOperator"/>) in the <paramref name="expression"/> by
    /// prepending the necessary <see cref="GroupingStartOperator"/>s and appending
    /// the necessary <see cref="GroupingEndOperator"/>s for expression to have
    /// the same amount of each, making it possibly evaluatable.
    /// </summary>
    /// <param name="expression">The <see cref="Expression"/> object used for matching.</param>
    /// <returns>An <see cref="Expression"/> object with possibly well-formed parentheses.</returns>
    public static Expression MatchAllParentheses(this Expression expression)
    {
        var neededGroupingStartOperators = expression.Parts
            .Aggregate(0, (count, part) =>
                part switch
                {
                    GroupingStartOperator => count - 1,
                    GroupingEndOperator => count + 1,
                    _ => count
                });

        var newExpression = Expression.Create(expression.Mode);

        for (var i = 0; i < neededGroupingStartOperators; i++)
        {
            newExpression.AddPart(new GroupingStartOperator());
        }

        foreach (var part in expression.Parts)
        {
            newExpression.AddPart(part);
        }

        for (var i = 0; i < -neededGroupingStartOperators; i++)
        {
            newExpression.AddPart(new GroupingEndOperator());
        }

        return newExpression;
    }

    /// <summary>
    /// Substitutes every parenthesis inside of the range <c>[start, end)</c> of
    /// <paramref name="parts"/> with a nested <see cref="Expression"/>,
    /// after stripping the parentheses that wrap the whole range.
    /// </summary>
    /// <param name="parts">The parts that are being substituted.</param>
    /// <param name="matches">
    /// The precomputed index of the grouping operator that matches each grouping
    /// operator in <paramref name="parts"/>, or -1 when there is none.
    /// </param>
    /// <param name="mode">The <see cref="AngleMode"/> of the produced expressions.</param>
    /// <param name="start">The inclusive index at which the range begins.</param>
    /// <param name="end">The exclusive index at which the range ends.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="Expression"/> representing
    /// the substituted range or one <see cref="Error"/> listed here:
    /// <see cref="GroupingOperatorsRemainedInExpression"/> |
    /// <see cref="SuitableGroupingEndOperatorNotFound"/>.
    /// </returns>
    private static Result<Expression> SubstituteRange(
        IReadOnlyList<IExpressionPart> parts,
        int[] matches,
        AngleMode mode,
        int start,
        int end)
    {
        // Remove outer parentheses
        while (start < end &&
            parts[start] is GroupingStartOperator &&
            matches[start] == end - 1)
        {
            start++;
            end--;
        }

        var newExpression = Expression.Create(mode);
        var result = newExpression.ToResult();
        var index = start;

        while (index < end)
        {
            if (parts[index] is GroupingStartOperator)
            {
                var groupingEndIndex = matches[index];
                if (groupingEndIndex == -1 || groupingEndIndex >= end)
                {
                    return new SuitableGroupingEndOperatorNotFound();
                }

                var innerResult = SubstituteRange(parts, matches, mode, index + 1, groupingEndIndex);

                // Try to collapse the group to a single operand
                result = result.Map(current => innerResult.Map(value =>
                {
                    current.AddPart(value.Parts is [IOperand operand] ? operand : value);
                    return current.ToResult();
                }));
                index = groupingEndIndex + 1;
                continue;
            }

            newExpression.AddPart(parts[index]);
            index++;
        }

        return result.Map(current => current.Parts.Any(part =>
            part is GroupingStartOperator or GroupingEndOperator)
                ? new GroupingOperatorsRemainedInExpression()
                : current.ToResult());
    }

    /// <summary>
    /// Computes the matching grouping operator index for every grouping operator in
    /// <paramref name="parts"/> in a single pass using a stack.
    /// </summary>
    /// <param name="parts">The parts whose grouping operators are matched.</param>
    /// <returns>
    /// An array where the entry for a grouping operator is the index of its matching
    /// operator, or -1 when it has none.
    /// </returns>
    private static int[] MatchGroupingOperators(IReadOnlyList<IExpressionPart> parts)
    {
        var matches = new int[parts.Count];
        for (var i = 0; i < matches.Length; i++)
        {
            matches[i] = -1;
        }

        var openGroupings = new Stack<int>();

        for (var i = 0; i < parts.Count; i++)
        {
            switch (parts[i])
            {
                case GroupingStartOperator:
                    openGroupings.Push(i);
                    break;

                case GroupingEndOperator when openGroupings.Count > 0:
                    var startIndex = openGroupings.Pop();
                    matches[startIndex] = i;
                    matches[i] = startIndex;
                    break;
            }
        }

        return matches;
    }
}
