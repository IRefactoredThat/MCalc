using Essentials.ErrorType;

namespace Essentials.ResultType;

public static class ResultExtensions
{
    /// <summary>
    /// Converts a <paramref name="value"/> to a <see cref="Result{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of <paramref name="value"/>.</typeparam>
    /// <param name="value">The value to convert.</param>
    /// <returns>
    /// A new <see cref="Result{T}"/> instance wrapping <paramref name="value"/>.
    /// </returns>
    /// <remarks>
    /// <b>Note</b>: Do not rely on casting. Use this method for converting 
    /// any type by specifying the type argument explicitly, if needed.
    /// </remarks>
    public static Result<T> ToResult<T>(this T value) => value;

    /// <summary>
    /// Tries to access the <see cref="Result{T}.Value"/> of <paramref name="result"/>.
    /// If access was successful, applies <paramref name="ifValue"/> func to the value.
    /// </summary>
    /// <typeparam name="T">The type of <paramref name="result"/>.</typeparam>
    /// <typeparam name="R">The inner type of returned instance.</typeparam>
    /// <param name="result">The <see cref="Result{T}"/> instance used for mapping.</param>
    /// <param name="ifValue">The func to be applied to <see cref="Result{T}.Value"/>.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> instance of type <typeparamref name="R"/> either
    /// representing a result of <paramref name="ifValue"/> func, or <see cref="Error"/>
    /// from <paramref name="result"/>.
    /// </returns>
    /// <remarks>Note:
    /// Use <see cref="ToResult{T}(T)"/> on <paramref name="ifValue"/> 
    /// if this method cannot infer the return argument type.
    /// </remarks>
    public static Result<R> Map<T, R>(this Result<T> result, Func<T, Result<R>> ifValue) 
        => result.Error is not None ? result.Error : ifValue(result.Value);

    /// <summary>
    /// Tries to access the <see cref="Result{T}.Value"/> of <paramref name="result"/>.
    /// If access was successful, applies <paramref name="ifValue"/> func to the value.
    /// </summary>
    /// <typeparam name="T">The type of <paramref name="result"/>.</typeparam>
    /// <param name="result">The <see cref="Result{T}"/> instance used for mapping.</param>
    /// <param name="ifValue">The func to be applied to <see cref="Result{T}.Value"/></param>
    /// <returns>
    /// A <see cref="Result{T}"/> either representing a result of <paramref name="ifValue"/>
    /// func, or <see cref="Error"/> from <paramref name="result"/>.
    /// </returns>
    public static Result<T> SelfMap<T>(this Result<T> result, Func<T, Result<T>> ifValue) => 
        result.Error is not None ? result.Error : ifValue(result.Value);

    /// <summary>
    /// Tries to access the value of <paramref name="result"/>.
    /// If access was successful, applies <paramref name="ifValue"/> action 
    /// to the <see cref="Result{T}.Value"/>.
    /// If not, applies <paramref name="ifError"/> action to the 
    /// <see cref="Result{T}.Error"/>.
    /// </summary>
    /// <typeparam name="T">The type of <paramref name="result"/>.</typeparam>
    /// <param name="result">The <see cref="Result{T}"/> instance used 
    /// for mapping.</param>
    /// <param name="ifValue">The action to be applied to 
    /// <see cref="Result{T}.Value"/></param>
    /// <param name="ifError">The action to be applied to 
    /// <see cref="Result{T}.Error"/></param>
    public static void Switch<T>(this Result<T> result,
        Action<T> ifValue, Action<Error> ifError)
    {
        if(result.Error is None)
        {
            ifValue(result.Value);
        }
        else
        {
            ifError(result.Error);
        }
    }

    public static TResult ValueOrDefault<T, TResult>(this Result<T> result,
        Func<T, TResult> ifValue, TResult defaultValue)
    {
        return result.Error is None ? ifValue(result.Value) : defaultValue;
    }
}