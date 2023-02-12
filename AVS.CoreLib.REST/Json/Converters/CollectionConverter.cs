using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AVS.CoreLib.REST.Extensions;
using Newtonsoft.Json;

namespace AVS.CoreLib.REST.Json.Converters
{
    /// <summary>
    /// Converts an object that implements ICollection to JSON with properties and data items
    /// combines JsonObjectContract and JsonArrayContract
    /// usage: [JsonConverter(typeof(CollectionConverter`T))]
    /// </summary>
    /// <example>
    /// JSON:
    /// { 
    ///    property1: "Value",
    ///    data: [..collection items here..]
    /// }
    /// </example>
    public class CollectionConverter<T> : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object obj, JsonSerializer serializer)
        {
            var type = obj.GetType();
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(
                prop =>
                    prop.CanRead &&
                    prop.GetIndexParameters().Length == 0 &&
                    prop.Name != "Comparer" &&
                    prop.HasIgnoreAttribute() == false &&
                    prop.ShouldSerialize(type, obj)
            ).ToArray();

            if (props.Length == 0)
            {
                //write as array
                WriteArray(writer, (ICollection<T>)obj, serializer);
            }
            else
            {
                writer.WriteStartObject();
                string dataPropertyName = null;
                if (props.Length > 1)
                {
                    //write object properties
                    foreach (var prop in props)
                    {
                        writer.WritePropertyName(prop.Name.ToCamelCase());
                        var value = prop.GetValue(obj);
                        writer.WritePropertyValue(prop, value, serializer);
                    }
                }
                else
                {
                    var value = props[0].GetValue(obj);
                    if (value is string str && !string.IsNullOrEmpty(str))
                    {
                        dataPropertyName = str.ToCamelCase();
                    }
                }

                writer.WritePropertyName(dataPropertyName ?? "data");
                WriteArray(writer, (ICollection<T>)obj, serializer);
                writer.WriteEndObject();
            }
        }

        private void WriteArray(JsonWriter writer, ICollection<T> collection, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            foreach (var item in collection)
            {
                serializer.Serialize(writer, item);
            }
            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ICollection<T>);
        }
    }
}