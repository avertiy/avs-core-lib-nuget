using System;
using System.Collections.Generic;
using System.Linq;

namespace AVS.CoreLib.Extensions.Collections
{
    public static class ListExtensions
    {
        /// <summary>
        /// Adds the elements of the given collection one-by-one to the end of this list.
        /// </summary>        
        public static int AddRange<T>(this IList<T> list, params T[] items)
        {
            var counter = 0;
            foreach (var item in items)
            {
                list.Add(item);
                counter++;
            }

            return counter;
        }

        /// <summary>
        /// Adds the elements of the given collection one-by-one to the end of this list.
        /// </summary>        
        public static int AddRange<T>(this IList<T> list, IEnumerable<T> collection, bool distinct = false)
        {
            var counter = 0;
            foreach (var item in collection)
            {
                list.Add(item);
                counter++;
            }

            return counter;
        }

        /// <summary>
        /// Adds distinct elements one-by-one to the end of this list.
        /// </summary>        
        public static int AddDistinct<T>(this IList<T> list, params T[] items)
        {
            if(items == null || items.Length == 0)
                return 0;

            var counter = 0;
            foreach (var item in items)
            {
                if (list.Contains(item))
                    continue;

                list.Add(item);
                counter++;
            }
            return counter;
        }

        /// <summary>
        /// Adds distinct elements one-by-one to the end of this list.
        /// </summary>        
        public static int AddDistinct<T>(this IList<T> list, IEnumerable<T>? collection)
        {
            if (collection == null)
                return 0;

            var counter = 0;
            foreach (var item in collection)
            {
                if (list.Contains(item))
                    continue;

                list.Add(item);
                counter++;
            }
            return counter;
        }

        /// <summary>
        /// Adds unique by key items of the given collection to the end of the source list.        
        /// </summary>
        public static int AddRange<T, TKey>(this IList<T> source, IEnumerable<T> items, Func<T, TKey> key)
        {
            var knownKeys = new HashSet<TKey>(source.Select(key));
            var counter = 0;
            foreach (var item in items)
            {
                var itemKey = key(item);
                if (knownKeys.Add(itemKey))
                {
                    source.Add(item);
                    counter++;
                }
            }
            return counter;
        }

        /// <summary>
        /// Adds unique by key items of the given collection to the end of the source list.
        /// if they are satisfying a predicate condition
        /// </summary>
        public static int AddRange<T, TKey>(this IList<T> target, IEnumerable<T> second, Func<T, TKey> key, Func<T, bool> predicate)
        {
            var knownKeys = new HashSet<TKey>(target.Select(key));
            var counter = 0;
            foreach (var item in second)
            {
                if (predicate(item) && knownKeys.Add(key(item)))
                {
                    target.Add(item);
                    counter++;
                }
            }
            return counter;
        }

        

        public static bool ContainsAll<T>(this IList<T> source, params T[] items)
        {
            if (source.Count < items.Length)
                return false;

            var result = true;

            foreach (var item in items)
            {
                if (!source.Contains(item))
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Merge source with another collection into a new list of items, dropping duplicates
        /// </summary>
        public static List<T> Merge<T, TKey>(this IEnumerable<T> source, IEnumerable<T> other, Func<T, TKey> key, Func<(T source, T other),T>? resolve = null)
        {
            var dict = new Dictionary<TKey, T>();
            foreach (var item in source)
            {
                var itemKey = key(item);
                if (dict.ContainsKey(itemKey))
                {
                    dict[itemKey] = resolve != null ? resolve.Invoke((dict[itemKey], item)) : dict[itemKey];
                }
                else
                {
                    dict[itemKey] = item;
                }
            }

            foreach (var item in other)
            {
                var itemKey = key(item);
                if (dict.ContainsKey(itemKey))
                {
                    dict[itemKey] = resolve != null ? resolve.Invoke((dict[itemKey], item)) : dict[itemKey];
                }
                else
                {
                    dict[itemKey] = item;
                }
            }

            return dict.Values.ToList();
        }

        public static IList<T> Shrink<T>(this IList<T> items, Func<T, double> selector, double threshold = 0.0)
        {
            var avg = items.Average(selector);
            if (threshold <= 0)
                threshold = avg;
            return items.Where(i => selector(i) >= threshold).ToList();
        }
    }
}