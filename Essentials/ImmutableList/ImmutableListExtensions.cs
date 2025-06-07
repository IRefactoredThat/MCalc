using System.Collections.Immutable;

namespace Essentials.ImmutableList;

public static class ImmutableListExtensions
{
    /// <summary>
    /// Retrieves a subset of the specified immutable list based on the given range.
    /// </summary>
    /// <typeparam name="T">The type of elements in the immutable list.</typeparam>
    /// <param name="list">The immutable list to extract a range from.</param>
    /// <param name="range">
    /// A <see cref="Range"/> specifying the start and end indices of the subset.
    /// The range is evaluated against the total count of the list.
    /// </param>
    /// <returns>
    /// A new <see cref="ImmutableList{T}"/> containing the elements in the specified range.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the computed offset or length is invalid for the given list.
    /// </exception>
    public static ImmutableList<T> Spread<T>(this ImmutableList<T> list, Range range)
    {
        var (offset, length) = range.GetOffsetAndLength(list.Count);
        return list.GetRange(offset, length);
    }

    /// <summary>
    /// Removes a specified number of elements from the end of the immutable list.
    /// </summary>
    /// <typeparam name="T">The type of elements in the immutable list.</typeparam>
    /// <param name="list">The immutable list from which to remove elements.</param>
    /// <param name="count">The number of elements to remove from the end of the list.
    /// </param>
    /// <returns>
    /// A new <see cref="ImmutableList{T}"/> with the last <paramref name="count"/> 
    /// elements removed. If <paramref name="count"/> is greater than or equal to the
    /// list's count, an empty list is returned.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="count"/> is negative.
    /// </exception>
    public static ImmutableList<T> RemoveLast<T>(this ImmutableList<T> list, int count)
        => list.Spread(..^count);
}