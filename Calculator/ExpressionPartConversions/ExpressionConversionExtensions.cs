using System.Collections.Immutable;
using Calculator.Operators;
using Essentials.Calculator;

namespace Calculator.ExpressionPartConversions;

/// <summary>
/// Represents an extension class for conversions 
/// for <see cref="IExpressionPart"/> descendants.
/// </summary>
internal static class ExpressionPartConversionExtensions
{
    /// <summary>
    /// Transforms an <see cref="ImmutableList{T}"/> of type 
    /// <see cref="IExpressionPart"/> into an <see cref="Expression"/>
    /// object by attaching a given <see cref="AngleMode"/>.
    /// </summary>
    /// <param name="parts">A list to transform.</param>
    /// <param name="mode">A mode to use.</param>
    /// <returns>
    /// A new <see cref="Expression"/> object with given 
    /// <paramref name="parts"/> and <paramref name="mode"/>.
    /// </returns>
    public static Expression AttachAngleMode(this 
        ImmutableList<IExpressionPart> parts, AngleMode mode) => new(parts, mode);

    /// <summary>
    /// Wraps an <see cref="IExpressionPart"/> around an 
    /// <see cref="ImplicitMultiplicationOperator"/>.
    /// </summary>
    /// <param name="part">A part to transform.</param>
    /// <returns>
    /// A new instance of <see cref="ImplicitMultiplicationOperator"/>
    /// with <see cref="ImplicitMultiplicationOperator.InnerPart"/> set 
    /// to <paramref name="part"/>.
    /// </returns>
    public static IExpressionPart WrapForImplicitMultiplication
        (this IExpressionPart part) => new ImplicitMultiplicationOperator(part);

    /// <summary>
    /// Converts a given <see cref="IBinaryOperator"/> to a <see cref="string"/>.
    /// </summary>
    /// <param name="binaryOperator">The binary operator to convert.</param>
    /// <returns>A <see cref="string"/> representing equivalent token.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Binary operator not defined.</exception>
    public static OperatorToken ToToken(this IBinaryOperator binaryOperator) =>
        binaryOperator switch
        {
            AdditionOperator => PlusToken.Instance,
            SubtractionOperator => MinusToken.Instance,
            MultiplicationOperator => TimesToken.Instance,
            DivisionOperator => DivToken.Instance,
            ExponentiationOperator or NegativeExponentiationOperator => CaretToken.Instance,
            ModuloOperator => PercentToken.Instance,
            _ => throw new ArgumentOutOfRangeException(nameof(binaryOperator), binaryOperator, null)
        };
    
    /// <summary>
    /// Converts a given <see cref="ILeftSideUnaryOperator"/> to a <see cref="string"/>.
    /// </summary>
    /// <param name="unaryOperator">The unary operator to convert.</param>
    /// <returns>A <see cref="string"/> representing equivalent token.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Unary operator not defined.</exception>
    public static OperatorToken ToToken(this ILeftSideUnaryOperator unaryOperator) =>
        unaryOperator switch
        {
            SineOperator => SinToken.Instance,
            CosineOperator => CosToken.Instance,
            TangentOperator => TanToken.Instance,
            CotangentOperator => CotToken.Instance,
            SecantOperator => SecToken.Instance,
            CosecantOperator => CscToken.Instance,
            
            ArcsineOperator => AsinToken.Instance,
            ArccosineOperator => AcosToken.Instance,
            ArctangentOperator => AtanToken.Instance,
            ArccotangentOperator => AcotToken.Instance,
            ArcsecantOperator => AsecToken.Instance,
            ArccosecantOperator => AcscToken.Instance,
            
            HyperbolicSineOperator => SinhToken.Instance,
            HyperbolicCosineOperator => CoshToken.Instance,
            HyperbolicTangentOperator => TanhToken.Instance,
            HyperbolicCotangentOperator => CothToken.Instance,
            HyperbolicSecantOperator => SechToken.Instance,
            HyperbolicCosecantOperator => CschToken.Instance,
            
            HyperbolicArcsineOperator => AsinhToken.Instance,
            HyperbolicArccosineOperator => AcoshToken.Instance,
            HyperbolicArctangentOperator => AtanhToken.Instance,
            HyperbolicArccotangentOperator => AcothToken.Instance,
            HyperbolicArcsecantOperator => AsechToken.Instance,
            HyperbolicArccosecantOperator => AcschToken.Instance,

            PositiveValueOperator => PlusToken.Instance,
            NegationOperator => MinusToken.Instance,
            SquareRootOperator => SqrtToken.Instance,
            CubeRootOperator => CbrtToken.Instance,
            LogarithmOperator => LogToken.Instance,
            NaturalLogarithmOperator => LnToken.Instance,
            
            _ => throw new ArgumentOutOfRangeException(nameof(unaryOperator), unaryOperator, null)
        };

    /// <summary>
    /// Maps a given <see cref="IRightSideUnaryOperator"/> to a <see cref="string"/>
    /// </summary>
    /// <param name="unaryOperator">The unary operator to convert.</param>
    /// <returns>A <see cref="string"/> representing equivalent token.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Unary operator not defined.</exception>
    public static OperatorToken ToToken(this IRightSideUnaryOperator unaryOperator) =>
        unaryOperator switch
        {
            FactorialOperator => ExclamationMarkToken.Instance,
            _ => throw new ArgumentOutOfRangeException(nameof(unaryOperator), unaryOperator, null)
        };

    /// <summary>
    /// Maps a given <see cref="IUnaryOperator"/> to a <see cref="string"/>
    /// </summary>
    /// <param name="unaryOperator">The unary operator to convert.</param>
    /// <returns>A <see cref="StreamReader"/> representing equivalent token.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Unary operator not defined.</exception>
    public static OperatorToken ToToken(this IUnaryOperator unaryOperator) =>
        unaryOperator switch
        {
            ILeftSideUnaryOperator leftSideUnaryOperator => leftSideUnaryOperator.ToToken(),
            IRightSideUnaryOperator rightSideUnaryOperator => rightSideUnaryOperator.ToToken(),
            _ => throw new ArgumentOutOfRangeException(nameof(unaryOperator), unaryOperator, null)
        };
}