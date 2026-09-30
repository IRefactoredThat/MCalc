using Essentials.Calculator;

namespace Calculator.Operators;

/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="SinToken"/>.
/// </summary>
public readonly record struct SineOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="CosToken"/>.
/// </summary>
public readonly record struct CosineOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="TanToken"/>.
/// </summary>
public readonly record struct TangentOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="CotToken"/>.
/// </summary>
public readonly record struct CotangentOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="SecToken"/>.
/// </summary>
public readonly record struct SecantOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="CscToken"/>.
/// </summary>
public readonly record struct CosecantOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="AsecToken"/>.
/// </summary>
public readonly record struct ArcsineOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="AcosToken"/>.
/// </summary>
public readonly record struct ArccosineOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="AtanToken"/>.
/// </summary>
public readonly record struct ArctangentOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="AcotToken"/>.
/// </summary>
public readonly record struct ArccotangentOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="AsecToken"/>.
/// </summary>
public readonly record struct ArcsecantOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="AcscToken"/>.
/// </summary>
public readonly record struct ArccosecantOperator : ITrigonometricOperator;