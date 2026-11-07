using Calculator.ExpressionPartConversions;
using Essentials.Calculator;
using Essentials.ErrorType;
using Essentials.Numerics;
using Essentials.ResultType;

namespace Calculator.Operators;

/// <summary>
/// Represents an extension class containing a method to evaluate a unary operation.
/// </summary>
internal static class UnaryOperatorsExtensions
{
    /// <summary>
    /// Applies <paramref name="unaryOperator"/> to the <paramref name="number"/>
    /// using <paramref name="mode"/> if necessary.
    /// </summary>
    /// <param name="unaryOperator">The <see cref="IUnaryOperator"/> applied.</param>
    /// <param name="number">The <see cref="Number"/> instance.</param>
    /// <param name="mode">The <see cref="AngleMode"/> used.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="Number"/> either representing
    /// a new <see cref="Number"/> set to the computed result of the operation,
    /// or one <see cref="Error"/> listed here:
    /// <see cref="NumberOverflow"/> | <see cref="InvalidOperation"/> |
    /// <see cref="UndefinedOperation"/>.
    /// </returns>
    public static Result<Number> Apply(this IUnaryOperator unaryOperator, Number number, AngleMode mode)
    {
        var token = unaryOperator.ToToken();

        try
        {
            var result = unaryOperator switch
            {
                IDirectCircularTrigOperator directCircularTrigOperator =>
                    directCircularTrigOperator.Apply(number, mode),
                IInverseCircularTrigOperator inverseCircularTrigOperator =>
                    inverseCircularTrigOperator.Apply(number, mode),
                IDirectHyperbolicTrigOperator directHyperbolicTrigOperator =>
                    directHyperbolicTrigOperator.Apply(number),
                IInverseHyperbolicTrigOperator inverseHyperbolicTrigOperator =>
                    inverseHyperbolicTrigOperator.Apply(number),
                ILeftSideUnaryOperator leftSideUnaryOperator =>
                    leftSideUnaryOperator.Apply(number),
                IRightSideUnaryOperator rightSideUnaryOperator =>
                    rightSideUnaryOperator.Apply(number),
                _ => new UndefinedOperation(unaryOperator)
            };

            return Normalize(result, token);
        }
        catch (OverflowException)
        {
            return new NumberOverflow(token);
        }
        catch (ArgumentOutOfRangeException)
        {
            return new InvalidOperation(token);
        }
        catch (ArithmeticException)
        {
            return new InvalidOperation(token);
        }
    }

    private static Result<Number> Normalize(Result<Number> result, OperatorToken token)
    {
        Result<Number> normalized = new InvalidOperation(token);

        result.Switch(
            value => normalized = value,
            error => normalized = error is ZeroDivision
                ? new InvalidOperation(token)
                : error);

        return normalized;
    }

    /// <summary>
    /// Applies <paramref name="unaryOperator"/> to the <paramref name="number"/>.
    /// </summary>
    /// <param name="unaryOperator">The <see cref="ILeftSideUnaryOperator"/>
    /// applied.</param>
    /// <param name="number">The <see cref="Number"/> instance.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="Number"/> either representing
    /// a new <see cref="Number"/> set to the computed result of the operation,
    /// or one <see cref="Error"/> listed here: <see cref="UndefinedOperation"/>.
    /// </returns>
    private static Result<Number> Apply(this ILeftSideUnaryOperator unaryOperator,
        Number number) => unaryOperator switch
        {
            SquareRootOperator => number.Sqrt(),
            CubeRootOperator => number.Cbrt(),
            LogarithmOperator => number.Log(),
            NaturalLogarithmOperator => number.Ln(),
            _ => new UndefinedOperation(unaryOperator),
        };

    /// <summary>
    /// Applies <paramref name="unaryOperator"/> to the <paramref name="number"/>.
    /// </summary>
    /// <param name="unaryOperator">The <see cref="IRightSideUnaryOperator"/>
    /// applied.</param>
    /// <param name="number">The <see cref="Number"/> instance.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="Number"/> either representing
    /// a new <see cref="Number"/> set to the computed result of the operation,
    /// or one <see cref="Error"/> listed here:
    /// <see cref="UndefinedOperation"/> | <see cref="NumberOverflow"/>.
    /// </returns>
    private static Result<Number> Apply(this IRightSideUnaryOperator unaryOperator, Number number) =>
        unaryOperator switch
        {
            FactorialOperator => number.Factorial(),
            _ => new UndefinedOperation(unaryOperator),
        };

