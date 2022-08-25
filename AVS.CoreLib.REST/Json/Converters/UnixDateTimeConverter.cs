using System;
using AVS.CoreLib.Dates;
using AVS.CoreLib.Utilities;
using Newtonsoft.Json;

namespace AVS.CoreLib.REST.Json.Converters
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
            var nullable = ReflectionUtils.IsNullable(objectType);
            if (reader.TokenType == JsonToken.Null)
            {
                if (!nullable)
                {
                    throw new JsonSerializationException($"Cannot convert null value to {objectType}.");
                }

                return null;
            }

            long seconds;

            if (reader.TokenType == JsonToken.Integer)
            {
                seconds = (long)reader.Value!;
            }
            else if (reader.TokenType == JsonToken.String)
            {
                if (!long.TryParse((string)reader.Value!, out seconds))
                {
                    throw new JsonSerializationException($"Unable to parse string token {reader.Value} into long.");
                }
            }
            else
            {
                throw new JsonSerializationException(
                    $"Unexpected token parsing date. Expected Integer or String, got {reader.TokenType}.");
            }

            var timestamp = DateTimeHelper.FromUnixTimestamp(seconds);
            return timestamp;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue((long)Math.Round(((DateTime)value - new DateTime(1970, 1, 1)).TotalSeconds));
        }
    }
}
