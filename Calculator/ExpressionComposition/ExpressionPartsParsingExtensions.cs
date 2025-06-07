using System.Text.RegularExpressions;
using Essentials.Calculator;

using static Calculator.ExpressionPartsMapping.ExpressionPartSymbols;

namespace Calculator.ExpressionComposition;

/// <summary>
/// Represent an extension class containing a helper method used for 
/// <see cref="ExpressionParsingExtensions.Parse(string, AngleMode)"/> method.
/// </summary>
internal static class ExpressionPartsParsingExtensions
{
    /// <summary>
    /// Identifies all symbols found inside <paramref name="expression"/>.
    /// </summary>
    /// <param name="expression">The <see cref="string"/> used for identification.</param>
    /// <returns> 
    /// A <see cref="IEnumerable{T}"/> of type <see cref="string"/> 
    /// where each <see cref="string"/> is one identified symbol.
    /// </returns>
    /// <remarks>
    /// <para>Note: Symbols that can be returned are declared in 
    /// <see cref="ExpressionPartsMapping.ExpressionPartSymbols"/> class.
    /// </para>
    /// </remarks>
    public static IEnumerable<string> Symbols(string expression) =>
        SymbolsExtractionRegex
            .Matches(expression)
            .Select(match => match.Value);

    /// <summary>
    /// A <see cref="Regex"/> object used for <see cref="Symbols(string)"/> method.
    /// Can be directly accessed to use methods like 
    /// <see cref="Regex.EnumerateMatches(ReadOnlySpan{char})"/>.
    /// </summary>
    public static Regex SymbolsExtractionRegex => new(
$@"
    {string.Join("|", AllExpressionSymbols().Select(Regex.Escape))} 
    # Matches all the symbols defined here
    | \d+(\.\d+)?([E][+-]?\d+)    # Matches a number with optional decimal point, for scientific notation
    | \d+(,\d+)*(\.\d+)?          # Matches numbers with optional commas and optional decimal
    | \d+\.                       # Matches a number followed by a dot, ensuring nothing follows it
    | \.\d+                       # Matches a dot followed by a number
",
RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace
);
}