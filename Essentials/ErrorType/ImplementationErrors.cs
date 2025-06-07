using Essentials.Calculator;

namespace Essentials.ErrorType;

/// <summary>
/// Represents an implementation error. 
/// Occurs when operation including <paramref name="Operator"/> was not defined.
/// </summary>
/// <param name="Operator"></param>
public record UndefinedOperation(IOperator Operator) 
    : Error($"Operation with the {Operator.GetType().Name} was not defined.");
/// <summary>
/// Represents an implementation error.
/// Occurs when a mapping from <paramref name="Symbol"/> to 
/// <see cref="IExpressionPart"/> was not defined.
/// </summary>
/// <param name="Symbol"></param>
public record UndefinedExpressionPartMapping(string Symbol)
    : Error($"Identifed expression part: {Symbol} was not valid or not defined.");