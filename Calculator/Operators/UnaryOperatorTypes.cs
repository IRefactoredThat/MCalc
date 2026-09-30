using Essentials.Calculator;

namespace Calculator.Operators;

/// <summary>
/// Represents a type of <see cref="ILeftSideUnaryOperator"/>, denoted by the <see cref="PlusToken"/>.
/// </summary>
public readonly record struct PositiveValueOperator : ILeftSideUnaryOperator;
/// <summary>
/// Represents a type of <see cref="ILeftSideUnaryOperator"/>, denoted by the <see cref="MinusToken"/>.
/// </summary>
public readonly record struct NegationOperator : ILeftSideUnaryOperator;
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