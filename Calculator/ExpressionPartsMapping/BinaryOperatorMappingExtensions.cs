using System.Globalization;
using Calculator.ExpressionPartConversions;
using Calculator.Operators;
using Essentials.Calculator;
using Essentials.ErrorType;
using Essentials.ResultType;

namespace Calculator.ExpressionPartsMapping;

/// <summary>
/// Represents a class containing a method used for mapping a token
/// to an <see cref="IBinaryOperator"/> instance or, when a <see cref="PlusToken"/>
/// or <see cref="MinusToken"/> appears in operand position, to a
/// <see cref="PositiveValueOperator"/> or <see cref="NegativeValueOperator"/> marker.
/// </summary>
internal static class BinaryOperatorMappingExtensions
{
    /// <summary>
    /// Maps a <paramref name="token"/> into an <see cref="IBinaryOperator"/> instance
    /// or a <see cref="PositiveValueOperator"/> / <see cref="NegativeValueOperator"/> marker
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
    /// 2) A <see cref="PositiveValueOperator"/> or <see cref="NegativeValueOperator"/> marker,
    /// when a <see cref="PlusToken"/> or <see cref="MinusToken"/> appears in operand
    /// position and therefore represents a negation of the operand it precedes.
    /// </para>
    /// <para>
    /// 3) An <see cref="InvalidBinaryOperatorUse"/>,
    /// when the mapping was successful, but the resulting instance cannot be
    /// preceded by previous <see cref="IExpressionPart"/>.
    /// </para>
    /// <para>
    /// 4) A <see cref="Result{T}"/> of type <see cref="IExpressionPart"/> formed by
    /// delegating the request to the <see cref="OperandMappingExtensions"/> class, when
    /// the mapping was not successful.
    /// </para>
    /// </returns>
    public static Result<Expression> BinaryOperatorMappings(
        Expression expression, NumberFormatInfo info,
        IToken token) => (token, expression.Parts[^1]) switch
    {
        (TimesToken or DivToken or CaretToken or PercentToken, ILeftSideUnaryOperator unaryOperator)
            => new InvalidUnaryOperatorUse(unaryOperator.ToToken()),
        (PlusToken or MinusToken or TimesToken or DivToken or CaretToken or PercentToken,
            IOperand or GroupingEndOperator or IRightSideUnaryOperator or
            ImplicitMultiplicationOperator(IOperand)) =>
            expression.AddPart(MapTokenToBinaryOperator(token)),
        (PlusToken, _) => expression.AddPart(new PositiveValueOperator()),
        (MinusToken, _) => expression.AddPart(new NegativeValueOperator()),
        (TimesToken, _) => new InvalidBinaryOperatorUse(TimesToken.Instance),
        (DivToken, _) => new InvalidBinaryOperatorUse(DivToken.Instance),
        (CaretToken, _) => new InvalidBinaryOperatorUse(CaretToken.Instance),
        (PercentToken, _) => new InvalidBinaryOperatorUse(PercentToken.Instance),
        _ => OperandMappingExtensions.OperandMappings(expression, info, token)
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
