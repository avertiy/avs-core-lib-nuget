﻿using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Guards;

namespace AVS.CoreLib.Extensions.Collections
{
    public static class SliceExtensions
    {
        public static IEnumerable<T[]> Slice<T>(this IList<T> source, int n, int startIndex = 0)
        {
            Guard.MustBe.GreaterThan(n, 0);
            while (startIndex < source.Count)
            {
                var arr = source.Skip(startIndex).Take(n).ToArray();
                yield return arr;
                startIndex += n;
            }
        }

        public static IEnumerable<KeyValuePair<TKey, TValue>[]> Slice<TKey, TValue>(this IDictionary<TKey, TValue> source, int n, int startIndex = 0)
        {
            Guard.MustBe.GreaterThan(n, 0);
            while (startIndex < source.Count)
            {
                var arr = source.Skip(startIndex).Take(n).ToArray();
                yield return arr;
                startIndex += n;
            }
        }
    }

    public static class CropExtensions
    {
        public static T[] Crop<T>(this T[] source, int startIndex, int endIndex)
        {
            Guard.MustBe.WithinRange(startIndex, 0, endIndex);
            Guard.MustBe.WithinRange(endIndex, startIndex, source.Length);

            var list = new List<T>(endIndex - startIndex);
            for (var i = startIndex; i < endIndex; i++)
            {
                list.Add(source[i]);
            }

            return list.ToArray();
        }

        public static IList<T> Crop<T>(this IList<T> source, int startIndex, int endIndex)
        {
            Guard.MustBe.WithinRange(startIndex, 0, endIndex);
            Guard.MustBe.WithinRange(endIndex, startIndex, source.Count);

            var list = new List<T>(endIndex - startIndex);
            for (var i = startIndex; i < endIndex; i++)
            {
                list.Add(source[i]);
            }

            return list;
        }
    }
}