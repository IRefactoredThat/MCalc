using System.Text.Json.Serialization;
using CalculatorApp.SettingsPage.Backup;

namespace CalculatorApp.SettingsPage.Base;

public enum PreferenceType { String, Int, Bool, Long, Double, Float, DateTime, DateTimeOffset }
[JsonConverter(typeof(PreferenceJsonConverter))]
public readonly record struct Preference(string Key, PreferenceType Type, object Value);

public class SettingsPreferences() :
    IPreferences, IStorage<Preference[]>
{
    private readonly Dictionary<string, object> _entries = [];
    private readonly IPreferences _preferences = Preferences.Default;

    public bool ContainsKey(string key, string? sharedName = null) =>
        _preferences.ContainsKey(key, sharedName);

    public void Remove(string key, string? sharedName = null)
    {
        _entries.Remove(key);
        _preferences.Remove(key, sharedName);
    }

    public void Clear(string? sharedName = null)
    {
        _entries.Clear();
        _preferences.Clear(sharedName);
    }

    public void Set<T>(string key, T value, string? sharedName = null)
    {
        if (value is null)
        {
            return;
        }
        _entries[key] = value;
        _preferences.Set(key, value, sharedName);
    }

    public T Get<T>(string key, T defaultValue, string? sharedName = null)
    {
        var value = _preferences.Get(key, defaultValue, sharedName);
        if (value is null)
        {
            return defaultValue;
        }
        _entries[key] = value;
        return value;
    }

    public Preference[] Get() =>
    [
        .. _entries.Select(x =>
        {
            var preferenceType = x.Value switch
            {
                string => PreferenceType.String,
                int => PreferenceType.Int,
                bool => PreferenceType.Bool,
                long => PreferenceType.Long,
                double => PreferenceType.Double,
                float => PreferenceType.Float,
                DateTime => PreferenceType.DateTime,
                DateTimeOffset => PreferenceType.DateTimeOffset,
                _ => throw new ArgumentOutOfRangeException(nameof(x.Value), x, null)
            };

            return new Preference(x.Key, preferenceType, x.Value);
        })
    ];

    public void Save(Preference[] item)
    {
        foreach (var (key, type, value) in item)
        {
            switch (type)
            {
                case PreferenceType.String: Set(key, (string)value); break;
                case PreferenceType.Int: Set(key, (int) value); break;
                case PreferenceType.Bool: Set(key, (bool) value); break;
                case PreferenceType.Long: Set(key, (long) value); break;
                case PreferenceType.Double: Set(key, (double) value); break;
                case PreferenceType.Float: Set(key, (float) value); break;
                case PreferenceType.DateTime: Set(key, (DateTime) value); break;
                case PreferenceType.DateTimeOffset: Set(key, (DateTimeOffset) value); break;
                default: throw new ArgumentOutOfRangeException(nameof(type));
            }
        }
    }

    public Task SaveAsync(Preference[] item)
    {
        Save(item);
        return Task.CompletedTask;
    }
}
