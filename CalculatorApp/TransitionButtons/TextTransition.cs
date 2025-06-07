namespace CalculatorApp.TransitionButtons;

interface ITextTransition;

internal record AlternateTextTransition(string First, string Second) : ITextTransition
{
    public string UpdateValue(string value)
    {
        if(value == First)
        {
            return Second;
        }
        return First;
    }
}

internal class TrigonometricFunctionTransition : ITextTransition
{
    public static string UpdateValueToHyp(string value)
    {
        if (value.EndsWith('h'))
        {
            return value[..^1];
        }
        return $"{value}h";
    }

    public static string UpdateValueToCir(string value)
    {
        if (value.StartsWith('a'))
        {
            return value[1..];
        }
        return $"a{value}";
    }
}