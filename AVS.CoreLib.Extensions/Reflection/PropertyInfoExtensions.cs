using System;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json.Serialization;

namespace AVS.CoreLib.Extensions.Reflection
{
    public static class PropertyInfoExtensions
    {
        public static string GetDisplayName(this PropertyInfo propertyInfo)
        {
            var attr = propertyInfo.GetCustomAttribute<DisplayNameAttribute>();
            if (attr != null)
            {
                return attr.DisplayName;
            }

            return propertyInfo.Name;
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
        public static JsonConverterAttribute? GetJsonConverterAttribute(this PropertyInfo prop)
        {
            return (JsonConverterAttribute?)prop.GetCustomAttribute(typeof(JsonConverterAttribute));
        }
    }
}