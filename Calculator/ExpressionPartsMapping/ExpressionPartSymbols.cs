using System.Collections.Immutable;
using System.Reflection;

namespace Calculator.ExpressionPartsMapping;

/// <summary>
/// Represent a class containing all the symbols supported by the calculator.
/// </summary>
internal class ExpressionPartSymbols
{
    /// <summary>
    /// The symbol for addition or positive value of operand.
    /// </summary>
    public const string Plus = "+";
    /// <summary>
    /// The symbol for subtraction or negative value of operand.
    /// </summary>
    public const string Minus = "-";
    /// <summary>
    /// The symbol for multiplication.
    /// </summary>
    public const string Times = "*";
    /// <summary>
    /// The symbol for division.
    /// </summary>
    public const string Slash = "/";
    /// <summary>
    /// The symbol for exponentiation.
    /// </summary>
    public const string Caret = "^";
    /// <summary>
    /// The symbol for modulo operation.
    /// </summary>
    public const string Percent = "%";
    /// <summary>
    /// The symbol for factorial operation.
    /// </summary>
    public const string ExclamationMark = "!";
    /// <summary>
    /// The symbol for open parentheses.
    /// </summary>
    public const string Open = "(";
    /// <summary>
    /// The symbol for closed parentheses.
    /// </summary>
    public const string Closed = ")";
    /// <summary>
    /// The symbol for sine function.
    /// </summary>
    public const string Sin = "sin";
    /// <summary>
    /// The symbol for cosine function.
    /// </summary>
    public const string Cos = "cos";
    /// <summary>
    /// The symbol for cotangent function.
    /// </summary>
    public const string Cot = "cot";
    /// <summary>
    /// The symbol for tangent function.
    /// </summary>
    public const string Tan = "tan";
    /// <summary>
    /// The symbol for secant function.
    /// </summary>
    public const string Sec = "sec";
    /// <summary>
    /// The symbol for cosecant function.
    /// </summary>
    public const string Csc = "csc";
    /// <summary>
    /// The symbol for inverse sine function.
    /// </summary>
    public const string Asin = "asin";
    /// <summary>
    /// The symbol for inverse cosine function.
    /// </summary>
    public const string Acos = "acos";
    /// <summary>
    /// The symbol for inverse tangent function.
    /// </summary>
    public const string Atan = "atan";
    /// <summary>
    /// The symbol for inverse cotangent function.
    /// </summary>
    public const string Acot = "acot";
    /// <summary>
    /// The symbol for inverse secant function.
    /// </summary>
    public const string Asec = "asec";
    /// <summary>
    /// The symbol for inverse cosecant function.
    /// </summary>
    public const string Acsc = "acsc";
    /// <summary>
    /// The symbol for hyperbolic sine function.
    /// </summary>
    public const string Sinh = "sinh";
    /// <summary>
    /// The symbol for hyperbolic cosine function.
    /// </summary>
    public const string Cosh = "cosh";
    /// <summary>
    /// The symbol for hyperbolic cotangent function.
    /// </summary>
    public const string Coth = "coth";
    /// <summary>
    /// The symbol for hyperbolic tangent function.
    /// </summary>
    public const string Tanh = "tanh";
    /// <summary>
    /// The symbol for hyperbolic secant function.
    /// </summary>
    public const string Sech = "sech";
    /// <summary>
    /// The symbol for hyperbolic cosecant function.
    /// </summary>
    public const string Csch = "csch";
    /// <summary>
    /// The symbol for inverse hyperbolic sine function.
    /// </summary>
    public const string Asinh = "asinh";
    /// <summary>
    /// The symbol for inverse hyperbolic cosine function.
    /// </summary>
    public const string Acosh = "acosh";
    /// <summary>
    /// The symbol for inverse hyperbolic tangent function.
    /// </summary>
    public const string Atanh = "atanh";
    /// <summary>
    /// The symbol for inverse hyperbolic cotangent function.
    /// </summary>
    public const string Acoth = "acoth";
    /// <summary>
    /// The symbol for inverse hyperbolic secant function.
    /// </summary>
    public const string Asech = "asech";
    /// <summary>
    /// The symbol for inverse hyperbolic cosecant function.
    /// </summary>
    public const string Acsch = "acsch";
    /// <summary>
    /// The symbol for square root function.
    /// </summary>
    public const string Sqrt = "√";
    /// <summary>
    /// The symbol for cube root function.
    /// </summary>
    public const string Cbrt = "∛";
    /// <summary>
    /// The symbol for logarithm function.
    /// </summary>
    public const string Log = "log";
    /// <summary>
    /// The symbol for natural logarithm function.
    /// </summary>
    public const string Ln = "ln";
    /// <summary>
    /// The symbol for constant π.
    /// </summary>
    public const string Pi = "π";
    /// <summary>
    /// The symbol for Euler's number.
    /// </summary>
    public const string E = "e";

    /// <summary>
    /// A method used to identify all <see cref="string"/> objects representing
    /// symbols inside of this class.
    /// </summary>
    /// <returns>
    /// An <see cref="ImmutableList{T}"/> of type <see cref="string"/>
    /// where each <see cref="string"/> is one identified symbol in this class.
    /// </returns>
    /// <remarks>
    /// <b>Note</b>: Do not modify this method as it relies on 
    /// <see cref="System.Reflection"/> namespace to get all the
    /// <see cref="string"/> objects.
    /// </remarks>
    public static ImmutableList<string> AllExpressionSymbols() =>
        [.. typeof(ExpressionPartSymbols)
            .GetFields(BindingFlags.Public
            | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f => f.IsLiteral && f.FieldType == typeof(string))
            .Select(f => (string)f.GetRawConstantValue()!)
            .OrderByDescending(f => f.Length)];
}