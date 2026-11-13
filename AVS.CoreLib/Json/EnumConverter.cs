using AVS.CoreLib.Attributes;
using AVS.CoreLib.Utilities;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AVS.CoreLib.Json
{
    /// <summary>
    /// Enum converter based on <see cref="EnumMapper"/> and <see cref="MapAttribute"/>
    /// </summary>
    public sealed class EnumConverter<T> : JsonConverter<T>
        where T : struct, Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException($"Expected string but got {reader.TokenType}.");

            var str = reader.GetString();

            if (string.IsNullOrEmpty(str))
                return default;

            if (EnumMapper.TryParse<T>(str, out var enumValue))
                return enumValue;

            throw new JsonException($"Unable to parse '{str}' as {typeof(T).Name}.");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
