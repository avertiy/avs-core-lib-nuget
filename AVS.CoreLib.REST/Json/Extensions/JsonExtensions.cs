using System;
using System.IO;
using System.Reflection;
using AVS.CoreLib.REST;
using AVS.CoreLib.REST.Json;
using Newtonsoft.Json;

namespace AVS.CoreLib.Json.Extensions
{
    public static class JsonExtensions
    {
        public static void WritePropertyValue(this JsonWriter writer, PropertyInfo prop, object value, JsonSerializer serializer)
        {
            var converterAttribute = prop.GetJsonConverterAttribute();
            if (converterAttribute != null)
                writer.WriteRawValue(value.Serialize(converterAttribute));
            else if (JsonHelper.IsSimpleType(prop.PropertyType))
                writer.WriteValue(value);
            else
                serializer.Serialize(writer, value);
        }

        public static string Serialize(this object value, JsonConverterAttribute attr)
        {
            return JsonConvert.SerializeObject(value, (JsonConverter)Activator.CreateInstance(attr.ConverterType));
        }

        public static JsonConverterAttribute GetJsonConverterAttribute(this PropertyInfo prop)
        {
            return (JsonConverterAttribute)prop.GetCustomAttribute(typeof(JsonConverterAttribute));
        }

        public static bool HasIgnoreAttribute(this PropertyInfo prop)
        {
            return prop.GetCustomAttribute<JsonIgnoreAttribute>() != null;
        }

        public static bool ShouldSerialize(this PropertyInfo prop, Type type, object value)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance;
            var shouldSerializeName = "ShouldSerialize" + prop.Name;
            var mi = type.GetMethod(shouldSerializeName, flags);
            if (mi != null && mi.ReturnType == typeof(bool))
            {
                bool shouldSerialize = (bool)mi.Invoke(value, new object[] { });
                return shouldSerialize;
            }
            return true;
        }

        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T Deserialize<T>(this string json) where T : new()
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static T Deserialize<T>(this JsonResult jsonResult)
        {
            using (var stringReader = new StringReader(jsonResult.JsonText))
            {
                using (var jsonTextReader = new JsonTextReader(stringReader))
                {
                    try
                    {
                        var serializer = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore };
                        return (T)serializer.Deserialize(jsonTextReader, typeof(T));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Deserialization of type {typeof(T).Name} failed", ex);
                    }
                }
            }
        }
    }
}