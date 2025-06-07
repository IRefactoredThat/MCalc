using Essentials.ErrorType;

namespace Essentials.ResultType;
/// <summary>
/// Represents a type which instance either wraps an 
/// <see cref="ErrorType.Error"/> instance or <typeparamref name="T"/> instance.
/// </summary>
/// <typeparam name="T"></typeparam>
public record Result<T>
{
    /// <summary>
    /// A <typeparamref name="T"/> instance having either 
    /// a concrete or default value.
    /// </summary>
    internal T Value { get; }
    /// <summary>
    /// A <see cref="ErrorType.Error"/> instance having either 
    /// a concrete or default value.
    /// </summary>
    internal Error Error { get; }
    /// <summary>
    /// Used to construct the <see cref="Result{T}"/> instance. Users should only rely on
    /// implicit conversions.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="error"></param>
    private Result(T value, Error error) => (Value, Error) = (value, error);
    /// <summary>
    /// Implicitly converts <paramref name="value"/> to <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator Result<T>(T value) => new(value, default!);
    /// <summary>
    /// Implicitly converts <paramref name="error"/> to <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    public static implicit operator Result<T>(Error error) => new(default!, error);
}