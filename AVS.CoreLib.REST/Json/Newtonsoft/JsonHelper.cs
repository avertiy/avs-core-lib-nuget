#nullable enable
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.REST.Json.Newtonsoft
{
    public static class JsonHelper
    {
        public static Lazy<JsonSerializer> LazySerializer => new Lazy<JsonSerializer>(
            () => new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore });
        public static JsonSerializer Serializer => LazySerializer.Value;

        public static T? Deserialize<T>(JToken jToken)
        {
            return Serializer.Deserialize<T>(jToken.CreateReader());
        }

        internal static bool IsSimpleType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                // nullable type, check if the nested type is simple.
                return IsSimpleType(type.GetGenericArguments()[0]);
            return type.IsPrimitive
                   || type.IsEnum
                   || type == typeof(string)
                   || type == typeof(decimal);
        }

        /// <summary>
        /// populate the JSON values onto the target object
        /// by utilizing JsonSerializer.Populate(jObject.CreateReader(), target); 
        /// </summary>        
        public static void Populate<T>(JToken jToken, T target)
        {
            Serializer.Populate(jToken.CreateReader(), target!);
        }

        public static List<T> ParseList<T>(JArray jArray, Type itemType, Action<T?>? foreachItem = null, Func<T?, bool>? predicate = null)
        {
            var list = new List<T>();
            if (!jArray.HasValues)
                return list;

            foreach (var token in jArray)
            {
                if (token.Type == JTokenType.Object)
                {
                    var value = Deserialize<T>((JObject)token);
                    if (value == null)
                        throw new JsonSerializationException($"Deserialize<{typeof(T).Name}> returned null");
                    foreachItem?.Invoke(value);
                    if (predicate == null || predicate(value))
                        list.Add(value);
                    continue;
                }

                if (token.Type == JTokenType.Array)
                {
                    var value = ParseJArray<T>(itemType, Serializer, (JArray)token);
                    foreachItem?.Invoke(value);
                    if (predicate == null || predicate(value))
                        list.Add(value!);
                    continue;
                }

                throw new JsonReaderException($"Unexpected JToken type {token.Type}");
            }

            return list;
        }

        public static IList<T> ParseList<T>(JObject jObject, Func<string, JToken, T> convertFunc)
        {
            var list = new List<T>();
            if (!jObject.HasValues)
                return list;

            foreach (var kp in jObject)
            {
                var item = convertFunc(kp.Key, kp.Value!);
                list.Add(item);
            }

            return list;
        }

        public static T? Deserialize<T>(JToken token, Type itemType)
        {
            return (T?)Serializer.Deserialize(token.CreateReader(), itemType);
            //try
            //{
            //    return (T?)Serializer.Deserialize(token.CreateReader(), itemType);
            //}
            //catch (Exception ex)
            //{
            //    throw new JsonSerializationException($"Deserialize<{typeof(T).Name}> failed: " + ex.Message, ex);
            //}
        }

        internal static T? ParseJArray<T>(Type itemType, JsonSerializer serializer, JArray token)
        {
            try
            {
                return (T?)serializer.Deserialize(token.CreateReader(), itemType);
            }
            catch (JsonSerializationException ex)
            {
                throw new Exception(
                    $"Unable to parse {itemType.Name} from json array (consider using [ArrayConverter] attribute)", ex);
            }
        }

        public static IDictionary<TKey, TValue> ParseDictionary<TKey, TValue>(JObject jObject,
            Func<dynamic, TKey> keyFunc,
            Func<dynamic, TValue> valFunc
            )
        {
            var dictionary = new Dictionary<TKey, TValue>(jObject.Count);

            if (!jObject.HasValues)
                return dictionary;

            foreach (var kp in jObject)
                if (kp.Value!.Type == JTokenType.String)
                {
                    var value = ((JValue)kp.Value).Value<string>();
                    dictionary.Add(keyFunc(kp.Key), valFunc(value!));
                }
                else if (kp.Value.Type == JTokenType.Boolean)
                {
                    var value = ((JValue)kp.Value).Value<bool>();
                    dictionary.Add(keyFunc(kp.Key), valFunc(value));
                }
                else if (kp.Value.Type == JTokenType.Integer)
                {
                    var value = ((JValue)kp.Value).Value<int>();
                    dictionary.Add(keyFunc(kp.Key), valFunc(value));
                }
                else if (kp.Value.Type == JTokenType.Float)
                {
                    var value = ((JValue)kp.Value).Value<float>();
                    dictionary.Add(keyFunc(kp.Key), valFunc(value));
                }
                else
                {
                    var obj = kp.Value.Value<object>();
                    dictionary.Add(keyFunc(kp.Key), valFunc(obj!));
                }

            return dictionary;
        }
    }
}