using Essentials.Calculator;

namespace CalculatorApp.CalculatorPage.History;

public class HistoryItem : IEquatable<HistoryItem>
{
    public required IReadOnlyList<IToken> Expression { get; init; }
    public required NumberToken Result { get; init; }

    public bool Equals(HistoryItem? other)
    {
        if (other is null)
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Result.Equals(other.Result) && Expression.SequenceEqual(other.Expression);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }
        if (ReferenceEquals(this, obj))
        {
            return true;
        }
        return obj.GetType() == GetType() && Equals((HistoryItem)obj);
    }

    public override int GetHashCode() => HashCode.Combine(Expression, Result);
}