using Calculator.ExpressionPartConversions;
using Calculator.Operands;
using Essentials.Calculator;
using Essentials.ErrorType;
using Essentials.ResultType;

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
    public static Result<IOperand> Apply(this IUnaryOperator unaryOperator, IOperand operand, AngleMode mode)
    {
        var result = unaryOperator switch
        {
            IDirectCircularTrigOperator directCircularTrigOperator =>
                directCircularTrigOperator.Apply(operand, mode),
            IInverseCircularTrigOperator inverseCircularTrigOperator =>
                inverseCircularTrigOperator.Apply(operand, mode),
            IDirectHyperbolicTrigOperator directHyperbolicTrigOperator =>
                directHyperbolicTrigOperator.Apply(operand),
            IInverseHyperbolicTrigOperator inverseHyperbolicTrigOperator =>
                inverseHyperbolicTrigOperator.Apply(operand),
            ILeftSideUnaryOperator leftSideUnaryOperator =>
                leftSideUnaryOperator.Apply(operand),
            IRightSideUnaryOperator rightSideUnaryOperator =>
                rightSideUnaryOperator.Apply(operand),
            _ => new UndefinedOperation(unaryOperator)
        };
        return result.Map(finalOperand => double.IsFinite(finalOperand.Value) ?
            finalOperand.ToResult() : new InvalidOperation(unaryOperator.ToToken()));
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

            SquareRootOperator => new Number(double.Sqrt(operand.Value)),
            CubeRootOperator => new Number(double.Cbrt(operand.Value)),

            LogarithmOperator => new Number(double.Log10(operand.Value)),
            NaturalLogarithmOperator => new Number(double.Log(operand.Value)),

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
            FactorialOperator when operand.FactorialOverflow()
                => new NumberOverflow(ExclamationMarkToken.Instance),
            FactorialOperator when operand.FactorialDefined()
                => operand.Value.GetFactorial().ToResult(),

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
    private static Result<IOperand> Apply(this IDirectCircularTrigOperator trigonometricOperator,
        IOperand operand, AngleMode mode)
    {
        var value = operand.Value;
        if (mode is AngleMode.DEG)
        {
            value = double.DegreesToRadians(value);
        }

        return trigonometricOperator switch
        {
            SineOperator => new Number(double.Sin(value)),
            CosineOperator => new Number(double.Cos(value)),
            TangentOperator => new Number(double.Tan(value)),
            CotangentOperator => new Number(double.Cos(value) / double.Sin(value)),
            SecantOperator => new Number(1 / double.Cos(value)),
            CosecantOperator => new Number(1 / double.Sin(value)),
            _ => new UndefinedOperation(trigonometricOperator)
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
    private static Result<IOperand> Apply(this IInverseCircularTrigOperator trigonometricOperator,
        IOperand operand, AngleMode mode)
    {
        var value = operand.Value;
        Result<double> result = trigonometricOperator switch
        {
            ArcsineOperator => double.Asin(value),
            ArccosineOperator => double.Acos(value),
            ArctangentOperator => double.Atan(value),
            ArccotangentOperator => double.Atan2(1, value),
            ArcsecantOperator => double.Acos(1 / value),
            ArccosecantOperator => double.Asin(1 / value),
            _ => new UndefinedOperation(trigonometricOperator)
        };

        return result.Map<double, IOperand>(validValue =>
        {
            if (!double.IsFinite(validValue))
            {
                return new InvalidOperation(trigonometricOperator.ToToken());
            }

            if (mode is AngleMode.DEG)
            {
                validValue = double.RadiansToDegrees(validValue);
            }

            return new Number(validValue);
        });
    }

    /// <summary>
    /// Applies <paramref name="trigonometricOperator"/> to the <paramref name="operand"/>.
    /// </summary>
    /// <param name="trigonometricOperator">The <see cref="ITrigonometricOperator"/>
    /// applied.</param>
    /// <param name="operand">The <see cref="IOperand"/> instance.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="IOperand"/> either representing
    /// a new <see cref="IOperand"/> instance with <see cref="IOperand.Value"/> set to
    /// the computed result of the operation, or one <see cref="Error"/> listed here:
    /// <see cref="UndefinedOperation"/>.
    /// </returns>
    private static Result<IOperand> Apply(this IDirectHyperbolicTrigOperator trigonometricOperator,
        IOperand operand)
    {
        var value = operand.Value;
        return trigonometricOperator switch
        {
            HyperbolicSineOperator => new Number(double.Sinh(value)),
            HyperbolicCosineOperator => new Number(double.Cosh(value)),
            HyperbolicTangentOperator => new Number(double.Tanh(value)),
            HyperbolicCotangentOperator => new Number(double.Cosh(value) / double.Sinh(value)),
            HyperbolicSecantOperator => new Number(1 / double.Cosh(value)),
            HyperbolicCosecantOperator => new Number(1 / double.Sinh(value)),
            _ => new UndefinedOperation(trigonometricOperator)
        };
    }

    /// <summary>
    /// Applies <paramref name="trigonometricOperator"/> to the <paramref name="operand"/>.
    /// </summary>
    /// <param name="trigonometricOperator">The <see cref="ITrigonometricOperator"/>
    /// applied.</param>
    /// <param name="operand">The <see cref="IOperand"/> instance.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="IOperand"/> either representing
    /// a new <see cref="IOperand"/> instance with <see cref="IOperand.Value"/> set to
    /// the computed result of the operation, or one <see cref="Error"/> listed here:
    /// <see cref="UndefinedOperation"/>.
    /// </returns>
    private static Result<IOperand> Apply(this IInverseHyperbolicTrigOperator trigonometricOperator,
        IOperand operand)
    {
        var value = operand.Value;
        return trigonometricOperator switch
        {
            HyperbolicArcsineOperator => new Number(double.Asinh(value)),
            HyperbolicArccosineOperator => new Number(double.Acosh(value)),
            HyperbolicArctangentOperator => new Number(double.Atanh(value)),
            HyperbolicArccotangentOperator => new Number(double.Atanh(1 / value)),
            HyperbolicArcsecantOperator => new Number(double.Acosh(1 / value)),
            HyperbolicArccosecantOperator => new Number(double.Asinh(1 / value)),
            _ => new UndefinedOperation(trigonometricOperator)
        };
    }
}
