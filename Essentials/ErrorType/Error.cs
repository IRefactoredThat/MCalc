namespace Essentials.ErrorType;
/// <summary>
/// Represents an error that occurred during execution.
/// Indicates a failure of some operation.
/// </summary>
/// <param name="Message">A simple message explaining the error.</param>
public abstract record Error(string Message);
/// <summary>
/// Represents a non-existent error.
/// </summary>
public record NoError() : Error("No error occured.");