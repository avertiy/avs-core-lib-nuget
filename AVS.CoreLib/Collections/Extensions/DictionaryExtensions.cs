﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;

namespace AVS.CoreLib.Collections.Extensions
{
    public static class DictionaryExtensions
    {
        [DebuggerStepThrough]
        public static string ToHttpPostString(this IDictionary<string, object> dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));
            if (dictionary.Count == 0)
                return string.Empty;

            var output = string.Empty;
            foreach (var entry in dictionary)
            {
                var valueString = entry.Value as string;
                if (valueString == null)
                {
                    output += "&" + entry.Key + "=" + entry.Value;
                }
                else
                {
                    output += "&" + entry.Key + "=" + valueString.Replace(' ', '+');
                }
            }

            return output.Substring(1);
        }
        [DebuggerStepThrough]
        public static string ToHttpPostString(this IDictionary<string, string> dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));
            if (dictionary.Count == 0)
                return string.Empty;

            var output = string.Empty;
            foreach (var entry in dictionary)
            {
                var valueString = entry.Value;
                output += "&" + entry.Key + "=" + valueString.Replace(' ', '+');
            }

            return output.Substring(1);
        }
        [DebuggerStepThrough]
        public static NameValueCollection ToNameValueCollection<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            var nameValueCollection = new NameValueCollection();

            foreach (var kvp in dict)
            {
                string value = string.Empty;
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
