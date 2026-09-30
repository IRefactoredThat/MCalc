namespace Essentials.ErrorType;
/// <summary>
/// Represents an error that occurred during execution.
/// Indicates a failure of some operation.
/// </summary>
/// <param name="Message">A simple message explaining the error.</param>
public abstract record Error(string Message);

/// <summary>
/// Represents a successful operation.
/// </summary>
public record None : Error
{
    private None() : base(Message: string.Empty) { }
    /// <summary>
    /// A shared instance to represent the <see cref="None"/> error.
    /// </summary>
    public static None Instance { get; } = new();
}