#nullable enable
using System.Reflection;
using AVS.CoreLib.Extensions.Reflection;
using Newtonsoft.Json;

namespace AVS.CoreLib.REST.Json.Newtonsoft
{
    public static class NewtonsoftJsonExtensions
    {
        public static void WritePropertyValue(this JsonWriter writer, PropertyInfo prop, object value, JsonSerializer serializer)
        {
            var converterAttribute = prop.GetJsonConverterAttribute();

            if (converterAttribute != null)
            {
                var json = NewtonsoftJsonSerializer.SerializeObject(value, null, converterAttribute.ConverterType);
                writer.WriteRawValue(json);
            }
            else if (prop.PropertyType.IsSimpleType())
            {
                writer.WriteValue(value);
            }
            else
            {
                serializer.Serialize(writer, value);
            }
                
        }

        private static JsonConverterAttribute? GetJsonConverterAttribute(this PropertyInfo prop)
        {
            return (JsonConverterAttribute?)prop.GetCustomAttribute(typeof(JsonConverterAttribute));
        }

        public static bool HasIgnoreAttribute(this PropertyInfo prop)
        {
            return prop.GetCustomAttribute<JsonIgnoreAttribute>() != null;
        }      
    }
}