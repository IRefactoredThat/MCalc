using System.Text.Json;
using System.Text.Json.Serialization;
using CalculatorApp.SettingsPage.Formatting;
using Essentials.Calculator;

namespace CalculatorApp.CalculatorPage.History.Converters;

public sealed class TokenConverter(FormattingSettings formattingSettings) : JsonConverter<IToken>
{
    public static NumberToken ToNumberToken(string? value,
        FormattingSettings formattingSettings)
    {
        ArgumentNullException.ThrowIfNull(value);

        var separator = (char)formattingSettings.DecimalSeparator;

        var result = string.Create(
            value.Length,
            (value, separator),
            static (destination, state) =>
            {
                for (var i = 0; i < state.value.Length; i++)
                {
                    var c = state.value[i];

                    destination[i] = c switch
                    {
                        (char)DecimalSeparator.Period or (char)DecimalSeparator.Comma
                            => state.separator,
                        _ => c
                    };
                }
            });

        return new NumberToken(result, separator);
    }

    public override IToken Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString() switch
        {
            "π" => PiToken.Instance,
            "e" => EToken.Instance,
            "+" => PlusToken.Instance,
            "-" => MinusToken.Instance,
            "×" => TimesToken.Instance,
            "÷" => DivToken.Instance,
            "^" => CaretToken.Instance,
            "%" => PercentToken.Instance,
            "!" => ExclamationMarkToken.Instance,
            "(" => OpenBracketToken.Instance,
            ")" => ClosedBracketToken.Instance,
            "sin" => SinToken.Instance,
            "cos" => CosToken.Instance,
            "tan" => TanToken.Instance,
            "cot" => CotToken.Instance,
            "sec" => SecToken.Instance,
            "csc" => CscToken.Instance,
            "asin" => AsinToken.Instance,
            "acos" => AcosToken.Instance,
            "atan" => AtanToken.Instance,
            "acot" => AcotToken.Instance,
            "asec" => AsecToken.Instance,
            "acsc" => AcscToken.Instance,
            "sinh" => SinhToken.Instance,
            "cosh" => CoshToken.Instance,
            "tanh" => TanhToken.Instance,
            "coth" => CothToken.Instance,
            "sech" => SechToken.Instance,
            "csch" => CschToken.Instance,
            "asinh" => AsinhToken.Instance,
            "acosh" => AcoshToken.Instance,
            "atanh" => AtanhToken.Instance,
            "acoth" => AcothToken.Instance,
            "asech" => AsechToken.Instance,
            "acsch" => AcschToken.Instance,
            "√" => SqrtToken.Instance,
            "∛" => CbrtToken.Instance,
            "log" => LogToken.Instance,
            "ln" => LnToken.Instance,
            var value => ToNumberToken(value, formattingSettings)
        };
    }

    public override void Write(Utf8JsonWriter writer, IToken token, JsonSerializerOptions options)
    {
        writer.WriteStringValue(token.Value);
    }
}