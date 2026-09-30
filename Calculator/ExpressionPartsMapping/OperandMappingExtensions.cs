using System.Collections.Immutable;
using System.Globalization;
using Calculator.ExpressionPartConversions;
using Calculator.Operands;
using Calculator.Operators;
using Essentials.Calculator;
using Essentials.ErrorType;
using Essentials.ResultType;

namespace Calculator.ExpressionPartsMapping;

/// <summary>
/// Represents a class containing a method used for mapping a token
/// to an <see cref="IOperand"/> instance.
/// </summary>
internal static class OperandMappingExtensions
{
    /// <summary>
    /// Maps a <paramref name="token"/> into an <see cref="IOperand"/> instance
    /// or <see cref="ImplicitMultiplicationOperator"/> instance wrapping
    /// <see cref="IOperand"/> instance
    /// based on <see cref="IExpressionPart"/> formed
    /// by previous invocation of <see cref="ExpressionPartsMapping"/> method chain
    /// or returns an <see cref="Error"/> when the mapping was not successful.
    /// This method represent the end of chain.
    /// </summary>
    /// <param name="token">An assumed <see cref="IExpressionPart"/> to map.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="IExpressionPart"/> being
    /// an <see cref="IOperand"/> instance or <see cref="ImplicitMultiplicationOperator"/> that wraps it.
    /// </returns>
    public static Result<ImmutableArray<IExpressionPart>> OperandMappings(
        ImmutableArray<IExpressionPart> parts, NumberFormatInfo info, IToken token)
    {
        Result<IOperand> operand = token switch
        {
            PiToken => new PiConstant(),
            EToken => new EConstant(),
            NumberToken number => double.TryParse(number.Value,
                NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent |
                NumberStyles.AllowThousands | NumberStyles.AllowLeadingSign,
                info, out var result) && double.IsFinite(result)
                ? new Number(result) : new NumberFormatError(),
            _ => throw new ArgumentException($"Invalid argument of type {token.GetType().Name}", nameof(token))
        };

        return operand.Map<IOperand, ImmutableArray<IExpressionPart>>(value =>
            parts[^1] is GroupingEndOperator ? parts.Add(value.WrapForImplicitMultiplication()) : parts.Add(value));
    }
}