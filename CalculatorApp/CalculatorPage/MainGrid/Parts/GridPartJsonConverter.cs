using System.Text.Json;
using System.Text.Json.Serialization;
using CalculatorApp.CalculatorPage.Buttons;

namespace CalculatorApp.CalculatorPage.MainGrid.Parts;

public sealed class GridPartJsonConverter(
    IBaseButtonViewModel[] pool)
    : JsonConverter<GridViewModelPart>
{
    private readonly Dictionary<IBaseButtonViewModel, int> _indices =
        BuildIndices(pool);

    private static Dictionary<IBaseButtonViewModel, int> BuildIndices(
        IBaseButtonViewModel[] pool)
    {
        var indices = new Dictionary<IBaseButtonViewModel, int>();
        for (var i = 0; i < pool.Length; i++)
        {
            indices[pool[i]] = i;
        }

        return indices;
    }

    public override GridViewModelPart Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("GridViewModelPart must be an object.");
        }

        var layoutMode = SubgridLayoutMode.Grid;
        var rowSpanExtension = 0;
        var columnSpanExtension = 0;
        List<int>? buttons = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("Invalid GridViewModelPart property.");
            }

            var propertyName = reader.GetString();
            reader.Read();

            switch (propertyName)
            {
                case nameof(GridViewModelPart.LayoutMode):
                    layoutMode = (SubgridLayoutMode)reader.GetInt32();
                    break;

                case nameof(GridViewModelPart.RowSpanExtension):
                    rowSpanExtension = reader.GetInt32();
                    break;

                case nameof(GridViewModelPart.ColumnSpanExtension):
                    columnSpanExtension = reader.GetInt32();
                    break;

                case "Buttons":
                    buttons ??= [];
                    while (reader.Read() &&
                        reader.TokenType != JsonTokenType.EndArray)
                    {
                        buttons.Add(reader.GetInt32());
                    }

                    break;

                default:
                    reader.Skip();
                    break;
            }
        }

        var part = new GridViewModelPart(
            layoutMode, rowSpanExtension, columnSpanExtension);

        if (buttons is null) return part;
        foreach (var index in buttons.Where(index =>
                     index > 0 && index < pool.Length))
        {
            part.Add(pool[index]);
        }

        return part;
    }

    public override void Write(
        Utf8JsonWriter writer,
        GridViewModelPart value,
        JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber(nameof(GridViewModelPart.LayoutMode),
            (int)value.LayoutMode);
        writer.WriteNumber(nameof(GridViewModelPart.RowSpanExtension),
            value.RowSpanExtension);
        writer.WriteNumber(nameof(GridViewModelPart.ColumnSpanExtension),
            value.ColumnSpanExtension);

        writer.WritePropertyName("Buttons");
        writer.WriteStartArray();
        foreach (var button in value)
        {
            if (_indices.TryGetValue(button, out var index))
            {
                writer.WriteNumberValue(index);
            }
        }

        writer.WriteEndArray();
        writer.WriteEndObject();
    }
}
