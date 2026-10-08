using System.Text.Json;
using System.Text.Json.Serialization;

namespace CalculatorApp.SettingsPage.Base;

public sealed class PreferenceJsonConverter : JsonConverter<Preference>
{
    public override Preference Read(ref Utf8JsonReader reader, Type _, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;
        var key = root.GetProperty(nameof(Preference.Key)).GetString()!;
        var type = (PreferenceType)root.GetProperty(nameof(Preference.Type)).GetInt32();
        var v = root.GetProperty(nameof(Preference.Value));
        object value = type switch
        {
            PreferenceType.String => v.GetString()!,
            PreferenceType.Int => v.GetInt32(),
            PreferenceType.Bool => v.GetBoolean(),
            PreferenceType.Long => v.GetInt64(),
            PreferenceType.Double => v.GetDouble(),
            PreferenceType.Float => v.GetSingle(),
            PreferenceType.DateTime => v.GetDateTime(),
            PreferenceType.DateTimeOffset => v.GetDateTimeOffset(),
            _ => throw new JsonException("Unknown preference type")
        };
        return new Preference(key, type, value);
    }

    public override void Write(Utf8JsonWriter writer, Preference value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString(nameof(Preference.Key), value.Key);
        writer.WriteNumber(nameof(Preference.Type), (int)value.Type);
        writer.WritePropertyName(nameof(Preference.Value));
        switch (value.Type)
        {
            case PreferenceType.String: writer.WriteStringValue((string)value.Value); break;
            case PreferenceType.Int: writer.WriteNumberValue((int)value.Value); break;
            case PreferenceType.Bool: writer.WriteBooleanValue((bool)value.Value); break;
            case PreferenceType.Long: writer.WriteNumberValue((long)value.Value); break;
            case PreferenceType.Double: writer.WriteNumberValue((double)value.Value); break;
            case PreferenceType.Float: writer.WriteNumberValue((float)value.Value); break;
            case PreferenceType.DateTime: writer.WriteStringValue((DateTime)value.Value); break;
            case PreferenceType.DateTimeOffset: writer.WriteStringValue((DateTimeOffset)value.Value); break;
            default: throw new JsonException("Unknown preference type");
        }
        writer.WriteEndObject();
    }
}
