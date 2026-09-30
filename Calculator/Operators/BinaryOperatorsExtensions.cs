using Calculator.ExpressionPartConversions;
using Calculator.Operands;
using Essentials.Calculator;
using Essentials.ErrorType;
using Essentials.ResultType;

namespace Calculator.Operators;

/// <summary>
/// Represents an extension class containing a method to evaluate a binary operation.
/// </summary>
internal static class BinaryOperatorsExtensions
{
    /// <summary>
    /// Applies <paramref name="binaryOperator"/> to the <paramref name="leftOperand"/>
    /// and <paramref name="rightOperand"/>.
    /// </summary>
    /// <param name="binaryOperator">The <see cref="IBinaryOperator"/> applied.</param>
    /// <param name="leftOperand">The left <see cref="IOperand"/>.</param>
    /// <param name="rightOperand">The right <see cref="IOperand"/>.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> of type <see cref="IOperand"/> either representing
    /// a new <see cref="IOperand"/> instance with <see cref="IOperand.Value"/> set to
    /// the computed result of the operation, or one <see cref="Error"/> listed here:
    /// <see cref="NumberOverflow"/> |
    /// <see cref="InvalidOperation"/> |
    /// <see cref="UndefinedOperation"/>.
    /// </returns>
    public static Result<IOperand> Apply(this IBinaryOperator binaryOperator, 
        IOperand leftOperand, IOperand rightOperand)
    {
        var result = binaryOperator switch
        {
            AdditionOperator =>
                new Number(leftOperand.Value + rightOperand.Value),

            SubtractionOperator =>
                new Number(leftOperand.Value - rightOperand.Value),

            MultiplicationOperator =>
                new Number(leftOperand.Value * rightOperand.Value),

            DivisionOperator when rightOperand.Value == 0 =>
                new InvalidOperation(DivToken.Instance),
            DivisionOperator =>
                new Number(leftOperand.Value / rightOperand.Value),

            ExponentiationOperator when leftOperand.Value == 0 &&
                rightOperand.Value < 0 => new InvalidOperation(CaretToken.Instance),
            ExponentiationOperator => new Number(
                double.Pow(leftOperand.Value, rightOperand.Value)),

            NegativeExponentiationOperator when 
                leftOperand.Value == 0 && rightOperand.Value < 0 => 
                    new InvalidOperation(CaretToken.Instance),
            NegativeExponentiationOperator => new Number(
                -double.Pow(leftOperand.Value, rightOperand.Value)),

            ModuloOperator => leftOperand.Modulo(rightOperand).ToResult(),

            _ => new UndefinedOperation(binaryOperator),
        };


        return result.Map(operand =>
        {
            if (double.IsNegativeInfinity(operand.Value) || double.IsPositiveInfinity(operand.Value))
            {
                return new NumberOverflow(binaryOperator.ToToken());
            }

            if (double.IsNaN(operand.Value))
            {
                return new InvalidOperation(binaryOperator.ToToken());
            }

            return operand.ToResult();
        });

    }
}