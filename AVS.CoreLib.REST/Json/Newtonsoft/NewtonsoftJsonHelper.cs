#nullable enable
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.REST.Json.Newtonsoft
{
    internal static class NewtonsoftJsonHelper
    {
        internal static T? Deserialize<T>(JToken? jToken, Type itemType)
        {
            if (jToken == null)
                return default;

            return (T?)JsonService.Serializer.Deserialize(jToken.CreateReader(), itemType);
            //return (T?)Serializer.DeserializeObject(token.ToString(Formatting.None), itemType);
        }

        /// <summary>
        /// populate the JSON values onto the target object
        /// by utilizing JsonSerializer.Populate(jObject.CreateReader(), target); 
        /// </summary>        
        internal static void Populate<T>(JToken jToken, T target)
        {
            JsonService.Serializer.Populate(jToken.CreateReader(), target!);
            //var json = jToken.ToString(Formatting.None);
            //Serializer.Populate(target!, json);
        }
        [Obsolete("seems no usages")]
        private static IDictionary<TKey, TValue> ParseDictionary<TKey, TValue>(JObject jObject,
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
        [Obsolete("seems no usages")]
        private static List<T> ParseList<T>(JArray jArray, Type itemType, Action<T?>? foreachItem = null, Func<T?, bool>? predicate = null)
        {
            var list = new List<T>();
            if (!jArray.HasValues)
                return list;

            foreach (var token in jArray)
            {
                if (token.Type == JTokenType.Object)
                {
                    //var value = Deserialize<T>((JObject)token);
                    var value = JsonHelper.DeserializeObject<T>(token.ToString());
                    if (value == null)
                        throw new JsonSerializationException($"Deserialize<{typeof(T).Name}> returned null");
                    foreachItem?.Invoke(value);
                    if (predicate == null || predicate(value))
                        list.Add(value);
                    continue;
                }

                if (token.Type == JTokenType.Array)
                {
                    var value = ParseJArray<T>(itemType, (JArray)token);
                    foreachItem?.Invoke(value);
                    if (predicate == null || predicate(value))
                        list.Add(value!);
                    continue;
                }

                throw new JsonReaderException($"Unexpected JToken type {token.Type}");
            }

            return list;
        }

        [Obsolete("seems no usages")]
        private static IList<T> ParseList<T>(JObject jObject, Func<string, JToken, T> convertFunc)
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
        [Obsolete("seems no usages")]
        private static T? ParseJArray<T>(Type itemType, JArray token)
        {
            try
            {
                return (T?)JsonHelper.DeserializeObject(token.ToString(Formatting.None), itemType);
            }
            catch (JsonSerializationException ex)
            {
                throw new Exception(
                    $"Unable to parse {itemType.Name} from json array (consider using [ArrayConverter] attribute)", ex);
            }
        }
    }
}