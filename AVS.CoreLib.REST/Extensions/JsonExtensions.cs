#nullable enable
using System;
using System.Reflection;
using AVS.CoreLib.Extensions.Reflection;
using AVS.CoreLib.REST.Json;
using Newtonsoft.Json;

namespace AVS.CoreLib.REST.Extensions
{
    public static class JsonExtensions
    {
        public static void WritePropertyValue(this JsonWriter writer, PropertyInfo prop, object value, JsonSerializer serializer)
        {
            var converterAttribute = prop.GetJsonConverterAttribute();
            if (converterAttribute != null)
                writer.WriteRawValue(value.Serialize(converterAttribute));
            else if (prop.PropertyType.IsSimpleType())
                writer.WriteValue(value);
            else
                serializer.Serialize(writer, value);
        }

        private static string Serialize(this object value, JsonConverterAttribute attr)
        {
            return JsonHelper.SerializeObject(value, null, attr.ConverterType);
        }

        private static JsonConverterAttribute? GetJsonConverterAttribute(this PropertyInfo prop)
        {
            return (JsonConverterAttribute?)prop.GetCustomAttribute(typeof(JsonConverterAttribute));
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
                var shouldSerialize = (bool)mi.Invoke(value, new object[] { });
                return shouldSerialize;
            }
            return true;
        }

        public static string ToJson(this object obj)
        {
            return JsonHelper.SerializeObject(obj);
        }

        public static string ToJsonOrToString(this object obj)
        {
            try
            {
                return JsonHelper.SerializeObject(obj);
            }
            catch
            {
                return obj.ToString();
            }
        }

        public static string JsonSafe(this object obj)
        {
            try
            {
                return JsonHelper.SerializeObject(obj);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        [Obsolete("Use JsonHelper.DeserializeObject or IJsonService instead")]
        public static T? Deserialize<T>(this string? json)
        {
            return JsonHelper.DeserializeObject<T>(json);
        }
    }
}