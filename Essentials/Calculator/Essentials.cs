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
/// Represents the base for all direct circular trigonometric functions.
/// </summary>
public interface IDirectCircularTrigOperator : ITrigonometricOperator;

/// <summary>
/// Represents the base for all inverse circular trigonometric functions.
/// </summary>
public interface IInverseCircularTrigOperator : ITrigonometricOperator;

/// <summary>
/// Represents the base for all direct hyperbolic trigonometric functions.
/// </summary>
public interface IDirectHyperbolicTrigOperator : ITrigonometricOperator;

/// <summary>
/// Represents the base for all inverse hyperbolic trigonometric functions.
/// </summary>
public interface IInverseHyperbolicTrigOperator : ITrigonometricOperator;
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
    /// Period between digits.
    /// </summary>
    Period = '.'
}
