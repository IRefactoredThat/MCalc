using Essentials.ErrorType;

namespace Essentials.ResultType;

public static class ResultExtensions
{
    /// <summary>
    /// Method used to determine if <paramref name="result"/> wraps an <see cref="Error"/>.
    /// </summary>
    /// <typeparam name="T">The type of <paramref name="result"/>.</typeparam>
    /// <param name="result">The <see cref="Result{T}"/> instance used for check.</param>
    /// <returns>
    /// A <see cref="bool"/> value indicating whether 
    /// <paramref name="result"/> is an <see cref="Error"/>.
    /// </returns>
    public static bool IsError<T>(this Result<T> result) => 
        result.Error is not default(Error);

    /// <summary>
    /// Converts a <paramref name="value"/> to a <see cref="Result{T}"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of <paramref name="result"/>.</typeparam>
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
    {
        if(result.IsError())
        {
            return result.Error;
        }

        return ifValue(result.Value);
    }

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
    public static Result<T> SelfMap<T>(this Result<T> result, Func<T, Result<T>> ifValue)
    {
        if (result.IsError())
        {
            return result.Error;
        }

        return ifValue(result.Value);
    }

    /// <summary>
    /// Tries to access the <see cref="Result{T}.Value"/> of <paramref name="result"/>.
    /// If access was successful, applies <paramref name="ifValue"/> func to the value.
    /// </summary>
    /// <typeparam name="T">The type of <paramref name="result"/>.</typeparam>
    /// <param name="result">The <see cref="Result{T}"/> instance used for mapping.</param>
    /// <param name="ifValue">The func to be applied to <see cref="Result{T}.Value"/></param>
    /// <returns>
    /// A <see cref="bool"/> value either
    /// representing a result of <paramref name="ifValue"/> func, or false
    /// if <paramref name="result"/> is an <see cref="Error"/>.
    /// </returns>
    public static bool BoolMap<T>(this Result<T> result, Func<T, bool> ifValue)
    {
        if(result.IsError())
        {
            return false;
        }

        return ifValue(result.Value);
    }

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
        if(result.IsError())
        {
            ifError(result.Error);
            return;
        }

        ifValue(result.Value);
    }

    /// <summary>
    /// Tries to access the value of <paramref name="result"/>.
    /// If access was successful, applies <paramref name="ifValue"/> action 
    /// to the <see cref="Result{T}.Value"/>.
    /// If not, applies <paramref name="ifError"/> action to the 
    /// <see cref="Result{T}.Error"/>.
    /// Async variant of <see cref="Switch{T}(Result{T}, Action{T}, Action{Error})"/>.
    /// </summary>
    /// <typeparam name="T">The type of <paramref name="result"/>.</typeparam>
    /// <param name="result">The <see cref="Result{T}"/> instance used 
    /// for mapping.</param>
    /// <param name="ifValue">The func to be applied to 
    /// <see cref="Result{T}.Value"/></param>
    /// <param name="ifError">The func to be applied to 
    /// <see cref="Result{T}.Error"/></param>
    /// <returns>A <see cref="Task"/> from 
    /// <paramref name="ifValue"/> or <paramref name="ifError"/>.</returns>
    public static async Task SwitchAsync<T>(this Result<T> result,
        Func<T, Task> ifValue, Func<Error, Task> ifError)
    {
        if (result.IsError())
        {
            await ifError(result.Error);
            return;
        }

        await ifValue(result.Value);
    }

    /// <summary>
    /// Transforms an <see cref="IEnumerable{T}"/> of type <see cref="Result{T}"/>
    /// into an <see cref="Result{T}"/> of type <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in <paramref name="results"/> 
    /// sequence.</typeparam>
    /// <param name="results">The sequence to transform.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> either representing a sequence of 
    /// <see cref="Result{T}.Value"/> instances or first <see cref="Error"/>
    /// from <paramref name="results"/>.
    /// </returns>
    public static Result<IEnumerable<T>> ValuesOrFirstError<T>(
        this IEnumerable<Result<T>> results)
    {
        var result = results.FirstOrDefault(result => 
            result.IsError(), default!);

        if(result is not default(Result<T>))
        {
            return result.Error;
        }

        return results.Select(result => result.Value)
            .ToResult();
    }

    /// <summary>
    /// Tries to get the <see cref="Result{T}.Error"/> component.
    /// </summary>
    /// <typeparam name="T">The type of <paramref name="result"/>.</typeparam>
    /// <param name="result">The <see cref="Result{T}"/> instance.</param>
    /// <returns>
    /// A <see cref="Error"/> object either representing a <see cref="Result{T}.Error"/> 
    /// from <paramref name="result"/> or <see cref="NoError"/> object.
    /// </returns>
    public static Error GetErrorOrNoError<T>(this Result<T> result) =>
        result.IsError() ? result.Error : new NoError();
}