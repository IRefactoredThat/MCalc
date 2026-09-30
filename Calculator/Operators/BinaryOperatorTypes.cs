using Essentials.Calculator;

namespace Calculator.Operators;

/// <summary>
/// Represents a type of <see cref="IBinaryOperator"/>, denoted by the <see cref="PlusToken"/>.
/// </summary>
public readonly record struct AdditionOperator : IBinaryOperator;
/// <summary>
/// Represents a type of <see cref="IBinaryOperator"/>, denoted by the <see cref="MinusToken"/>.
/// </summary>
public readonly record struct SubtractionOperator : IBinaryOperator;
/// <summary>
/// Represents a type of <see cref="IBinaryOperator"/>, denoted by the <see cref="TimesToken"/>.
/// </summary>
public readonly record struct MultiplicationOperator : IBinaryOperator;
/// <summary>
/// Represents a type of <see cref="IBinaryOperator"/>, denoted by the <see cref="DivToken"/>.
/// </summary>
public readonly record struct DivisionOperator : IBinaryOperator;
/// <summary>
/// Represents a type of <see cref="IBinaryOperator"/>, denoted by the <see cref="CaretToken"/>.
/// </summary>
public readonly record struct ExponentiationOperator : IBinaryOperator;
/// <summary>
/// Represents a type of <see cref="IBinaryOperator"/>, denoted by the <see cref="CaretToken"/>,
/// used to correct the behavior of the evaluation for examples like -1^-2.
/// </summary>
public readonly record struct NegativeExponentiationOperator : IBinaryOperator;
/// <summary>
/// Represents a type of <see cref="IBinaryOperator"/>, denoted by the <see cref="ModuloOperator"/>.
/// </summary>
public readonly record struct ModuloOperator : IBinaryOperator;