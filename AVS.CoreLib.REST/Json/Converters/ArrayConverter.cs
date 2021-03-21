using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using AVS.CoreLib.Json.Extensions;
using AVS.CoreLib.REST.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.Json.Converters
{
    /// <summary>
    /// Allows to convert json string into custom type 
    /// e.g. JSON: [0.123, 125] =>
    /// {
    ///    [ArrayProperty(0)]
    ///    double Price;
    ///    [ArrayProperty(1)]
    ///    int Qty; 
    /// }
    /// usage: [JsonConverter(typeof(ArrayConverter))]
    /// </summary>
    public class ArrayConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(JToken))
                return JToken.Load(reader);

            var result = Activator.CreateInstance(objectType);
            var arr = JArray.Load(reader);
            return ParseObject(arr, result, objectType);
        }

        private static object ParseObject(JArray arr, object result, Type objectType)
        {
            foreach (var property in objectType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var attribute =
                    (ArrayPropertyAttribute)property.GetCustomAttribute(typeof(ArrayPropertyAttribute));
                if (attribute == null)
                    continue;

                if (attribute.Index >= arr.Count)
                    continue;

                if (property.PropertyType.BaseType == typeof(Array))
                {
                    var objType = property.PropertyType.GetElementType();
                    var innerArray = (JArray)arr[attribute.Index];
                    var count = 0;
                    if (innerArray.Count == 0)
                    {
                        var arrayResult = (IList)Activator.CreateInstance(property.PropertyType, new[] { 0 });
                        property.SetValue(result, arrayResult);
                    }
                    else if (innerArray[0].Type == JTokenType.Array)
                    {
                        var arrayResult = (IList)Activator.CreateInstance(property.PropertyType, new[] { innerArray.Count });
                        foreach (var obj in innerArray)
                        {
                            var innerObj = Activator.CreateInstance(objType);
                            arrayResult[count] = ParseObject((JArray)obj, innerObj, objType);
                            count++;
                        }
                        property.SetValue(result, arrayResult);
                    }
                    else
                    {
                        var arrayResult = (IList)Activator.CreateInstance(property.PropertyType, new[] { 1 });
                        var innerObj = Activator.CreateInstance(objType);
                        arrayResult[0] = ParseObject(innerArray, innerObj, objType);
                        property.SetValue(result, arrayResult);
                    }
                    continue;
                }

                var converterAttribute = (JsonConverterAttribute)property.GetCustomAttribute(typeof(JsonConverterAttribute)) ?? (JsonConverterAttribute)property.PropertyType.GetCustomAttribute(typeof(JsonConverterAttribute));
                var value = converterAttribute != null ? arr[attribute.Index].ToObject(property.PropertyType, new JsonSerializer { Converters = { (JsonConverter)Activator.CreateInstance(converterAttribute.ConverterType) } }) : arr[attribute.Index];

                if (value != null && property.PropertyType.IsInstanceOfType(value))
                    property.SetValue(result, value);
                else
                {
                    if (value is JToken token)
                        if (token.Type == JTokenType.Null)
                            value = null;

                    if ((property.PropertyType == typeof(decimal)
                     || property.PropertyType == typeof(decimal?))
                     && (value != null && value.ToString().Contains("e")))
                    {
                        if (decimal.TryParse(value.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var dec))
                            property.SetValue(result, dec);
                    }
                    else
                    {
                        property.SetValue(result, value == null ? null : Convert.ChangeType(value, property.PropertyType));
                    }
                }
            }
            return result;
        }

        public override void WriteJson(JsonWriter writer, object obj, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            var type = obj.GetType();
            var props = type.GetProperties();
            var list = new SortedList<int, (PropertyInfo prop, bool Exclusive)>();
            foreach (var p in props)
            {
                if (p.HasIgnoreAttribute())
                    continue;

                var attr = p.GetCustomAttribute<ArrayPropertyAttribute>();
                if (attr == null)
                    continue;

                list.Add(attr.Index, (p, attr.Exclusive));
            }

            //drop index gaps e.g. -10 0 1 2 3 etc. the gap is -10;0
            var dict = new Dictionary<int, (PropertyInfo prop, bool Exclusive)>(list.Count);
            var i = 0;
            foreach (var kp in list)
            {
                dict.Add(i++, kp.Value);
            }

            var last = -1;

            foreach (var kp in dict)
            {
                var prop = kp.Value.prop;
                var index = kp.Key;

                if (index == last)
                    continue;

                if (!prop.ShouldSerialize(type, obj))
                {
                    if (kp.Value.Exclusive)
                        last += 1;
                    continue;
                }

                while (index != last + 1)
                {
                    writer.WriteValue((string)null);
                    last += 1;
                }

                last = index;

                var value = prop.GetValue(obj);
                writer.WritePropertyValue(prop, value, serializer);

                if (kp.Value.Exclusive)
                    break;
            }

            writer.WriteEndArray();
        }
    }
}
