using Essentials.Calculator;

namespace Calculator.Operators;

/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="SinhToken"/>.
/// </summary>
public readonly record struct HyperbolicSineOperator : IDirectHyperbolicTrigOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="CoshToken"/>.
/// </summary>
public readonly record struct HyperbolicCosineOperator : IDirectHyperbolicTrigOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="TanhToken"/>.
/// </summary>
public readonly record struct HyperbolicTangentOperator : IDirectHyperbolicTrigOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="CothToken"/>.
/// </summary>
public readonly record struct HyperbolicCotangentOperator : IDirectHyperbolicTrigOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="SechToken"/>.
/// </summary>
public readonly record struct HyperbolicSecantOperator : IDirectHyperbolicTrigOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="CschToken"/>.
/// </summary>
public readonly record struct HyperbolicCosecantOperator : IDirectHyperbolicTrigOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="AsinhToken"/>.
/// </summary>
public readonly record struct HyperbolicArcsineOperator : IInverseHyperbolicTrigOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="AcoshToken"/>.
/// </summary>
public readonly record struct HyperbolicArccosineOperator : IInverseHyperbolicTrigOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="AtanhToken"/>.
/// </summary>
public readonly record struct HyperbolicArctangentOperator : IInverseHyperbolicTrigOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="AcothToken"/>.
/// </summary>
public readonly record struct HyperbolicArccotangentOperator : IInverseHyperbolicTrigOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="AsechToken"/>.
/// </summary>
public readonly record struct HyperbolicArcsecantOperator : IInverseHyperbolicTrigOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the <see cref="AcschToken"/>.
/// </summary>
public readonly record struct HyperbolicArccosecantOperator : IInverseHyperbolicTrigOperator;