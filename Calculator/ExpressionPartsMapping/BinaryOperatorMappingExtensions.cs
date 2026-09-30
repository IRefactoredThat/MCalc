using System.Collections.Immutable;
using System.Globalization;
using Calculator.ExpressionPartConversions;
using Calculator.Operators;
using Essentials.Calculator;
using Essentials.ErrorType;
using Essentials.ResultType;

namespace Calculator.ExpressionPartsMapping;

/// <summary>
/// Represents a class containing a method used for mapping a token
/// to an <see cref="IBinaryOperator"/> instance.
/// </summary>
internal static class BinaryOperatorMappingExtensions
{
    /// <summary>
    /// Maps a <paramref name="token"/> into an <see cref="IBinaryOperator"/> instance
    /// based on <see cref="IExpressionPart"/> type and <see cref="IExpressionPart"/> formed
    /// by previous invocation of <see cref="ExpressionPartsMapping"/> method chain
    /// or delegates this request to the
    /// <see cref="OperandMappingExtensions"/> class if the <paramref name="token"/>
    /// does not represent an <see cref="IBinaryOperator"/>.
    /// </summary>
    /// <param name="token">An assumed <see cref="IExpressionPart"/> to map.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="IExpressionPart"/> either being:
    /// <para>
    /// 1) An <see cref="IBinaryOperator"/> instance, when the mapping was successful.
    /// </para>
    /// <para>
    /// 2) An <see cref="InvalidBinaryOperatorUse"/>,
    /// when the mapping was successful, but the resulting instance cannot be 
    /// preceded by previous <see cref="IExpressionPart"/>.
    /// </para>
    /// 3) A <see cref="Result{T}"/> of type <see cref="IExpressionPart"/> formed by
    /// delegating the request to the <see cref="OperandMappingExtensions"/> class, when
    /// the mapping was not successful.
    /// </returns>
    public static Result<ImmutableArray<IExpressionPart>> BinaryOperatorMappings(
        ImmutableArray<IExpressionPart> parts, NumberFormatInfo info,
        IToken token) => (token, parts[^1]) switch
    {
        (TimesToken or DivToken or CaretToken or PercentToken, ILeftSideUnaryOperator unaryOperator)
            => new InvalidUnaryOperatorUse(unaryOperator.ToToken()),
        (PlusToken or MinusToken or TimesToken or DivToken or CaretToken or PercentToken,
            IOperand or GroupingEndOperator or IRightSideUnaryOperator or
            ImplicitMultiplicationOperator(IOperand)) =>
            parts.Add(MapTokenToBinaryOperator(token)),
        (PlusToken, _) => parts.Add(new PositiveValueOperator()),
        (MinusToken, _) => parts.Add(new NegationOperator()),
        (TimesToken, _) => new InvalidBinaryOperatorUse(TimesToken.Instance),
        (DivToken, _) => new InvalidBinaryOperatorUse(DivToken.Instance),
        (CaretToken, _) => new InvalidBinaryOperatorUse(CaretToken.Instance),
        (PercentToken, _) => new InvalidBinaryOperatorUse(PercentToken.Instance),
        _ => OperandMappingExtensions.OperandMappings(parts, info, token)
    };

    private static IBinaryOperator MapTokenToBinaryOperator(IToken token) => token switch
    {
        PlusToken => new AdditionOperator(),
        MinusToken => new SubtractionOperator(),
        TimesToken => new MultiplicationOperator(),
        DivToken => new DivisionOperator(),
        CaretToken => new ExponentiationOperator(),
        PercentToken => new ModuloOperator(),
        _ => throw new ArgumentOutOfRangeException(nameof(token), token, null)
    };
}