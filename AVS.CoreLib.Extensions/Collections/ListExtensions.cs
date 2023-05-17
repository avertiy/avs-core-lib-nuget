using System;
using System.Collections.Generic;
using System.Linq;

namespace AVS.CoreLib.Extensions.Collections
{
    public static class ListExtensions
    {
        public static void AddRange<T>(this IList<T> source, params T[] items)
        {
            foreach (var item in items)
                source.Add(item);
        }

        public static void AddRange<T>(this IList<T> source, IEnumerable<T> items)
        {
            foreach (var item in items)
                source.Add(item);
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
        /// Merge by key target list with a second list, returns the target list
        /// </summary>
        public static IList<T> Merge<T, TKey>(this IList<T> target, IEnumerable<T> second, Func<T, TKey> selector)
        {
            var knownKeys = new HashSet<TKey>(target.Select(selector));

            foreach (var item in second)
            {
                var added = knownKeys.Add(selector(item));
                if (added)
                    target.Add(item);
            }

            return target;
        }

        /// <summary>
        /// Merge by key source list with the second list, returns the result list
        /// </summary>
        public static IEnumerable<T> Merge<T, TKey>(this IEnumerable<T> source, IEnumerable<T> second, Func<T, TKey> selector)
        {
            var knownKeys = new HashSet<TKey>();

            foreach (var item in source)
            {
                knownKeys.Add(selector(item));
                yield return item;
            }

            foreach (var item in second)
            {
                var added = knownKeys.Add(selector(item));
                if (added)
                    yield return item;
            }
        }

        /// <summary>
        /// Merge by key target list with a second list, items form the second list should also match predicate condition, returns the target list
        /// </summary>
        public static IList<T> Merge<T, TKey>(this IList<T> target, IEnumerable<T> second, Func<T, TKey> keySelector, Func<T, bool> predicate)
        {
            var knownKeys = new HashSet<TKey>(target.Select(keySelector));

            foreach (var item in second)
            {
                if (predicate(item) && knownKeys.Add(keySelector(item)))
                    target.Add(item);
            }

            return target;
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