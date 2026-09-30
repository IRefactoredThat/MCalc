namespace Essentials.Linq;

public static class LinqExtensions
{
    /// <summary>
    /// Aggregates the elements of a sequence while a condition is met.
    /// </summary>
    /// <typeparam name="TSource">The type of elements in the sequence.</typeparam>
    /// <typeparam name="TAccumulate">The type of the accumulator value.</typeparam>
    /// <param name="source">The sequence of elements to aggregate.</param>
    /// <param name="seed">The initial accumulator value.</param>
    /// <param name="func">A function that accumulates values.</param>
    /// <param name="predicate">A function to determine whether aggregation should continue.</param>
    /// <returns>The final accumulated value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if any argument is null.</exception>
    public static TAccumulate AggregateWhile<TSource, TAccumulate>(
        this IEnumerable<TSource> source,
        TAccumulate seed,
        Func<TAccumulate, TSource, TAccumulate> func,
        Func<TAccumulate, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(func);
        ArgumentNullException.ThrowIfNull(predicate);

        var accumulator = seed;

        foreach (var item in source)
        {
            if (!predicate(accumulator))
                break;

            accumulator = func(accumulator, item);
        }

        return accumulator;
    }
}