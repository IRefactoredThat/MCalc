using System.Collections.Immutable;
using Calculator.ExpressionPartConversions;
using Calculator.Operators;
using Essentials.Calculator;
using Essentials.ErrorType;
using Essentials.ImmutableList;
using Essentials.Linq;
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
    public static Result<Expression> SubstituteAllParentheses(this Expression expression)
    {
        return expression.Parts.Aggregate
            (expression.ToResult(),

            (newExpression, _) => newExpression.Map(currentExpression =>
                currentExpression.RemoveRedundantParentheses()
                    .SubstituteParentheses()),

            resultExpression => resultExpression.SelfMap(finalExpression =>
                finalExpression.Parts.Any(part =>
                    part is GroupingStartOperator or GroupingEndOperator)
                    ? new GroupingOperatorsRemainedInExpression() : finalExpression)
        );
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

        var newParts = neededGroupingStartOperators switch
        {
            >0 => expression.Parts.InsertRange(0, Enumerable.Repeat<IExpressionPart>(
                new GroupingStartOperator(), neededGroupingStartOperators)),
            <0 => expression.Parts.AddRange(Enumerable.Repeat<IExpressionPart>(
                new GroupingEndOperator(), -neededGroupingStartOperators)),
            _ => expression.Parts
        };

        return newParts.AttachAngleMode(expression.Mode);
    }

    /// <summary>
    /// Substitutes each pair of parentheses (denoted by <see cref="GroupingStartOperator"/>
    /// and <see cref="GroupingEndOperator"/>) in the <paramref name="expression"/>
    /// with a new <see cref="Expression"/> object and then invokes
    /// <see cref="SubstituteAllParentheses(Expression)"/> on each newly created
    /// <see cref="Expression"/> to achieve full substitution.
    /// </summary>
    /// <param name="expression">The <see cref="Expression"/> object used for 
    /// substitution.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="Expression"/> either representing
    /// a new <see cref="Expression"/> with no parentheses or one <see cref="Error"/> listed
    /// here: <see cref="SuitableGroupingEndOperatorNotFound"/>.
    /// </returns>
    private static Result<Expression> SubstituteParentheses(this Expression expression)
    {
        var groupingStartIndex = expression.Parts
            .FindIndex(part => part is GroupingStartOperator);

        if (groupingStartIndex == -1)
        {
            return expression;
        }

        var groupingEndIndex = expression.MatchingGroupingOperatorIndex(groupingStartIndex);
        if (groupingEndIndex == -1)
        {
            return new SuitableGroupingEndOperatorNotFound();
        }

        var partsBefore = expression.Parts
                .Spread(..groupingStartIndex);
        var partsAfter = expression.Parts
                .Spread((groupingEndIndex + 1)..);

        var innerExpressionResult = expression.Parts
            .Spread((groupingStartIndex + 1)..groupingEndIndex)
            .AttachAngleMode(expression.Mode)
            .SubstituteAllParentheses();

        return innerExpressionResult.Map(value =>
            {
                ImmutableList<IExpressionPart> newParts = 
                    [..partsBefore, 
                    value.Parts is [IOperand operand] ? operand : value, 
                    ..partsAfter];

                return newParts
                    .AttachAngleMode(expression.Mode)
                    .ToResult();
            });
    }

    /// <summary>
    /// Removes each pair of parentheses (denoted by <see cref="GroupingStartOperator"/>
    /// and <see cref="GroupingEndOperator"/>) that do not affect the order 
    /// of operations inside <paramref name="expression"/>.
    /// </summary>
    /// <param name="expression">The <see cref="Expression"/> objects 
    /// used for removal.</param>
    /// <returns>
    /// A new <see cref="Expression"/> object with only necessary parentheses
    /// remaining in the <paramref name="expression"/>.
    /// </returns>
    private static Expression RemoveRedundantParentheses(this Expression expression)
    {
        var finalIndex = expression.Parts
                    .AggregateWhile(0, (index, parts) => index + 1,
                    index => index < expression.Parts.Count / 2 &&
                    expression.Parts[index] is GroupingStartOperator &&
                    expression.MatchingGroupingOperatorIndex(index) ==
                    expression.Parts.Count - 1 - index
            );

        return expression.Parts
            .Spread(finalIndex..^finalIndex)
            .AttachAngleMode(expression.Mode);
    }

    /// <summary>
    /// Finds a matching end index (index of <see cref="GroupingEndOperator"/> 
    /// in <paramref name="expression"/>) for <paramref name="startingIndex"/>
    /// (index of <see cref="GroupingStartOperator"/> in <paramref name="expression"/>).
    /// </summary>
    /// <param name="expression">The <see cref="Expression"/> that is searched.</param>
    /// <param name="startingIndex">Index of <see cref="GroupingStartOperator"/>.</param>
    /// <returns>
    /// A corresponding end index for <paramref name="startingIndex"/> or -1, if such
    /// does not exist.
    /// </returns>
    private static int MatchingGroupingOperatorIndex
        (this Expression expression, int startingIndex)
    {
        var indexedIndicator = expression.Parts
            .Skip(startingIndex + 1) // Skip everything before (
            .AggregateWhile(
                (Indicator: 1, Index: startingIndex),
                (accumulator, part) => accumulator switch
                {
                    var (indicator, index) when part
                    is GroupingStartOperator => (indicator + 1, index + 1),
                    var (indicator, index) when part
                    is GroupingEndOperator => (indicator - 1, index + 1),
                    _ => (accumulator.Indicator, accumulator.Index + 1)
                },
                acc => acc.Indicator != 0
            );

        return indexedIndicator.Indicator != 0 ? -1 : indexedIndicator.Index;
    }
}