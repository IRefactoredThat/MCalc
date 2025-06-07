using Calculator.Operators;
using Essentials.Calculator;
using Essentials.ErrorType;
using Essentials.ResultType;
using static Calculator.ExpressionPartsMapping.ExpressionPartSymbols;

namespace Calculator.ExpressionPartsMapping;

/// <summary>
/// Represents a class containing a method used for mapping a symbol
/// to an <see cref="IBinaryOperator"/> instance.
/// </summary>
internal class BinaryOperatorMappingExtensions
{
    /// <summary>
    /// Maps a <paramref name="symbol"/> into an <see cref="IBinaryOperator"/> instance
    /// based on <see cref="string"/> contents and <paramref name="precedingPart"/> formed
    /// by previous invocation of <see cref="ExpressionPartsMapping"/> method chain
    /// or delegates this request to the
    /// <see cref="OperandMappingExtensions"/> class if the <paramref name="symbol"/>
    /// does not represent an <see cref="IBinaryOperator"/>.
    /// </summary>
    /// <param name="symbol">A <see cref="string"/> to map.</param>
    /// <param name="precedingPart">An <see cref="IExpressionPart"/> formed 
    /// by previous invocation.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="IExpressionPart"/> either being:
    /// <para>
    /// 1) An <see cref="IBinaryOperator"/> instance, when the mapping was successful.
    /// </para>
    /// <para>
    /// 2) An <see cref="Error"/> listed here: <see cref="InvalidBinaryOperatorUse"/>,
    /// when the mapping was successful, but the resulting instance cannot be 
    /// preceded by <paramref name="precedingPart"/>.
    /// </para>
    /// 3) A <see cref="Result{T}"/> of type <see cref="IExpressionPart"/> formed by
    /// delegating the request to the <see cref="OperandMappingExtensions"/> class, when
    /// the mapping was not successful.
    /// </returns>
    public static Result<IExpressionPart> BinaryOperatorMappings(string symbol,
        IExpressionPart precedingPart)
    {
        var resultPart = (symbol, precedingPart is IOperand or GroupingEndOperator 
            or IRightSideUnaryOperator) switch
        {
            (Plus, true) => new AdditionOperator(),
            (Plus, false) => new PositiveValueOperator(),
            (Minus, true) => new SubtractionOperator(),
            (Minus, false) => new NegationOperator(),
            (Times, true) => new MultiplicationOperator(),
            (Times, false) => new InvalidBinaryOperatorUse
                (new MultiplicationOperator()),
            (Slash, true) => new DivisionOperator(),
            (Slash, false) => new InvalidBinaryOperatorUse
                (new DivisionOperator()),
            (Caret, true) => new ExponentiationOperator(),
            (Caret, false) => new InvalidBinaryOperatorUse
                (new ExponentiationOperator()),
            (Percent, true) => new ModuloOperator(),
            (Percent, false) => new InvalidBinaryOperatorUse
                (new ModuloOperator()),
            _ => OperandMappingExtensions.OperandMappings(symbol, precedingPart)
        };

        if(resultPart.BoolMap(part => part is IBinaryOperator) &&
            precedingPart is ILeftSideUnaryOperator unaryOperator)
        {
            return new InvalidUnaryOperatorUse(unaryOperator);
        }

        return resultPart;
    }
}