using System.Text.Json;
using System.Text.Json.Serialization;
using CalculatorApp.SettingsPage.Formatting;
using Essentials.Calculator;

namespace CalculatorApp.CalculatorPage.History.Converters;

public sealed class NumberTokenConverter(FormattingSettings formattingSettings)
    : JsonConverter<NumberToken>
{
    public override NumberToken Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var rawValue = reader.GetString();

        return TokenConverter.ToNumberToken(rawValue!, formattingSettings);
    }

    public override void Write(
        Utf8JsonWriter writer,
        NumberToken value,
        JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}