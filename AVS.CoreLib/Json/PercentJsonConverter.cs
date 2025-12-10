using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AVS.CoreLib.Json;

public class PercentJsonConverter : JsonConverter<decimal>
{
    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var str = reader.GetString()!;

            if (str.EndsWith("%"))
            {
                // "25%" -> 0.25m
                var numeric = str.TrimEnd('%');
                if (decimal.TryParse(numeric, out var percent))
                    return percent / 100m;
            }

            // 50 -> 0.5m; 0.5 -> 0.005m
            if (decimal.TryParse(str, out var value))
                return value / 100m;

            throw new JsonException($"Cannot parse percentage string: {str}");
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            // 25 -> 25m OR 0.25m depending on rule
            var num = reader.GetDecimal();
            return num / 100m;
        }

        throw new JsonException("Invalid JSON token for percentage");
    }

    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("P"));
    }
}