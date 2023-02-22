using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.REST.Json.Converters
{
    public class DecimalConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(decimal) || objectType == typeof(decimal?);
        }

        public override object ReadJson(JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            var token = JToken.Load(reader);

            if (token.Type == JTokenType.Float || token.Type == JTokenType.Integer)
                return objectType == typeof(decimal) ? token.ToObject<decimal>() : token.ToObject<decimal?>();

            if (token.Type == JTokenType.String)
            {
                var str = token.ToString();
                if (string.IsNullOrEmpty(str))
                    return 0.0m;

                return decimal.Parse(str, NumberStyles.Number | NumberStyles.AllowExponent);
            }

            if (token.Type == JTokenType.Null && objectType == typeof(decimal?))
                return null;

            throw new JsonSerializationException($"Unexpected token type: {token.Type}");
        }

        public override void WriteJson(JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            writer.WriteValue((decimal)value);
        }
    }
}