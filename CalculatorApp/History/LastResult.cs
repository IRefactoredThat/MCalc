namespace CalculatorApp.History;

internal class LastResult
{
    private const string PreviousAnswerKey = "pA";

    public static string Get() => 
        Preferences.Get(PreviousAnswerKey, string.Empty);
    public static void Set(string formattedResult) => 
        Preferences.Set(PreviousAnswerKey, formattedResult);
}