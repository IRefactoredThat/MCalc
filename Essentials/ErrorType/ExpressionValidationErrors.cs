using Essentials.Calculator;

namespace Essentials.ErrorType;
/// <summary>
/// Represents a validation error. Occurs when <see cref="Expression"/> had no
/// symbols.
/// </summary>
public record EmptyExpression() :
    Error($"Provided expression was empty.");
/// <summary>
/// Represents a validation error. Occurs when <see cref="Expression"/> ends with an
/// invalid symbol.
/// </summary>
public record InvalidExpressionEnd() :
    Error($"Expression can only end with a number, ) or !.");
/// <summary>
/// Represents a validation error. Occurs when <see cref="Expression"/> had
/// misplaced or misused <paramref name="Operator"/>.
/// </summary>
/// <param name="Operator">The operator that was misplaced or misued.</param>
public record InvalidUnaryOperatorUse(IUnaryOperator Operator) :
    Error($"{Operator.GetType().Name} cannot be followed by binary operator.");
/// <summary>
/// Represents a validation error. Occurs when <see cref="Expression"/> had
/// misplaced or misused <paramref name="Operator"/>.
/// </summary>
/// <param name="Operator">The operator that was misplaced or misued.</param>
public record InvalidBinaryOperatorUse(IBinaryOperator Operator) :
    Error($@"Only a number or ) operator can precede the 
        {Operator.GetType().Name} operator.");
/// <summary>
/// Represents a validation error. Occurs when <see cref="Expression"/> had
/// misplaced or misused <paramref name="Operator"/>.
/// </summary>
/// <param name="Operator">The operator that was misplaced or misued.</param>
public record InvalidRightSideUnaryOperatorUse(IRightSideUnaryOperator Operator) :
    Error($"Number or expression must precede the {Operator.GetType().Name}.");
/// <summary>
/// Represents a validation error. Occurs when <see cref="Expression"/> had
/// misplaced or misused closed parenthesis.
/// </summary>
/// <param name="Operator">The operator that was misplaced or misued.</param>
public record InvalidGroupingEndOperatorUse() :
    Error($"Only a number or right side unary operator can precede the ) operator.");
/// <summary>
/// Represents a validation error. Occurs when open parentheses was directly
/// followed by closed parentheses in <see cref="Expression"/>.
/// </summary>
public record ExpressionContainsEmptyGrouping() :
    Error($"One of the ( and ) operators had no expression inside.");
/// <summary>
/// Represents a validation error. Occurs when a open parentheses was not
/// matched with a corresponding closed parentheses in <see cref="Expression"/>.
/// </summary>
/// <param name="GroupingStartIndex"></param>
public record SuitableGroupingEndOperatorNotFound(int GroupingStartIndex) :
    Error(@$"Operator ( at index {GroupingStartIndex} was not matched.");
/// <summary>
/// Represents a validation error. Occurs when, after substituting all parentheses
/// in <see cref="Expression"/>, one or more parentheses was not substituted.
/// </summary>
public record GroupingOperatorsRemainedInExpression() :
    Error($"One of the operators ( or ) was not matched.");