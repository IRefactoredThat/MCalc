namespace CalculatorApp.TextFormatting;

internal static class ResultFormatingExtensions
{
    public static string ToFormattedOutput(this double value)
    {
        if(value != 0 && (value >= 1E+15 || Math.Abs(value) <= 1E-10))
        {
            return value.ToString("0.##########E+0");
        }
        return value.ToString("#,##0.##########");
    }
}