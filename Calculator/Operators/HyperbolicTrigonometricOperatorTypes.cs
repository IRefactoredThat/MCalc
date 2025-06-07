using Essentials.Calculator;

namespace Calculator.Operators;

/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the sinh symbol.
/// </summary>
public readonly record struct HyperbolicSineOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the cosh symbol.
/// </summary>
public readonly record struct HyperbolicCosineOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the tanh symbol.
/// </summary>
public readonly record struct HyperbolicTangentOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the coth symbol.
/// </summary>
public readonly record struct HyperbolicCotangentOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the sech symbol.
/// </summary>
public readonly record struct HyperbolicSecantOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the csch symbol.
/// </summary>
public readonly record struct HyperbolicCosecantOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the asinh symbol.
/// </summary>
public readonly record struct HyperbolicArcsineOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the acosh symbol.
/// </summary>
public readonly record struct HyperbolicArccosineOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the atanh symbol.
/// </summary>
public readonly record struct HyperbolicArctangentOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the acoth symbol.
/// </summary>
public readonly record struct HyperbolicArccotangentOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the asech symbol.
/// </summary>
public readonly record struct HyperbolicArcsecantOperator : ITrigonometricOperator;
/// <summary>
/// Represents a type of <see cref="ITrigonometricOperator"/>, denoted by the acsch symbol.
/// </summary>
public readonly record struct HyperbolicArccosecantOperator : ITrigonometricOperator;