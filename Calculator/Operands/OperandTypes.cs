using Essentials.Calculator;

namespace Calculator.Operands;

/// <summary>
/// Represent the most common type of <see cref="IOperand"/>.
/// </summary>
/// <param name="Value">Value of this number.</param>
public readonly record struct Number(double Value) : IOperand;

/// <summary>
/// Represents a type of <see cref="IConstant"/>, denoted by <see cref="PiToken"/>.
/// </summary>
public readonly record struct PiConstant : IConstant
{
    /// <summary>
    /// Value of the constant, 3.1415926535897931...
    /// </summary>
    public double Value => double.Pi;
}

/// <summary>
/// Represents a type of <see cref="IConstant"/>, the constant <see cref="PiToken"/>.
/// </summary>
public readonly record struct EConstant : IConstant
{
    /// <summary>
    /// Value of the constant, 2.7182818284590451...
    /// </summary>
    public double Value => double.E;
}