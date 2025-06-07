using Essentials.Calculator;

namespace Essentials.ErrorType;

/// <summary>
/// Represents a parsing error. Occurs when, while <see cref="Expression"/>
/// was parsed, invalid symbols were identified.
/// </summary>
public record InvalidSymbolsInExpression() 
    : Error("Expression contains one or more symbols that are not valid.");
/// <summary>
/// Represents an implementation error. 
/// Occurs when <see cref="Expression"/> or inner expression was not evaluated.
/// </summary>
public record ExpressionNotEvaluated()
    : Error("Expression was not successfully evaluated.");