    /// <summary>
    /// Applies <paramref name="trigonometricOperator"/> to the <paramref name="number"/>
    /// using <paramref name="mode"/>.
    /// </summary>
    /// <param name="trigonometricOperator">The <see cref="ITrigonometricOperator"/>
    /// applied.</param>
    /// <param name="number">The <see cref="Number"/> instance.</param>
    /// <param name="mode">The <see cref="AngleMode"/> used.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="Number"/> either representing
    /// a new <see cref="Number"/> set to computed result of the operation,
    /// or one <see cref="Error"/> listed here: <see cref="UndefinedOperation"/>.
    /// </returns>
    private static Result<Number> Apply(this IDirectCircularTrigOperator trigonometricOperator,
        Number number, AngleMode mode)
    {
        // All operations are in radians so convert before applying
        return mode is AngleMode.DEG
            ? number.ToRadians().Map(radians =>
                ApplyInRadians(trigonometricOperator, radians))
            : ApplyInRadians(trigonometricOperator, number);
    }

    private static Result<Number> ApplyInRadians(
        ITrigonometricOperator trigonometricOperator, Number number) =>
        trigonometricOperator switch
        {
            SineOperator => number.Sin(),
            CosineOperator => number.Cos(),
            TangentOperator => number.Tan(),
            CotangentOperator => number.Cot(),
            SecantOperator => number.Sec(),
            CosecantOperator => number.Csc(),
            _ => new UndefinedOperation(trigonometricOperator)
        };

    /// <summary>
    /// Applies <paramref name="trigonometricOperator"/> to the <paramref name="number"/>
    /// using <paramref name="mode"/>.
    /// </summary>
    /// <param name="trigonometricOperator">The <see cref="ITrigonometricOperator"/>
    /// applied.</param>
    /// <param name="number">The <see cref="Number"/> instance.</param>
    /// <param name="mode">The <see cref="AngleMode"/> used.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="Number"/> either representing
    /// a new <see cref="Number"/> set to the computed result of the operation,
    /// or one <see cref="Error"/> listed here: <see cref="UndefinedOperation"/>.
    /// </returns>
    private static Result<Number> Apply(this IInverseCircularTrigOperator trigonometricOperator,
        Number number, AngleMode mode)
    {
        var result = trigonometricOperator switch
        {
            ArcsineOperator => number.Asin(),
            ArccosineOperator => number.Acos(),
            ArctangentOperator => number.Atan(),
            ArccotangentOperator => number.Acot(),
            ArcsecantOperator => number.Asec(),
            ArccosecantOperator => number.Acsc(),
            _ => new UndefinedOperation(trigonometricOperator)
        };

        return result.SelfMap(value => mode is AngleMode.DEG ?
            value.ToDegrees() : value);
    }

    /// <summary>
    /// Applies <paramref name="trigonometricOperator"/> to the <paramref name="number"/>.
    /// </summary>
    /// <param name="trigonometricOperator">The <see cref="ITrigonometricOperator"/>
    /// applied.</param>
    /// <param name="number">The <see cref="Number"/> instance.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="Number"/> either representing
    /// a new <see cref="Number"/> set to the computed result of the operation,
    /// or one <see cref="Error"/> listed here:
    /// <see cref="UndefinedOperation"/>.
    /// </returns>
    private static Result<Number> Apply(this IDirectHyperbolicTrigOperator trigonometricOperator, Number number) =>
        trigonometricOperator switch
        {
            HyperbolicSineOperator => number.Sinh(),
            HyperbolicCosineOperator => number.Cosh(),
            HyperbolicTangentOperator => number.Tanh(),
            HyperbolicCotangentOperator => number.Coth(),
            HyperbolicSecantOperator => number.Sech(),
            HyperbolicCosecantOperator => number.Csch(),
            _ => new UndefinedOperation(trigonometricOperator)
        };

    /// <summary>
    /// Applies <paramref name="trigonometricOperator"/> to the <paramref name="number"/>.
    /// </summary>
    /// <param name="trigonometricOperator">The <see cref="ITrigonometricOperator"/>
    /// applied.</param>
    /// <param name="number">The <see cref="Number"/> instance.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="Number"/> either representing
    /// a new <see cref="Number"/> set to the computed result of the operation,
    /// or one <see cref="Error"/> listed here: <see cref="UndefinedOperation"/>.
    /// </returns>
    private static Result<Number> Apply(this IInverseHyperbolicTrigOperator trigonometricOperator, Number number) =>
        trigonometricOperator switch
        {
            HyperbolicArcsineOperator => number.Asinh(),
            HyperbolicArccosineOperator => number.Acosh(),
            HyperbolicArctangentOperator => number.Atanh(),
            HyperbolicArccotangentOperator => number.Acoth(),
            HyperbolicArcsecantOperator => number.Asech(),
            HyperbolicArccosecantOperator => number.Acsch(),
            _ => new UndefinedOperation(trigonometricOperator)
        };
}
