namespace Essentials.ErrorType;

/// <summary>
/// Represents an application error. Occurs when, upon clicking '=', expression
/// had a number with too many digits.
/// </summary>
public record TooLongNumberInExpression() : 
            Error("Numbers can have a maximum of 15 integer digits and " +
                "a maximum of 10 fractional digits.");