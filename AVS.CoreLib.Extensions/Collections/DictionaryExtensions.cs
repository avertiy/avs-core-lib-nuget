using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;

namespace AVS.CoreLib.Extensions.Collections
{
    public static class DictionaryExtensions
    {
	    public static void AddIfNotEmpty(this IDictionary<string, object> dictionary, string key, object value)
	    {
			if(value == null)
                return;
			var str = value.ToString();
            if(string.IsNullOrEmpty(str) || str == "0")
                return;

            dictionary.Add(key, value);
	    }

        /// <summary>
        /// Add <paramref name="other"/> dictionary key/values into a <paramref name="source"/> dictionary        
        /// </summary>
        public static void Add<K, V>(this IDictionary<K, V> source, IDictionary<K, V> other)
        {
            foreach (var kvp in other)
            {
                source.Add(kvp.Key, kvp.Value);                
            }
        }

        /// <summary>
        /// Upsert <paramref name="other"/> dictionary key/values into a <paramref name="source"/> dictionary        
        /// </summary>
        public static void Upsert<K, V>(this IDictionary<K, V> source, IDictionary<K, V> other, bool overwrite = true)
        {
            foreach (var kvp in other)
            {
                // If the key already exists in the first dictionary, update the value.
                if (source.ContainsKey(kvp.Key))
                {
                    if (overwrite)
                        source[kvp.Key] = kvp.Value;
                }
                // If the key does not exist, add it to the merged dictionary.
                else
                {
                    source.Add(kvp.Key, kvp.Value);
                }
            }
        }

        /// <summary>
        /// Merge <paramref name="dict1"/> and <paramref name="dict2"/> into a new dictionary
        /// </summary>
        /// <remarks>if you expect merge into dict1 <see cref="Upsert"/> </remarks>
        public static Dictionary<K, V> Merge<K,V>(this IDictionary<K, V> dict1, IDictionary<K, V> dict2, bool overwrite = true)
        {
            // Create a new dictionary to hold the merged result
            var mergedDictionary = new Dictionary<K, V>(dict1);

            foreach (var kvp in dict2)
            {
                // If the key already exists in the first dictionary, update the value.
                if (mergedDictionary.ContainsKey(kvp.Key) )
                {
                    if(overwrite)
                        mergedDictionary[kvp.Key] = kvp.Value;
                }
                // If the key does not exist, add it to the merged dictionary.
                else
                {
                    mergedDictionary.Add(kvp.Key, kvp.Value);
                }
            }

            return mergedDictionary;
        }

        [DebuggerStepThrough]
        public static IEnumerable<TResult> Select<TKey, TValue, TResult>(this IDictionary<TKey, TValue> dictionary, Func<TValue, TResult> selector)
        {
            var list = new List<TResult>();
            foreach (var kp in dictionary)
            {
                list.Add(selector(kp.Value));
            }
            return list;
        }

        [DebuggerStepThrough]
        public static IEnumerable<TResult> Select<TKey, TValue, TResult>(this IDictionary<TKey, TValue> dictionary, Func<TKey, TValue, TResult> selector)
        {
            var list = new List<TResult>();
            foreach (var kp in dictionary)
            {
                list.Add(selector(kp.Key, kp.Value));
            }
            return list;
        }

        [DebuggerStepThrough]
        public static void ForEach<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Action<TKey, TValue> action)
        {
            foreach (var kp in dictionary)
            {
                action(kp.Key, kp.Value);
            }
        }

        public static NameValueCollection ToNameValueCollection<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            var nameValueCollection = new NameValueCollection();

            foreach (var kvp in dict)
            {
                var value = string.Empty;
                if (kvp.Value != null)
                    value = kvp.Value.ToString();

                nameValueCollection.Add(kvp.Key.ToString(), value);
            }

            return nameValueCollection;
        }

        public static string ToKeyValueString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, string keyValueSeparator = " => ", string separator = "\r\n")
        {
            var sb = new StringBuilder();
            foreach (var kp in dictionary)
            {
                sb.Append($"{kp.Key}{keyValueSeparator}{kp.Value}{separator}");
            }
            sb.Length -= separator.Length;
            return sb.ToString();
        }        
    }
}
