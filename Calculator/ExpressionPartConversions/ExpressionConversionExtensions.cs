using Calculator.Operators;
using Essentials.Calculator;
using System.Collections.Immutable;

namespace Calculator.ExpressionPartConversions;

/// <summary>
/// Represents an extension class for conversions 
/// for <see cref="IExpressionPart"/> descendants.
/// </summary>
internal static class ExpressionPartConversionExtensions
{
    /// <summary>
    /// Transforms an <see cref="ImmutableList{T}"/> of type 
    /// <see cref="IExpressionPart"/> into an <see cref="Expression"/>
    /// object by attaching a given <see cref="AngleMode"/>.
    /// </summary>
    /// <param name="parts">A list to transform.</param>
    /// <param name="mode">A mode to use.</param>
    /// <returns>
    /// A new <see cref="Expression"/> object with given 
    /// <paramref name="parts"/> and <paramref name="mode"/>.
    /// </returns>
    public static Expression AttachAngleMode(this 
        ImmutableList<IExpressionPart> parts, AngleMode mode) => new(parts, mode);

    /// <summary>
    /// Wraps an <see cref="IExpressionPart"/> around an 
    /// <see cref="ImplicitMultiplicationOperator"/>.
    /// </summary>
    /// <param name="part">A part to transform.</param>
    /// <returns>
    /// A new instance of <see cref="ImplicitMultiplicationOperator"/>
    /// with <see cref="ImplicitMultiplicationOperator.InnerPart"/> set 
    /// to <paramref name="part"/>.
    /// </returns>
    public static IExpressionPart WrapForImplicitMultiplication
        (this IExpressionPart part) => new ImplicitMultiplicationOperator(part);
}