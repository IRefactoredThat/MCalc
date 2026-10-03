using System.Collections.Immutable;

namespace Essentials.Calculator;

/// <summary>
/// Represents the base for all expression parts.
/// </summary>
public interface IExpressionPart;

/// <summary>
/// Represents the base for all operators.
/// </summary>
public interface IOperator : IExpressionPart;
/// <summary>
/// Represents the base for all unary operators.
/// </summary>
public interface IUnaryOperator : IOperator;
/// <summary>
/// Represents the base for all unary operators
/// that appear on the right side of the operand.
/// </summary>
public interface IRightSideUnaryOperator : IUnaryOperator;
/// <summary>
/// Represents the base for all unary operators
/// that appear on the left side of the operand.
/// </summary>
public interface ILeftSideUnaryOperator : IUnaryOperator;
/// <summary>
/// Represents the base for all trigonometric functions.
/// </summary>
public interface ITrigonometricOperator : ILeftSideUnaryOperator;
/// <summary>
/// Represents the base for all binary operators.
/// </summary>
public interface IBinaryOperator : IOperator;

/// <summary>
/// Represents the base for all grouping operators.
/// </summary>
public interface IGroupingOperator : IOperator;

/// <summary>
/// Represents the base for all operands.
/// </summary>
public interface IOperand : IExpressionPart
{
    /// <summary>
    /// Value of this operand.
    /// </summary>
    public double Value { get; }
}
/// <summary>
/// Represent the base for all constants.
/// </summary>
public interface IConstant : IOperand;

/// <summary>
/// Represents a non-existent <see cref="IExpressionPart"/>.
/// </summary>
public readonly record struct EmptyPart : IExpressionPart;

/// <summary>
/// Represents an expression object consisting of multiple parts.
/// <see cref="Expression"/> can also be a part of <see cref="Parts"/> i.e. it
/// is a type of <see cref="IExpressionPart"/>.
/// </summary>
/// <param name="Parts">A <see cref="ImmutableList{T}"/> of type
/// <see cref="IExpressionPart"/> instances used to determine the
/// expression object.</param>
/// <param name="Mode">An <see cref="AngleMode"/> used for trigonometric functions.</param>
public record Expression(ImmutableList<IExpressionPart> Parts, AngleMode Mode) : IExpressionPart;

/// <summary>
/// Represents a way angle values are interpreted in <see cref="Expression"/>.
/// </summary>
public enum AngleMode
{
    /// <summary>
    /// Angle values are in radians.
    /// </summary>
    RAD,
    /// <summary>
    /// Angle values are in degrees.
    /// </summary>
    DEG
}

/// <summary>
/// Separates digits on each thousand in <see cref="NumberToken"/>.
/// </summary>
public enum ThousandSeparator
{
    /// <summary>
    /// Comma between digits.
    /// </summary>
    Comma = ',',
    /// <summary>
    /// Period between digits.
    /// </summary>
    Period = '.',
    /// <summary>
    /// Space between digits.
    /// </summary>
    Space = ' '
}

/// <summary>
/// Separates integer and fractional digits.
/// </summary>
public enum DecimalSeparator
{
    /// <summary>
    /// Comma between digits.
    /// </summary>
    Comma = ',',
    /// <summary>
    /// Period between digits
    /// </summary>
    Period = '.'
}
