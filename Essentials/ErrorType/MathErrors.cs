using Essentials.Calculator;

namespace Essentials.ErrorType;
/// <summary>
/// Represents a math error. Occurs when applying <see cref="Operator"/> to operand 
/// resulted in an overflow.
/// </summary>
/// <param name="Operator"></param>
public record NumberOverflow(OperatorToken Operator) : Error($"Overflow after {Operator.Value}.");
/// <summary>
/// Represents a math error. Occurs when applying <see cref="Operator"/> to operand
/// was not possible because argument was invalid.
/// </summary>
/// <param name="Operator"></param>
public record InvalidOperation(OperatorToken Operator) : Error($"Invalid argument for {Operator.Value}.");