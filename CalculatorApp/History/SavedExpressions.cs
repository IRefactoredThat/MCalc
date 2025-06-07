using System.Text.Json;

namespace CalculatorApp.History;

public class ExpressionInfo : IEquatable<ExpressionInfo>
{
    public required string Expression { get; init; }
    public required string Result { get; init; }
    public required string Mode { get; init; }

    public bool Equals(ExpressionInfo? other)
    {
        if(other is null)
        {
            return false;
        }

        return Expression == other.Expression && Result == other.Result
            && Mode == other.Mode;
    }

    public override bool Equals(object? obj) => Equals(obj as ExpressionInfo);

    public override int GetHashCode() => HashCode.Combine(Expression, Result, Mode);
}

internal class SavedExpressions
{
    private const string PreviousAnswersKey = "pAs";
    private const int MaxCount = 20;

    public static bool AreNotEmpty() => Get().Count != 0;

    public static void Add(ExpressionInfo itemToAdd)
    {
        var deserializedItems = Get();
        if(deserializedItems.Count >= MaxCount)
        {
            deserializedItems.RemoveLast();
        }
        deserializedItems.AddFirst(itemToAdd);

        var newSerializedItems = JsonSerializer.Serialize(deserializedItems);
        Preferences.Set(PreviousAnswersKey, newSerializedItems);
    }

    public static LinkedList<ExpressionInfo> Get()
    {
        var serializedItems = Preferences.Get(PreviousAnswersKey, string.Empty);
        if(string.IsNullOrEmpty(serializedItems))
        {
            return [];
        }

        var deserializedItems = JsonSerializer.Deserialize
            <LinkedList<ExpressionInfo>>(serializedItems);
        return deserializedItems is null ? [] : deserializedItems;
    }

    public static bool TryRemove(ExpressionInfo itemToRemove)
    {
        var deserializedItems = Get();
        var removed = deserializedItems.Remove(itemToRemove);

        var serializedItems = JsonSerializer.Serialize(deserializedItems);
        Preferences.Set(PreviousAnswersKey, serializedItems);
        return removed;
    }
}