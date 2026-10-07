using System.Collections;

namespace CalculatorApp.SettingsPage.Base;

public readonly record struct Preference(string Key, object? Value);

public class SettingsPreferences() :
    IPreferences, IEnumerable<Preference>
{
    private readonly Dictionary<string, object?> _entries = [];
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
        _entries[key] = value;
        _preferences.Set(key, value, sharedName);
    }

    public T Get<T>(string key, T defaultValue, string? sharedName = null)
    {
        var value = _preferences.Get(key, defaultValue, sharedName);
        _entries[key] = value;
        return value;
    }

    public IEnumerator<Preference> GetEnumerator() =>
        _entries
            .Select(pair => new Preference(pair.Key, pair.Value))
            .GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
