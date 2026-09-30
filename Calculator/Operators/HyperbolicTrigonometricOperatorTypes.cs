using Essentials.Calculator;

namespace Calculator.Operators;

/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="SinhToken"/>.
/// </summary>
public readonly record struct HyperbolicSineOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="CoshToken"/>.
/// </summary>
public readonly record struct HyperbolicCosineOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="TanhToken"/>.
/// </summary>
public readonly record struct HyperbolicTangentOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="CothToken"/>.
/// </summary>
public readonly record struct HyperbolicCotangentOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="SechToken"/>.
/// </summary>
public readonly record struct HyperbolicSecantOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="CschToken"/>.
/// </summary>
public readonly record struct HyperbolicCosecantOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="AsinhToken"/>.
/// </summary>
public readonly record struct HyperbolicArcsineOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="AcoshToken"/>.
/// </summary>
public readonly record struct HyperbolicArccosineOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="AtanhToken"/>.
/// </summary>
public readonly record struct HyperbolicArctangentOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="AcothToken"/>.
/// </summary>
public readonly record struct HyperbolicArccotangentOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="AsechToken"/>.
/// </summary>
public readonly record struct HyperbolicArcsecantOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="AcschToken"/>.
/// </summary>
public readonly record struct HyperbolicArccosecantOperator : ITrigonometricOperator;