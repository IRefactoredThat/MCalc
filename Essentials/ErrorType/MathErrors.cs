using Essentials.Calculator;

namespace Essentials.ErrorType;
/// <summary>
/// Represents a math error. Occurs when applying <see cref="Operator"/> to operand 
/// resulted in an overflow.
/// </summary>
/// <param name="Operator"></param>
public record NumberOverflow(IOperator Operator)
    : Error($"Operation involving {Operator.GetType().Name} resulted in an overflow.");
/// <summary>
/// Represents a math error. Occurs when applying <see cref="Operator"/> to operand
/// was not possible because argument was invalid.
/// </summary>
/// <param name="Operator"></param>
public record InvalidOperation(IOperator Operator)
    : Error($"Invalid argument passed to the {Operator.GetType().Name}.");