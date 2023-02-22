using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using AVS.CoreLib.REST.Attributes;
using AVS.CoreLib.REST.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.REST.Json.Converters
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

            var obj = Activator.CreateInstance(objectType);
            var arr = JArray.Load(reader);
            return obj.FillObject(objectType, arr);
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

    public static class ArrayDeserializationExtensions
    {
        public static object FillObject(this object obj, Type objectType, JArray arr)
        {
            foreach (var property in objectType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var attribute = property.GetCustomAttribute<ArrayPropertyAttribute>();

                var index = attribute?.Index ?? -1;

                if (index < 0 || index >= arr.Count)
                    continue;

                if (property.PropertyType.BaseType == typeof(Array))
                {
                    SetArrayValue(property, obj, (JArray)arr[index]);
                    continue;
                }

                var value = arr[index].GetValue(property);
                property.SetValueInternal(obj, value);
            }
            return obj;
        }

        private static JsonSerializer GetJsonSerializer(Type type)
        {
            return new JsonSerializer { Converters = { (JsonConverter)Activator.CreateInstance(type) } };
        }

        private static object GetValue(this JToken token, PropertyInfo property)
        {
            var converterAttribute = property.GetCustomAttribute<JsonConverterAttribute>() ??
                                     property.PropertyType.GetCustomAttribute<JsonConverterAttribute>();
            var value = converterAttribute != null ?
                token.ToObject(property.PropertyType, GetJsonSerializer(converterAttribute.ConverterType))
                : token;
            return value;
        }

        private static void SetValueInternal(this PropertyInfo property, object obj, object value)
        {
            if (value != null && property.PropertyType.IsInstanceOfType(value))
                property.SetValue(obj, value);
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
                        property.SetValue(obj, dec);
                }
                else
                {
                    property.SetValue(obj, value == null ? null : Convert.ChangeType(value, property.PropertyType));
                }
            }
        }

        private static void SetArrayValue(this PropertyInfo property, object obj, JArray innerArray)
        {
            var objType = property.PropertyType.GetElementType();
            if (objType == null)
                throw new NullReferenceException($"{nameof(objType)} must be not null");

            var count = 0;
            if (innerArray.Count == 0)
            {
                var arrayResult = (IList)Activator.CreateInstance(property.PropertyType, new[] { 0 });
                property.SetValue(obj, arrayResult);
            }
            else if (innerArray[0].Type == JTokenType.Array)
            {
                var arrayResult = (IList)Activator.CreateInstance(property.PropertyType, new[] { innerArray.Count });
                foreach (var token in innerArray)
                {
                    var innerObj = Activator.CreateInstance(objType);
                    arrayResult[count] = FillObject(innerObj, objType, (JArray)token);
                    count++;
                }

                property.SetValue(obj, arrayResult);
            }
            else
            {
                var arrayResult = (IList)Activator.CreateInstance(property.PropertyType, new[] { 1 });
                var innerObj = Activator.CreateInstance(objType);
                arrayResult[0] = FillObject(innerObj, objType, innerArray);
                property.SetValue(obj, arrayResult);
            }
        }
    }
}
