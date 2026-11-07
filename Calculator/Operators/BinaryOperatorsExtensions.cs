using Calculator.ExpressionPartConversions;
using Essentials.Calculator;
using Essentials.ErrorType;
using Essentials.Numerics;
using Essentials.ResultType;

namespace Calculator.Operators;

/// <summary>
/// Represents an extension class containing a method to evaluate a binary operation.
/// </summary>
internal static class BinaryOperatorsExtensions
{
    /// <summary>
    /// Applies <paramref name="binaryOperator"/> to the <paramref name="left"/>
    /// and <paramref name="right"/>.
    /// </summary>
    /// <param name="binaryOperator">The <see cref="IBinaryOperator"/> applied.</param>
    /// <param name="left">The left <see cref="Number"/>.</param>
    /// <param name="right">The right <see cref="Number"/>.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="Number"/> either representing
    /// a new <see cref="Number"/> with result of the operation, or one <see cref="Error"/> listed here:
    /// <see cref="NumberOverflow"/> | <see cref="InvalidOperation"/> | <see cref="UndefinedOperation"/>.
    /// </returns>
    public static Result<Number> Apply(this IBinaryOperator binaryOperator, Number left, Number right)
    {
        var token = binaryOperator.ToToken();

        try
        {
            if (binaryOperator is DivisionOperator && right.IsZero())
            {
                return new InvalidOperation(token);
            }

            if (binaryOperator is ModuloOperator && right.IsZero())
            {
                return left;
            }

            return Normalize(binaryOperator switch
            {
                AdditionOperator => left.AddTo(right),
                SubtractionOperator => left.SubtractFrom(right),
                MultiplicationOperator => left.MultiplyWith(right),
                DivisionOperator => left.DivideWith(right),
                ExponentiationOperator => left.PowWith(right),
                ModuloOperator => left.ModuloWith(right),
                _ => new UndefinedOperation(binaryOperator),
            }, token);
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
}
