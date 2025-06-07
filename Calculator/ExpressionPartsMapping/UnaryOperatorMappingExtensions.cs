using Calculator.ExpressionPartConversions;
using Calculator.Operators;
using Essentials.Calculator;
using Essentials.ResultType;
using static Calculator.ExpressionPartsMapping.ExpressionPartSymbols;

namespace Calculator.ExpressionPartsMapping;

/// <summary>
/// Represents a class containing a method used for mapping a symbol
/// to an <see cref="IUnaryOperator"/> instance.
/// </summary>
internal class UnaryOperatorMappingExtensions
{
    /// <summary>
    /// Maps a <paramref name="symbol"/> into an <see cref="IUnaryOperator"/> instance
    /// or <see cref="ImplicitMultiplicationOperator"/> instance wrapping 
    /// <see cref="IUnaryOperator"/> instance
    /// based on <see cref="string"/> contents and <paramref name="precedingPart"/> formed
    /// by previous invocation of <see cref="ExpressionPartsMapping"/> method chain
    /// or delegates this request to the
    /// <see cref="BinaryOperatorMappingExtensions"/> class if the <paramref name="symbol"/>
    /// does not represent one of the aforementioned instances. 
    /// </summary>
    /// <param name="symbol">A <see cref="string"/> to map.</param>
    /// <param name="precedingPart">An <see cref="IExpressionPart"/> formed 
    /// by previous invocation.</param>
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
    public static Result<IExpressionPart> UnaryOperatorMappings(string symbol,
        IExpressionPart precedingPart)
    {
        Result<IExpressionPart> part = symbol switch
        {
            Sqrt => new SquareRootOperator(),
            Cbrt => new CubeRootOperator(),
            Log => new LogarithmOperator(),
            Ln => new NaturalLogarithmOperator(),
            ExclamationMark => new FactorialOperator(),

            Sin => new SineOperator(),
            Cos => new CosineOperator(),
            Tan => new TangentOperator(),
            Cot => new CotangentOperator(),
            Sec => new SecantOperator(),
            Csc => new CosecantOperator(),

            Asin => new ArcsineOperator(),
            Acos => new ArccosineOperator(),
            Atan => new ArctangentOperator(),
            Acot => new ArccotangentOperator(),
            Asec => new ArcsecantOperator(),
            Acsc => new ArccosecantOperator(),

            Sinh => new HyperbolicSineOperator(),
            Cosh => new HyperbolicCosineOperator(),
            Tanh => new HyperbolicTangentOperator(),
            Coth => new HyperbolicCotangentOperator(),
            Sech => new HyperbolicSecantOperator(),
            Csch => new HyperbolicCosecantOperator(),

            Asinh => new HyperbolicArcsineOperator(),
            Acosh => new HyperbolicArccosineOperator(),
            Atanh => new HyperbolicArctangentOperator(),
            Acoth => new HyperbolicArccotangentOperator(),
            Asech => new HyperbolicArcsecantOperator(),
            Acsch => new HyperbolicArccosecantOperator(),

            _ => BinaryOperatorMappingExtensions
                    .BinaryOperatorMappings(symbol, precedingPart)
        };

        return precedingPart is IOperand or GroupingEndOperator or
                IRightSideUnaryOperator or
                ImplicitMultiplicationOperator(IOperand) or 
                ImplicitMultiplicationOperator(GroupingEndOperator) or
                ImplicitMultiplicationOperator(IRightSideUnaryOperator)
                && part.BoolMap(part => part is ILeftSideUnaryOperator)
                ? part.Map(part => part
                    .WrapForImplicitMultiplication().ToResult()) : part;
    }
}