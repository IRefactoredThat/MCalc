using Essentials.ResultType;
using Essentials.Calculator;
using Essentials.ErrorType;
using Calculator.Operands;
using static System.Math;

namespace Calculator.Operators;

/// <summary>
/// Represents an extension class containing a method to evaluate a unary operation.
/// </summary>
internal static class UnaryOperatorsExtensions
{
    /// <summary>
    /// Applies <paramref name="unaryOperator"/> to the <paramref name="operand"/>
    /// using <paramref name="mode"/> if necessary.
    /// </summary>
    /// <param name="unaryOperator">The <see cref="IUnaryOperator"/> applied.</param>
    /// <param name="operand">The <see cref="IOperand"/> instance.</param>
    /// <param name="mode">The <see cref="AngleMode"/> used.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="IOperand"/> either representing
    /// a new <see cref="IOperand"/> instance with <see cref="IOperand.Value"/> set to
    /// the computed result of the operation, or one <see cref="Error"/> listed here:
    /// <see cref="InvalidOperation"/> |
    /// <see cref="UndefinedOperation"/>.
    /// </returns>
    public static Result<IOperand> Apply(this IUnaryOperator unaryOperator, 
        IOperand operand, AngleMode mode)
    {
        var result = unaryOperator switch
        {
            ITrigonometricOperator trigonometricOperator =>
                trigonometricOperator.Apply(operand, mode),
            ILeftSideUnaryOperator leftSideUnaryOperator =>
                leftSideUnaryOperator.Apply(operand),
            IRightSideUnaryOperator rightSideUnaryOperator =>
                rightSideUnaryOperator.Apply(operand),

            _ => new UndefinedOperation(unaryOperator)
        };

        return result.Map(operand => operand.Value is
            double.NegativeInfinity or double.PositiveInfinity or double.NaN ?
            new InvalidOperation(unaryOperator) : operand.ToResult());
    }

    /// <summary>
    /// Applies <paramref name="unaryOperator"/> to the <paramref name="operand"/>.
    /// </summary>
    /// <param name="unaryOperator">The <see cref="ILeftSideUnaryOperator"/> 
    /// applied.</param>
    /// <param name="operand">The <see cref="IOperand"/> instance.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="IOperand"/> either representing
    /// a new <see cref="IOperand"/> instance with <see cref="IOperand.Value"/> set to
    /// the computed result of the operation, or one <see cref="Error"/> listed here:
    /// <see cref="UndefinedOperation"/>.
    /// </returns>
    private static Result<IOperand> Apply(this ILeftSideUnaryOperator unaryOperator,
        IOperand operand)
    {
        return unaryOperator switch
        {
            NegationOperator => new Number(-operand.Value),
            PositiveValueOperator => operand.ToResult(),

            SquareRootOperator => new Number(Sqrt(operand.Value)),
            CubeRootOperator => new Number(Cbrt(operand.Value)),

            LogarithmOperator => new Number(Log10(operand.Value)),
            NaturalLogarithmOperator => new Number(Log(operand.Value)),

            _ => new UndefinedOperation(unaryOperator),
        };
    }

    /// <summary>
    /// Applies <paramref name="unaryOperator"/> to the <paramref name="operand"/>.
    /// </summary>
    /// <param name="unaryOperator">The <see cref="IRightSideUnaryOperator"/> 
    /// applied.</param>
    /// <param name="operand">The <see cref="IOperand"/> instance.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="IOperand"/> either representing
    /// a new <see cref="IOperand"/> instance with <see cref="IOperand.Value"/> set to
    /// the computed result of the operation, or one <see cref="Error"/> listed here:
    /// <see cref="UndefinedOperation"/> | <see cref="NumberOverflow"/>.
    /// </returns>
    private static Result<IOperand> Apply(this IRightSideUnaryOperator unaryOperator,
        IOperand operand)
    {
        return unaryOperator switch
        {
            FactorialOperator when operand.ExceedsMaximumOperandValueForFactorial()
                => new NumberOverflow(unaryOperator),
            FactorialOperator when operand.IsWholeNonNegativeInteger()
                => new Number(operand.GetFactorial()),

            _ => new UndefinedOperation(unaryOperator),
        };
    }

    /// <summary>
    /// Applies <paramref name="trigonometricOperator"/> to the <paramref name="operand"/>
    /// using <paramref name="mode"/>.
    /// </summary>
    /// <param name="trigonometricOperator">The <see cref="ITrigonometricOperator"/>
    /// applied.</param>
    /// <param name="operand">The <see cref="IOperand"/> instance.</param>
    /// <param name="mode">The <see cref="AngleMode"/> used.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="IOperand"/> either representing
    /// a new <see cref="IOperand"/> instance with <see cref="IOperand.Value"/> set to
    /// the computed result of the operation, or one <see cref="Error"/> listed here:
    /// <see cref="UndefinedOperation"/>.
    /// </returns>
    public static Result<IOperand> Apply(this ITrigonometricOperator 
        trigonometricOperator, IOperand operand, AngleMode mode)
    {
        var value = operand.Value;
        if(mode is AngleMode.DEG)
        {
            value = double.DegreesToRadians(value);
        }

        return trigonometricOperator switch
        {
            SineOperator => new Number(Sin(value)).ToResult<IOperand>(),
            CosineOperator => new Number(Cos(value)),
            TangentOperator => new Number(Tan(value)), 
            CotangentOperator => new Number(1 / Tan(value)),
            SecantOperator => new Number(1 / Cos(value)),
            CosecantOperator => new Number(1 / Sin(value)),

            ArcsineOperator => new Number(Asin(value)),
            ArccosineOperator => new Number(Acos(value)),
            ArctangentOperator => new Number(Atan(value)),
            ArccotangentOperator => new Number(1 / Atan(value)),
            ArcsecantOperator => new Number(1 / Acos(value)),
            ArccosecantOperator => new Number(1 / Asin(value)),

            HyperbolicSineOperator => new Number(Sinh(value)),
            HyperbolicCosineOperator => new Number(Cosh(value)),
            HyperbolicTangentOperator => new Number(Tanh(value)),
            HyperbolicCotangentOperator => new Number(1 / Tanh(value)),
            HyperbolicSecantOperator => new Number(1 / Cosh(value)),
            HyperbolicCosecantOperator => new Number(1 / Sinh(value)),

            HyperbolicArcsineOperator => new Number(Asinh(value)),
            HyperbolicArccosineOperator => new Number(Acosh(value)),
            HyperbolicArctangentOperator => new Number(Atanh(value)),
            HyperbolicArccotangentOperator => new Number(1 / Atanh(value)),
            HyperbolicArcsecantOperator => new Number(1 / Acosh(value)),
            HyperbolicArccosecantOperator => new Number(1 / Asinh(value)),

            _ => new UndefinedOperation(trigonometricOperator)
        };
    }
}