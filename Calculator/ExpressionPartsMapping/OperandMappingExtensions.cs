using System.Globalization;
using Calculator.ExpressionPartConversions;
using Calculator.Operators;
using Essentials.Calculator;
using Essentials.ErrorType;
using Essentials.Numerics;
using Essentials.ResultType;

namespace Calculator.ExpressionPartsMapping;

/// <summary>
/// Represents a class containing a method used for mapping a token
/// to an <see cref="Number"/> instance.
/// </summary>
internal static class NumberMappingExtensions
{
    /// <summary>
    /// Maps a <paramref name="token"/> into an <see cref="Number"/> instance
    /// or <see cref="ImplicitMultiplicationOperator"/> instance wrapping
    /// <see cref="Number"/> instance
    /// based on <see cref="IExpressionPart"/> formed
    /// by previous invocation of <see cref="ExpressionPartsMapping"/> method chain
    /// or returns an <see cref="Error"/> when the mapping was not successful.
    /// This method represent the end of chain.
    /// </summary>
    /// <param name="token">An assumed <see cref="IExpressionPart"/> to map.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="IExpressionPart"/> being
    /// an <see cref="Number"/> instance or <see cref="ImplicitMultiplicationOperator"/> that wraps it.
    /// </returns>
    public static Result<Expression> NumberMappings(
        Expression expression, NumberFormatInfo info, IToken token)
    {
        Result<Number> operand = token switch
        {
            PiToken => Number.Pi,
            EToken => Number.E,
            NumberToken number => ParseNumber(number.Value, info),
            _ => throw new ArgumentException(
                $"Invalid argument of type {token.GetType().Name}", nameof(token))
        };

        return operand.Map<Number, Expression>(value =>
            expression.Parts[^1] switch
            {
                GroupingEndOperator => expression.AddPart(
                    value.WrapForImplicitMultiplication()),
                AdditionOperator or SubtractionOperator when
                    expression.Parts[^2] is not Number => expression.AddPart(
                    value).AddPart(new GroupingEndOperator()),
                _ => expression.AddPart(value)
            });
    }

    private static Result<Number> ParseNumber(
        string value, NumberFormatInfo info) =>
        Number.TryParse(value,
            NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent |
            NumberStyles.AllowThousands | NumberStyles.AllowLeadingSign,
            info, out var result) ? result : new NumberFormatError();
}
