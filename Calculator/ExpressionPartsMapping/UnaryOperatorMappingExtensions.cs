using System.Collections.Immutable;
using System.Globalization;
using Calculator.ExpressionPartConversions;
using Calculator.Operators;
using Essentials.Calculator;
using Essentials.ResultType;

namespace Calculator.ExpressionPartsMapping;

/// <summary>
/// Represents a class containing a method used for mapping a token
/// to an <see cref="IUnaryOperator"/> instance.
/// </summary>
internal static class UnaryOperatorMappingExtensions
{
    /// <summary>
    /// Maps a <paramref name="token"/> into an <see cref="IUnaryOperator"/> instance
    /// or <see cref="ImplicitMultiplicationOperator"/> instance wrapping 
    /// <see cref="IUnaryOperator"/> instance
    /// based on <see cref="IExpressionPart"/> formed
    /// by previous invocation of <see cref="ExpressionPartsMapping"/> method chain
    /// or delegates this request to the
    /// <see cref="BinaryOperatorMappingExtensions"/> class.
    /// </summary>
    /// <param name="token">A <see cref="IExpressionPart"/> to map.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="IExpressionPart"/> either being:
    /// <para>
    /// 1) An <see cref="IUnaryOperator"/> instance or 
    /// <see cref="ImplicitMultiplicationOperator"/> instance, 
    /// when the mapping was successful.
    /// </para>
    /// 2) A <see cref="Result{T}"/> of type <see cref="IExpressionPart"/> formed by
    /// delegating the request to the <see cref="BinaryOperatorMappingExtensions"/> class,
    /// when the mapping was not successful.
    /// </returns>
    public static Result<ImmutableArray<IExpressionPart>> UnaryOperatorMappings(
        ImmutableArray<IExpressionPart> parts, NumberFormatInfo info, IToken token)
    {
        IExpressionPart @operator = token switch
        {
            SinToken => new SineOperator(),
            CosToken => new CosineOperator(),
            TanToken => new TangentOperator(),
            CotToken => new CotangentOperator(),
            SecToken => new SecantOperator(),
            CscToken => new CosecantOperator(),

            AsinToken => new ArcsineOperator(),
            AcosToken => new ArccosineOperator(),
            AtanToken => new ArctangentOperator(),
            AcotToken => new ArccotangentOperator(),
            AsecToken => new ArcsecantOperator(),
            AcscToken => new ArccosecantOperator(),

            SinhToken => new HyperbolicSineOperator(),
            CoshToken => new HyperbolicCosineOperator(),
            TanhToken => new HyperbolicTangentOperator(),
            CothToken => new HyperbolicCotangentOperator(),
            SechToken => new HyperbolicSecantOperator(),
            CschToken => new HyperbolicCosecantOperator(),

            AsinhToken => new HyperbolicArcsineOperator(),
            AcoshToken => new HyperbolicArccosineOperator(),
            AtanhToken => new HyperbolicArctangentOperator(),
            AcothToken => new HyperbolicArccotangentOperator(),
            AsechToken => new HyperbolicArcsecantOperator(),
            AcschToken => new HyperbolicArccosecantOperator(),

            SqrtToken => new SquareRootOperator(),
            CbrtToken => new CubeRootOperator(),
            LogToken => new LogarithmOperator(),
            LnToken => new NaturalLogarithmOperator(),
            ExclamationMarkToken => new FactorialOperator(),
            _ => new EmptyPart()
        };
        return (@operator, parts[^1]) switch
        {
            (ILeftSideUnaryOperator unaryOperator, IOperand or GroupingEndOperator or
                IRightSideUnaryOperator or
                ImplicitMultiplicationOperator(IOperand) or
                ImplicitMultiplicationOperator(GroupingEndOperator) or
                ImplicitMultiplicationOperator(IRightSideUnaryOperator)) =>
                parts.Add(unaryOperator.WrapForImplicitMultiplication()),
            (IUnaryOperator unaryOperator, _) => parts.Add(unaryOperator),
            _ => BinaryOperatorMappingExtensions.BinaryOperatorMappings(parts, info, token)
        };
    }
}