using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;

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
        
        [DebuggerStepThrough]
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

        [DebuggerStepThrough]
        public static IEnumerable<TResult> Select<TKey, TValue, TResult>(this IDictionary<TKey, TValue> dictionary,
            Func<TValue, TResult> selector)
        {
            var list = new List<TResult>();
            foreach (var kp in dictionary)
            {
                list.Add(selector(kp.Value));
            }
            return list;
        }

        [DebuggerStepThrough]
        public static IEnumerable<TResult> Select<TKey, TValue, TResult>(this IDictionary<TKey, TValue> dictionary,
            Func<TKey, TValue, TResult> selector)
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
    }
}
