using Essentials.Calculator;

namespace Calculator.Operators;

/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="SinToken"/>.
/// </summary>
public readonly record struct SineOperator : IDirectCircularTrigOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="CosToken"/>.
/// </summary>
public readonly record struct CosineOperator : IDirectCircularTrigOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="TanToken"/>.
/// </summary>
public readonly record struct TangentOperator : IDirectCircularTrigOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="CotToken"/>.
/// </summary>
public readonly record struct CotangentOperator : IDirectCircularTrigOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="SecToken"/>.
/// </summary>
public readonly record struct SecantOperator : IDirectCircularTrigOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="CscToken"/>.
/// </summary>
public readonly record struct CosecantOperator : IDirectCircularTrigOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="AsinToken"/>.
/// </summary>
public readonly record struct ArcsineOperator : IInverseCircularTrigOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="AcosToken"/>.
/// </summary>
public readonly record struct ArccosineOperator : IInverseCircularTrigOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="AtanToken"/>.
/// </summary>
public readonly record struct ArctangentOperator : IInverseCircularTrigOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="AcotToken"/>.
/// </summary>
public readonly record struct ArccotangentOperator : IInverseCircularTrigOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="AsecToken"/>.
/// </summary>
public readonly record struct ArcsecantOperator : IInverseCircularTrigOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="AcscToken"/>.
/// </summary>
public readonly record struct ArccosecantOperator : IInverseCircularTrigOperator;