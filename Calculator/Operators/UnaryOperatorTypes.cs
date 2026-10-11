using Essentials.Calculator;

namespace Calculator.Operators;

/// <summary>
/// Represents a type of <see cref="IUnaryOperator"/>, produced when a
/// <see cref="MinusToken"/> appears in operand position. It is expanded into a
/// <c>(0 − …</c> grouping that applies to the operand it precedes.
/// </summary>
public readonly struct NegativeValueOperator : IUnaryOperator;
/// <summary>
/// Represents a type of <see cref="IUnaryOperator"/>, produced when a
/// <see cref="PlusToken"/> appears in operand position. It is expanded into a
/// <c>(0 + …</c> grouping that applies to the operand it precedes.
/// </summary>
public readonly struct PositiveValueOperator : IUnaryOperator;
/// <summary>
/// Represents a type of <see cref="ILeftSideUnaryOperator"/>, denoted by the <see cref="SqrtToken"/>.
/// </summary>
public readonly struct SquareRootOperator : ILeftSideUnaryOperator;
/// <summary>
/// Represents a type of <see cref="ILeftSideUnaryOperator"/>, denoted by the <see cref="CbrtToken"/>.
/// </summary>
public readonly struct CubeRootOperator : ILeftSideUnaryOperator;
/// <summary>
/// Represents a type of <see cref="ILeftSideUnaryOperator"/>, denoted by the <see cref="LogToken"/>.
/// </summary>
public readonly struct LogarithmOperator : ILeftSideUnaryOperator;
/// <summary>
/// Represents a type of <see cref="ILeftSideUnaryOperator"/>, denoted by the <see cref="LnToken"/>.
/// </summary>
public readonly struct NaturalLogarithmOperator : ILeftSideUnaryOperator;
/// <summary>
/// Represents a type of <see cref="IRightSideUnaryOperator"/>, denoted by the <see cref="ExclamationMarkToken"/>.
/// </summary>
public readonly record struct FactorialOperator : IRightSideUnaryOperator;
