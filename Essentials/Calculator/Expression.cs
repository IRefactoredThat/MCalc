namespace Essentials.Calculator;

/// <summary>
/// Represents an expression object consisting of multiple parts.
/// <see cref="Expression"/> can also be a part of <see cref="Parts"/> i.e. it
/// is a type of <see cref="IExpressionPart"/>.
/// </summary>
public class Expression : IExpressionPart
{
    private readonly List<IExpressionPart> _parts;

    /// <summary>
    /// A read-only view over the <see cref="IExpressionPart"/>
    /// instances that determine the expression object.
    /// </summary>
    public IReadOnlyList<IExpressionPart> Parts => _parts;

    /// <summary>
    /// An <see cref="AngleMode"/> used for trigonometric functions.
    /// </summary>
    public AngleMode Mode { get; }

    private Expression(List<IExpressionPart> parts, AngleMode mode)
    {
        _parts = parts;
        Mode = mode;
    }

    /// <summary>
    /// Creates an empty <see cref="Expression"/> object with the
    /// specified <paramref name="mode"/>.
    /// </summary>
    /// <param name="mode">An <see cref="AngleMode"/> used for trigonometric functions.</param>
    /// <returns>A new, empty <see cref="Expression"/> object.</returns>
    public static Expression Create(AngleMode mode) => new([], mode);

    /// <summary>
    /// Appends <paramref name="part"/> to the end of <see cref="Parts"/>.
    /// </summary>
    /// <param name="part">The <see cref="IExpressionPart"/> to append.</param>
    /// <returns>This <see cref="Expression"/> instance.</returns>
    public Expression AddPart(IExpressionPart part)
    {
        _parts.Add(part);
        return this;
    }

    /// <summary>
    /// Inserts <paramref name="part"/> at the specified <paramref name="index"/>
    /// inside of <see cref="Parts"/>.
    /// </summary>
    /// <param name="index">The zero-based index at which to insert.</param>
    /// <param name="part">The <see cref="IExpressionPart"/> to insert.</param>
    /// <returns>This <see cref="Expression"/> instance.</returns>
    public Expression InsertPart(int index, IExpressionPart part)
    {
        _parts.Insert(index, part);
        return this;
    }

    /// <summary>
    /// Removes the <see cref="IExpressionPart"/> at the specified
    /// <paramref name="index"/> from <see cref="Parts"/>.
    /// </summary>
    /// <param name="index">The zero-based index of the part to remove.</param>
    /// <returns>This <see cref="Expression"/> instance.</returns>
    public Expression RemovePart(int index)
    {
        _parts.RemoveAt(index);
        return this;
    }

    /// <summary>
    /// Removes <paramref name="count"/> <see cref="IExpressionPart"/> instances
    /// starting at the specified <paramref name="index"/> from <see cref="Parts"/>.
    /// </summary>
    /// <param name="index">The zero-based index at which to start removing.</param>
    /// <param name="count">The number of parts to remove.</param>
    /// <returns>This <see cref="Expression"/> instance.</returns>
    public Expression RemoveParts(int index, int count)
    {
        _parts.RemoveRange(index, count);
        return this;
    }
}
