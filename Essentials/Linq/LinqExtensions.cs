namespace Essentials.Linq;

public static class LinqExtensions
{
    /// <summary>
    /// Performs a scan (cumulative aggregation) over a sequence, 
    /// starting with a seed value and returning each intermediate result,
    /// without the seed itself. 
    /// Similar to Aggregate, but returns all accumulated values, 
    /// without the seed, instead of just the final result.
    /// </summary>
    /// <typeparam name="TSource">The type of elements in the source sequence.</typeparam>
    /// <typeparam name="TAccumulate">The type of the accumulated value.</typeparam>
    /// <param name="source">The sequence of elements to accumulate over.</param>
    /// <param name="seed">The omitted accumulator value.</param>
    /// <param name="func">
    /// A function that specifies how to combine the current accumulated value 
    /// with each element in the sequence.
    /// </param>
    /// <returns>
    /// An <see cref="IEnumerable{TAccumulate}"/> where each element is the result of applying 
    /// <paramref name="func"/> to the previous accumulator and the next element 
    /// of <paramref name="source"/>, without the <paramref name="seed"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="source"/> or <paramref name="func"/> is null.
    /// </exception>
    public static IEnumerable<TAccumulate> ScanWithoutSeed<TSource, TAccumulate>(
        this IEnumerable<TSource> source,
        TAccumulate seed,
        Func<TAccumulate, TSource, TAccumulate> func)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));
        ArgumentNullException.ThrowIfNull(func, nameof(func));

        var accumulator = seed;

        foreach (var item in source)
        {
            accumulator = func(accumulator, item);
            yield return accumulator;
        }
    }

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

        TAccumulate accumulator = seed;

        foreach (var item in source)
        {
            if (!predicate(accumulator))
                break;

            accumulator = func(accumulator, item);
        }

        return accumulator;
    }
}