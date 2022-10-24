using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.Utilities;

namespace AVS.CoreLib.Collections.Extensions
{
    public static class ListExtensions
    {
        public static IList<T> Slice<T>(this IList<T> source, int startIndex, int endIndex)
        {
            Guard.MustBeWithinRange(startIndex, 0, endIndex);
            Guard.MustBeWithinRange(endIndex, startIndex, source.Count);

            var list = new List<T>(endIndex - startIndex);
            for (var i = startIndex; i < endIndex; i++)
            {
                list.Add(source[i]);
            }

            return list;
        }


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

        public static void Merge<T, TKey>(this IList<T> target, IList<T> source, Func<T, TKey> selector,
            Func<T, bool> predicate)
        {
            var knownKeys = new HashSet<TKey>(target.Select(selector));

            foreach (var item in source)
            {
                if (predicate(item) && knownKeys.Add(selector(item)))
                    target.Add(item);
            }
        }

        public static IList<T> Shrink<T>(this IList<T> items, Func<T, double> selector, double threshold = 0.0)
        {
            var avg = items.Average(selector);
            if (threshold <= 0)
                threshold = avg;
            return items.Where(i => selector(i) >= threshold).ToList();
        }

        public static IEnumerable<T> Take<T>(this IList<T> items, IPageOptions options)
        {
            var take = options.Limit > 0 && options.Limit < items.Count ? options.Limit : items.Count;

            for (var i = options.Offset; i < items.Count; i++)
            {
                if (take == 0)
                    break;
                yield return items[i];
                take--;
            }
        }
    }
}