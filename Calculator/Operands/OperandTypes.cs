using Essentials.Calculator;
using static System.Math;

namespace Calculator.Operands;

/// <summary>
/// Represent the most common type of <see cref="IOperand"/>.
/// </summary>
/// <param name="Value">Value of this number.</param>
public readonly record struct Number(double Value) : IOperand;

/// <summary>
/// Represents a type of <see cref="IConstant"/>, the constant π.
/// </summary>
public readonly record struct PiConstant() : IConstant
{
    /// <summary>
    /// Value of the constant, 3.1415926535897931...
    /// </summary>
    public double Value => PI;
}

/// <summary>
/// Represents a type of <see cref="IConstant"/>, the constant e.
/// </summary>
public readonly record struct EConstant() : IConstant
{
    /// <summary>
    /// Value of the constant, 2.7182818284590451...
    /// </summary>
    public double Value => E;
}