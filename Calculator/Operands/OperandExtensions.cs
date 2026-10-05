using Essentials.Calculator;

namespace Calculator.Operands;

internal static class OperandExtensions
{
    /// <summary>
    /// Determines whether an <paramref name="operand"/> has defined factorial.
    /// </summary>
    /// <param name="operand">The <see cref="IOperand"/> used for check.</param>
    /// <returns>
    /// A <see cref="bool"/> value indicating whether <see cref="IOperand.Value"/>
    /// is a whole non-negative integer.
    /// </returns>
    public static bool FactorialDefined(this IOperand operand)
    {
        var value = operand.Value;
        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            return false;
        }
        return !(value < 0 && double.IsInteger(value));
    }

    private static readonly double[] LanczosGammaCoefficients =
    [
        0.99999999999980993, 676.5203681218851, -1259.1392167224028,
        771.32342877765313, -176.61502916214059, 12.507343278686905,
        -0.13857109526572012, 9.9843695780195716e-6, 1.5056327351493116e-7
    ];

    private static double Gamma(double z)
    {
        if (z < 0.5)
        {
            return double.Pi / (double.Sin(Math.PI * z) * Gamma(1 - z));
        }

        z -= 1;

        double x = LanczosGammaCoefficients[0];

        for (int i = 1; i < LanczosGammaCoefficients.Length; i++)
        {
            x += LanczosGammaCoefficients[i] / (z + i);
        }

        double t = z + 7.5;

        return double.Sqrt(2 * double.Pi)
               * double.Pow(t, z + 0.5)
               * double.Exp(-t)
               * x;
    }

    /// <summary>
    /// Calculates the factorial of <paramref name="operand"/>, which can't be a negative integer.
    /// </summary>
    /// <param name="operand">The <see cref="IOperand"/> used for calculation.</param>
    /// <returns>
    /// A <see cref="IOperand"/> value representing the factorial
    /// of <paramref name="operand"/>.
    /// </returns>
    public static IOperand GetFactorial(this double operand)
    {
        if (double.IsInteger(operand) && operand >= 0)
        {
            double result = 1;
            for (double i = 2; i <= operand; i++)
            {
                result *= i;
            }

            return new Number(result);
        }
        return new Number(Gamma(operand + 1));
    }

    /// <summary>
    /// Determines whether <see cref="IOperand.Value"/> is too big to
    /// calculate factorial for.
    /// </summary>
    /// <param name="operand">The <see cref="IOperand"/> used for check.</param>
    /// <returns>
    /// A <see cref="bool"/> value indicating whether factorial of
    /// <paramref name="operand"/> can be calculated without overflow.
    /// </returns>
    public static bool FactorialOverflow(this IOperand operand) => operand.Value > 170;

    /// <summary>
    /// Calculates <paramref name="left"/> % <paramref name="right"/>, using
    /// mathematical standard.
    /// </summary>
    /// <param name="left">Left side of operation.</param>
    /// <param name="right">Right side of operation.</param>
    /// <returns>
    /// An <see cref="IOperand"/> representing the result of operation.
    /// </returns>
    public static IOperand Modulo(this IOperand left, IOperand right) =>
        right.Value == 0 ? left :
            new Number(left.Value - right.Value * double.Floor(left.Value / right.Value));

    public const double Tolerance = 1e-15;
    private static readonly int DigitCap = (int)-Math.Log10(Tolerance);

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
        if (Math.Abs(operand.Value) < Tolerance)
        {
            return 0;
        }

        var scale = 1;

        for (var digits = 0; digits <= DigitCap; digits++)
        {
            var rounded = double.Round(operand.Value * scale) / scale;
            if (double.Abs(operand.Value - rounded) <= Tolerance * double.Max(1, double.Abs(operand.Value)))
            {
                if (Math.Abs(rounded) < Tolerance)
                {
                    return 0;
                }
                return rounded;
            }
            scale *= 10;
        }

        return operand.Value;
    }
}
