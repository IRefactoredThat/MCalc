using Essentials.Calculator;

using static System.Math;

namespace Calculator.Operands;

internal static class OperandExtensions
{
    /// <summary>
    /// Represents a value used as a rough zero.
    /// </summary>
    private const double RoughZero = 1e-15;
    /// <summary>
    /// Determines whether an <paramref name="operand"/> is roughly equal to zero.
    /// </summary>
    /// <param name="operand">The <see cref="double"/> used for check.</param>
    /// <returns>
    /// A <see cref="bool"/> value indicating whether <paramref name="operand"/>
    /// is close to zero.
    /// </returns>
    public static bool IsCloseToZero(this double operand) => 
        Abs(operand) <= RoughZero;

    /// <summary>
    /// Determines whether an <paramref name="exponent"/> is an even number 
    /// when written in root form.
    /// </summary>
    /// <param name="exponent">The <see cref="IOperand"/> representing exponent.</param>
    /// <returns>
    /// A <see cref="bool"/> value indicating whether <see cref="IOperand.Value"/>
    /// is an even number when written in root form.
    /// </returns>
    public static bool IsEvenRoot(this IOperand exponent)
    {
        var value = exponent.Value;
        var fractionalPart = value - Floor(value);

        if (fractionalPart.IsCloseToZero())
        {
            return false;
        }

        var root = 1 / fractionalPart;
        return (root % 2).IsCloseToZero();
    }

    /// <summary>
    /// Determines whether an <paramref name="operand"/> is a whole number 
    /// greater than or equal to zero.
    /// </summary>
    /// <param name="operand">The <see cref="IOperand"/> used for check.</param>
    /// <returns>
    /// A <see cref="bool"/> value indicating whether <see cref="IOperand.Value"/>
    /// is a whole non-negative integer.
    /// </returns>
    public static bool IsWholeNonNegativeInteger(this IOperand operand)
        => double.IsPositive(operand.Value) && double.IsInteger(operand.Value);

    /// <summary>
    /// Calculates the factorial of <see cref="IOperand.Value"/>.
    /// </summary>
    /// <param name="operand">The <see cref="IOperand"/> used for calculation.</param>
    /// <returns>
    /// A <see cref="double"/> value representing the factorial 
    /// of <paramref name="operand"/>.
    /// </returns>
    public static double GetFactorial(this IOperand operand)
        => Enumerable.Range(1, (int)operand.Value)
                .Aggregate(1d, (result, next) => result * next);

    /// <summary>
    /// Determines whether <see cref="IOperand.Value"/> is too big to 
    /// calculate factorial for.
    /// </summary>
    /// <param name="operand">The <see cref="IOperand"/> used for check.</param>
    /// <returns>
    /// A <see cref="bool"/> value indicating whether factorial of 
    /// <paramref name="operand"/> can be calculated without overflow.
    /// </returns>
    public static bool ExceedsMaximumOperandValueForFactorial(this IOperand operand)
        => operand.Value > 170;

    /// <summary>
    /// Calculates <paramref name="left"/> mod <paramref name="right"/>, using
    /// mathematical standard.
    /// </summary>
    /// <param name="left">Left side of operation.</param>
    /// <param name="right">Right side of operation.</param>
    /// <returns>
    /// An <see cref="IOperand"/> representing the result of operation.
    /// </returns>
    public static IOperand Modulo(this IOperand left, IOperand right)
    {
        if(right.Value == 0)
        {
            return left;
        }
        return new Number(left.Value - right.Value * Floor(left.Value / right.Value));
    }

    /// <summary>
    /// Rounds a <see cref="IOperand.Value"/> to the nearest double, or 
    /// returns the original <see cref="IOperand.Value"/> if rounding is not needed,
    /// to account for small calculation errors for expressions like (√2)^2
    /// to be equal to 2, sin(π/6) to be equal to 0.5 or 0.1 + 0.2 to be equal to 0.3.
    /// </summary>
    /// <param name="operand">The <see cref="IOperand"/> used for rounding.</param>
    /// <returns>A rounded or original <see cref="double"/> value.</returns>
    public static double SmartRound(this IOperand operand)
    {
        var roundedOrOriginalValue = Enumerable.Range(0, 16)
            .Select(digits =>
            {
                var roundedValue = Round(operand.Value, digits);
                return new
                {
                    RoundedValue = roundedValue,
                    IsRounded = Abs(operand.Value - roundedValue).IsCloseToZero()
                };
            })
            .FirstOrDefault(result => result.IsRounded)?.RoundedValue ?? operand.Value;

        return roundedOrOriginalValue == -0 ? 0 : roundedOrOriginalValue;
    }
}