using Essentials.Calculator;

namespace Calculator.Operators;

/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the sin symbol.
/// </summary>
public readonly record struct SineOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the cos symbol.
/// </summary>
public readonly record struct CosineOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the tan symbol.
/// </summary>
public readonly record struct TangentOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the cot symbol.
/// </summary>
public readonly record struct CotangentOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the sec symbol.
/// </summary>
public readonly record struct SecantOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the csc symbol.
/// </summary>
public readonly record struct CosecantOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the asin symbol.
/// </summary>
public readonly record struct ArcsineOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the acos symbol.
/// </summary>
public readonly record struct ArccosineOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the atan symbol.
/// </summary>
public readonly record struct ArctangentOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the acot symbol.
/// </summary>
public readonly record struct ArccotangentOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the asec symbol.
/// </summary>
public readonly record struct ArcsecantOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the acsc symbol.
/// </summary>
public readonly record struct ArccosecantOperator : ITrigonometricOperator;