using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CalculatorApp.CalculatorPage.MainGrid.Cells;

public sealed class CellRangeJsonConverter : JsonConverter<CellRange>
{
    public override CellRange Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (value is null)
        {
            throw new JsonException("CellRange cannot be null.");
        }

        var parts = value.Split(',');
        if (parts.Length != 4 ||
            !int.TryParse(parts[0], NumberStyles.None,
                CultureInfo.InvariantCulture, out var row) ||
            !int.TryParse(parts[1], NumberStyles.None,
                CultureInfo.InvariantCulture, out var column) ||
            !int.TryParse(parts[2], NumberStyles.None,
                CultureInfo.InvariantCulture, out var rowSpan) ||
            !int.TryParse(parts[3], NumberStyles.None,
                CultureInfo.InvariantCulture, out var columnSpan))
        {
            throw new JsonException($"Invalid CellRange: '{value}'.");
        }

        return new CellRange(row, column, rowSpan, columnSpan);
    }

    public override void Write(
        Utf8JsonWriter writer,
        CellRange value,
        JsonSerializerOptions options)
    {
        writer.WriteStringValue(
            $"{value.Row},{value.Column},{value.RowSpan},{value.ColumnSpan}");
    }

    public override CellRange ReadAsPropertyName(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
        Read(ref reader, typeToConvert, options);

    public override void WriteAsPropertyName(
        Utf8JsonWriter writer,
        CellRange value,
        JsonSerializerOptions options)
    {
        writer.WritePropertyName(
            $"{value.Row},{value.Column},{value.RowSpan},{value.ColumnSpan}");
    }
}
