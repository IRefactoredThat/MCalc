using Essentials.Calculator;

namespace Calculator.Operators;

/// <summary>
/// Represents a type of <see cref="ILeftSideUnaryOperator"/>, denoted by the + symbol.
/// </summary>
public readonly record struct PositiveValueOperator : ILeftSideUnaryOperator;
/// <summary>
/// Represents a type of <see cref="ILeftSideUnaryOperator"/>, denoted by the - symbol.
/// </summary>
public readonly record struct NegationOperator : ILeftSideUnaryOperator;
/// <summary>
/// Represents a type of <see cref="ILeftSideUnaryOperator"/>, denoted by the √ symbol.
/// </summary>
public readonly struct SquareRootOperator : ILeftSideUnaryOperator;
/// <summary>
/// Represents a type of <see cref="ILeftSideUnaryOperator"/>, denoted by the ∛ symbol.
/// </summary>
public readonly struct CubeRootOperator : ILeftSideUnaryOperator;
/// <summary>
/// Represents a type of <see cref="ILeftSideUnaryOperator"/>, denoted by the log symbol.
/// </summary>
public readonly struct LogarithmOperator : ILeftSideUnaryOperator;
/// <summary>
/// Represents a type of <see cref="ILeftSideUnaryOperator"/>, denoted by the ln symbol.
/// </summary>
public readonly struct NaturalLogarithmOperator : ILeftSideUnaryOperator;
/// <summary>
/// Represents a type of <see cref="IRightSideUnaryOperator"/>, denoted by the ! symbol.
/// </summary>
public readonly record struct FactorialOperator : IRightSideUnaryOperator;