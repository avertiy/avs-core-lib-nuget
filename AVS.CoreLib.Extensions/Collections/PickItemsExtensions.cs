using System;
using System.Collections.Generic;

namespace AVS.CoreLib.Extensions.Collections
{
    public static class PickItemsExtensions
    {
        /// <summary>
        /// pick items from dictionary values
        /// </summary>
        public static List<TItem> PickItems<TKey, TValue, TItem>(this IDictionary<TKey, TValue> source, Func<TKey, TValue, IEnumerable<TItem>> selector)
        {
            var list = new List<TItem>();
            foreach (var kp in source)
            {
                var items = selector(kp.Key, kp.Value);

                if (items == null)
                    continue;

                list.AddRange(items);
            }

            return list;
        }

        /// <summary>
        /// pick items from dictionary values
        /// </summary>
        public static List<TItem> PickItems<TKey, TValue, TItem>(this IDictionary<TKey, TValue> source, Func<TValue, IEnumerable<TItem>> selector)
        {
            var list = new List<TItem>();
            foreach (var kp in source)
            {
                var items = selector(kp.Value);

                if (items == null)
                    continue;

                list.AddRange(items);
            }

            return list;
        }

        public static List<TItem> PickItems<T, TItem>(this IEnumerable<T> source, Func<T, IEnumerable<TItem>> selector)
        {
            var list = new List<TItem>();
            foreach (var kp in source)
            {
                var items = selector(kp);

                if (items == null)
                    continue;

                list.AddRange(items);
            }

            return list;
        }

        /// <summary>
        /// pick items unique by hashcode from dictionary values
        /// </summary>
        public static HashSet<TItem> PickUniqueItems<TKey, TValue, TItem>(this IDictionary<TKey, TValue> source, Func<TKey, TValue, IEnumerable<TItem>> selector)
        {
            var hashset = new HashSet<TItem>();
            foreach (var kp in source)
            {
                var items = selector(kp.Key, kp.Value);

                if (items == null)
                    continue;

                foreach (var item in items)
                    hashset.Add(item);
            }

            return hashset;
        }

        /// <summary>
        /// pick items unique by hashcode from dictionary values
        /// </summary>
        public static HashSet<TItem> PickUniqueItems<TKey, TValue, TItem>(this IDictionary<TKey, TValue> source, Func<TValue, IEnumerable<TItem>> selector)
        {
            var hashset = new HashSet<TItem>();
            foreach (var kp in source)
            {
                var items = selector(kp.Value);

                if (items == null)
                    continue;

                foreach (var item in items)
                    hashset.Add(item);
            }

            return hashset;
        }

        /// <summary>
        /// pick items unique by key from dictionary values
        /// </summary>
        public static Dictionary<TItemKey, TItem> PickUniqueItems<TKey, TValue, TItemKey, TItem>(this IDictionary<TKey, TValue> source, 
            Func<TValue, IEnumerable<TItem>> selector, Func<TItem, TItemKey> key)
        {
            var dict = new Dictionary<TItemKey, TItem>();
            foreach (var kp in source)
            {
                var items = selector(kp.Value);

                if (items == null)
                    continue;

                foreach (var item in items)
                {
                    var itemKey = key(item);
                    if (dict.ContainsKey(itemKey))
                        continue;

                    dict.Add(itemKey, item);
                }                
            }

            return dict;
        }

        public static HashSet<TItem> PickUniqueItems<T, TItem>(this IEnumerable<T> source, Func<T, IEnumerable<TItem>> selector)
        {
            var hashset = new HashSet<TItem>();
            foreach (var kp in source)
            {
                var items = selector(kp);

                if (items == null)
                    continue;

                foreach (var item in items)
                    hashset.Add(item);
            }

            return hashset;
        }

        public static Dictionary<TItemKey, TItem> PickUniqueItems<T, TItemKey, TItem>(this IEnumerable<T> source,
            Func<T, IEnumerable<TItem>> selector, Func<TItem, TItemKey> key)
        {
            var dict = new Dictionary<TItemKey, TItem>();
            foreach (var kp in source)
            {
                var items = selector(kp);

                if (items == null)
                    continue;

                foreach (var item in items)
                {
                    var itemKey = key(item);
                    if (dict.ContainsKey(itemKey))
                        continue;

                    dict.Add(itemKey, item);
                }
            }

            return dict;
        }
    }
}
