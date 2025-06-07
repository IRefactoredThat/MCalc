using Calculator.ExpressionPartConversions;
using Calculator.Operands;
using Calculator.Operators;
using Essentials.Calculator;
using Essentials.ErrorType;
using Essentials.ResultType;
using System.Globalization;
using static Calculator.ExpressionPartsMapping.ExpressionPartSymbols;

namespace Calculator.ExpressionPartsMapping;

/// <summary>
/// Represents a class containing a method used for mapping a symbol
/// to an <see cref="IOperand"/> instance.
/// </summary>
internal class OperandMappingExtensions
{
    /// <summary>
    /// Maps a <paramref name="symbol"/> into an <see cref="IOperand"/> instance
    /// or <see cref="ImplicitMultiplicationOperator"/> instance wrapping 
    /// <see cref="IOperand"/> instance
    /// based on <see cref="string"/> contents and <paramref name="precedingPart"/> formed
    /// by previous invocation of <see cref="ExpressionPartsMapping"/> method chain
    /// or returns an <see cref="Error"/> when the mapping was not successful.
    /// This method represent the end of chain.
    /// </summary>
    /// <param name="symbol">A <see cref="string"/> to map.</param>
    /// <param name="precedingPart">An <see cref="IExpressionPart"/> formed 
    /// by previous invocation.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="IExpressionPart"/> either being:
    /// <para>
    /// 1) An <see cref="IOperand"/> instance or 
    /// <see cref="ImplicitMultiplicationOperator"/>, when the mapping was successful.
    /// </para>
    /// <para>
    /// 2) An <see cref="Error"/> listed here: <see cref="UndefinedExpressionPartMapping"/>,
    /// when the mapping of the whole chain was not successful.
    /// </para>
    /// </returns>
    public static Result<IExpressionPart> OperandMappings(string symbol, 
        IExpressionPart precedingPart)
    {
        Result<IExpressionPart> part = symbol switch
        {
            Pi => new PiConstant(),
            E => new EConstant(),
            _ when symbol.Any(char.IsDigit) => new Number(double.Parse(symbol)),
            _ => new UndefinedExpressionPartMapping(symbol),
        };

        return precedingPart is GroupingEndOperator
            ? part.Map(part => part.WrapForImplicitMultiplication().ToResult()) : part;
    }
}