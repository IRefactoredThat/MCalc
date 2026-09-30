using Essentials.Calculator;

namespace Essentials.ErrorType;

/// <summary>
/// Represents an implementation error. 
/// Occurs when operation including <paramref name="Operator"/> was not defined.
/// </summary>
/// <param name="Operator"></param>
public record UndefinedOperation(IOperator Operator) 
    : Error($"Operation undefined for {Operator.GetType().Name}.");
/// <summary>
/// Represents an implementation error. 
/// Occurs when <see cref="Expression"/> was not evaluated.
/// </summary>
public record ExpressionNotEvaluated() : Error("Expression was not successfully evaluated.");