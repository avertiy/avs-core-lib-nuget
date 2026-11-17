#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Types;

namespace AVS.CoreLib.Json
{
    public static class JsonValidationExtensions
    {
        public static void Validate<T>(this JsonElement json, T data, IList<KeyValue> mappings) where T : class
        {
            var type = data.GetType();
            var props = type.GetProperties();
            json.Validate(type, props, data, mappings);
        }

        internal static void Validate(this JsonElement json, Type type, PropertyInfo[] props, object data, IList<KeyValue> mappings)
        {
            foreach (var prop in props)
            {
                var propertyType = prop.PropertyType;

                if (!propertyType.IsEnum)
                    continue;

                var propName = prop.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? prop.Name.ToCamelCase();

                if (!json.TryGetProperty(propName, out var jsonProperty))
                    continue;

                var jsonValue = jsonProperty.GetString()!;
                var mapping = mappings.FirstOrDefault(x => x.Key == jsonValue && x.Value.GetType().Equals(propertyType));

                if (mapping == null)
                    throw new JsonException($"Mapping not found for {propName}:\"{jsonValue}\"");

                var expectedValue = mapping.Value;
                var actualValue = prop.GetValue(data)!;

                var s1 = actualValue.ToString();
                var s2 = expectedValue.ToString();

                if (s1 != s2)
                {
                    throw new JsonException($"\"{propName}\":\"{jsonValue}\": was deserialized as {propertyType.Name} `{s1}` but should be `{s2}`");
                }
            }
        }
    }
}