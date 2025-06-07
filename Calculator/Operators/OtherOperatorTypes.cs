using Essentials.Calculator;

namespace Calculator.Operators;
/// <summary>
/// Represents a type of <see cref="IExpressionPart"/>, not explicitly written 
/// but can be expanded to <see cref="MultiplicationOperator"/>.
/// </summary>
/// <param name="InnerPart">The <see cref="IExpressionPart"/> that is next 
/// to <see cref="ImplicitMultiplicationOperator"/>.</param>
public readonly record struct ImplicitMultiplicationOperator
    (IExpressionPart InnerPart) : IExpressionPart;
/// <summary>
/// Represents a type of <see cref="IGroupingOperator"/>, denoted by the ( symbol.
/// </summary>
public readonly record struct GroupingStartOperator : IGroupingOperator;
/// <summary>
/// Represents a type of <see cref="IGroupingOperator"/>, denoted by the ) symbol.
/// </summary>
public readonly record struct GroupingEndOperator : IGroupingOperator;