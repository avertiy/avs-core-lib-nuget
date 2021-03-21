using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AVS.CoreLib.Json.Extensions;
using Newtonsoft.Json;

namespace AVS.CoreLib.Json.Converters
{
    /// <summary>
    /// Converts an object to JSON
    /// combines JsonObjectContract and JsonDictionaryContract
    /// NOTE you must avoid property names that will have collisions with dictionary keys
    /// usage: [JsonConverter(typeof(DictionaryConverter`TKey,TValue))]
    /// </summary>
    public class DictionaryConverter<TKey, TValue> : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object obj, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            var type = obj.GetType();
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead).ToArray();
            var dict = (IDictionary<TKey, TValue>)obj;

            //1. write object properties
            foreach (var prop in props)
            {
                if (prop.HasIgnoreAttribute())
                    continue;

                if (prop.ShouldSerialize(type, obj) == false)
                    continue;

                if (dict.Keys.Any(k => k.ToString() == prop.Name))
                    continue;

                writer.WritePropertyName(prop.Name.ToCamelCase());

                var value = prop.GetValue(obj);
                writer.WritePropertyValue(prop, value, serializer);
            }

            //2. serialize IDictionary contract
            foreach (var kp in dict)
            {
                writer.WritePropertyName(kp.Key.ToString().ToCamelCase());
                serializer.Serialize(writer, kp.Value);
            }
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("use dictionary projection instead");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IDictionary<TKey, TValue>);
        }
    }
}