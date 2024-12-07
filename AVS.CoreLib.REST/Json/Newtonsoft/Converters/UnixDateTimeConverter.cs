using System;
using AVS.CoreLib.Dates;
using AVS.CoreLib.Extensions.Reflection;
using Newtonsoft.Json;

namespace AVS.CoreLib.REST.Json.Newtonsoft.Converters
{
    /// <summary>
    /// converts unix time in seconds to DateTime value
    /// </summary>
    public class UnixDateTimeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {

            if (reader.TokenType == JsonToken.Null)
            {
                var nullable = objectType.IsNullable();
                if (nullable)
                    return null;

                throw new JsonSerializationException($"Cannot convert null value to {objectType.Name}.");
            }

            double value;

            switch (reader.TokenType)
            {
                case JsonToken.Integer:
                    value = (long)reader.Value!;
                    break;
                case JsonToken.String:
                    {
                        if (!double.TryParse((string)reader.Value!, out value))
                            throw new JsonSerializationException($"Unable to parse string token {reader.Value} into long.");
                        break;
                    }
                case JsonToken.Float:
                    value = (double)reader.Value!;
                    break;

                default:
                    throw new JsonSerializationException(
                        $"Parse {objectType.Name} failed. Unexpected token type {reader.TokenType}.");
            }

            var timestamp = DateTimeHelper.FromUnixTimestamp(value);
            return timestamp;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue((long)Math.Round(((DateTime)value - new DateTime(1970, 1, 1)).TotalSeconds));
        }
    }
}
