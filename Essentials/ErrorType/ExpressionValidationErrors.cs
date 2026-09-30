using Essentials.Calculator;

namespace Essentials.ErrorType;
/// <summary>
/// Represents a validation error. Occurs when number from <see cref="Expression"/> couldn't be parsed
/// due to improper format.
/// </summary>
public record NumberFormatError() : Error("Invalid number format.");
/// <summary>
/// Represents a validation error. Occurs when <see cref="Expression.Parts"/> is empty.
/// </summary>
public record EmptyExpression() : Error("No expression.");
/// <summary>
/// Represents a validation error. Occurs when <see cref="Expression"/> ends with an
/// invalid symbol.
/// </summary>
public record InvalidExpressionEnd() : Error("Invalid expression end.");
/// <summary>
/// Represents a validation error. Occurs when <see cref="Expression"/> had
/// misplaced or misused <paramref name="Operator"/>.
/// </summary>
/// <param name="Operator">The operator that was misplaced or misused.</param>
public record InvalidUnaryOperatorUse(OperatorToken Operator) :
    Error($"Invalid use of {Operator.Value}.");
/// <summary>
/// Represents a validation error. Occurs when <see cref="Expression"/> had
/// misplaced or misused <paramref name="Operator"/>.
/// </summary>
/// <param name="Operator">The operator that was misplaced or misused.</param>
public record InvalidBinaryOperatorUse(OperatorToken Operator) :
    Error($"Invalid use of {Operator.Value}.");
/// <summary>
/// Represents a validation error. Occurs when <see cref="Expression"/> had
/// misplaced or misused <paramref name="Operator"/>.
/// </summary>
/// <param name="Operator">The operator that was misplaced or misused.</param>
public record InvalidRightSideUnaryOperatorUse(OperatorToken Operator) :
    Error($"Invalid use of {Operator.Value}.");
/// <summary>
/// Represents a validation error. Occurs when <see cref="Expression"/> had
/// misplaced or misused closed parenthesis.
/// </summary>
public record InvalidGroupingEndOperatorUse() : Error("Invalid use of ).");
/// <summary>
/// Represents a validation error. Occurs when open parentheses was directly
/// followed by closed parentheses in <see cref="Expression"/>.
/// </summary>
public record ExpressionContainsEmptyGrouping() : Error("Empty pair ().");
/// <summary>
/// Represents a validation error. Occurs when a open parentheses was not
/// matched with a corresponding closed parentheses in <see cref="Expression"/>.
/// </summary>
public record SuitableGroupingEndOperatorNotFound() : Error("No matching ) was found");
/// <summary>
/// Represents a validation error. Occurs when, after substituting all parentheses
/// in <see cref="Expression"/>, one or more parentheses was not substituted.
/// </summary>
public record GroupingOperatorsRemainedInExpression() : Error("() remained in expression.");
/// <summary>
/// Represents an application error. Occurs when, upon clicking '=', expression
/// had a number with too many digits.
/// </summary>
public record TooLongNumberInExpression() : Error("Number is too long.");