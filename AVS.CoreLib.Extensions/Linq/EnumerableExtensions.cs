using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Extensions.Enums;

namespace AVS.CoreLib.Extensions.Linq
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<TResult> Cast<T, TResult>(this IEnumerable<T> source)
        {
            return new CastIterator<T, TResult>(source);
        }

        public static IEnumerable<TResult> Cast<TResult>(this IEnumerable source, Func<object, TResult> cast)
        {
            return new CastIterator<TResult>(source, cast);
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
                action(item);
        }

        public static IEnumerable<TResult> ConvertAll<T, TResult>(this IEnumerable<T> items, Func<T, TResult> func)
        {
            var res = new List<TResult>();
            foreach (var item in items)
                res.Add(func(item));
            return res;
        }

        public static IEnumerable<T> OrderBy<T, Key>(this IEnumerable<T> source, Func<T, Key> selector, Sort direction)
        {
            if (direction == Enums.Sort.None)
                return source;

            return direction == Enums.Sort.Asc ? source.OrderBy(selector) : source.OrderByDescending(selector);
        }

        //sometimes VS can't resolve a namespace, try to typing OrderBy2(..) it will help to resolve the namespace
        public static IEnumerable<T> OrderBy2<T, Key>(this IEnumerable<T> source, Func<T, Key> selector, Sort direction)
        {
            if (direction == Enums.Sort.None)
                return source;

            return direction == Enums.Sort.Asc ? source.OrderBy(selector) : source.OrderByDescending(selector);
        }

        public static bool IsAscending<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector, int count = 0) where TKey : IComparable<TKey>
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            using var enumerator = source.GetEnumerator();

            if (!enumerator.MoveNext())
                return true; // An empty sequence is considered ascending.

            var previous = selector(enumerator.Current);

            while (enumerator.MoveNext())
            {
                var current = selector(enumerator.Current);

                if (current.CompareTo(previous) < 0)
                    return false;

                previous = current;
                count--;
                if (count == 0)
                    break;
            }

            return true;
        }

        public static bool IsDescending<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector, int count = 0) where TKey : IComparable<TKey>
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            using var enumerator = source.GetEnumerator();

            if (!enumerator.MoveNext())
                return true; // An empty sequence is considered descending.

            var previous = selector(enumerator.Current);

            while (enumerator.MoveNext())
            {
                var current = selector(enumerator.Current);

                if (previous.CompareTo(current) < 0)
                    return false;

                previous = current;

                count--;
                if (count == 0)
                    break;
            }

            return true;
        }

        public static IOrderedEnumerable<T> ThenBy<T, Key>(this IOrderedEnumerable<T> source, Func<T, Key> selector, Sort direction)
        {
            if (direction == Enums.Sort.None)
                return source;

            return direction == Enums.Sort.Asc ? source.ThenBy(selector) : source.ThenByDescending(selector);
        }

        public static IEnumerable<T> Limit<T>(this IEnumerable<T> source, int limit)
        {
            return limit > 0 ? source.Take(limit) : source;
        }

        public static int Count(this IEnumerable col)
        {
            var counter = 0;

            foreach (var item in col)
                counter++;

            return counter;
        }

        public static (TResult? min, TResult? max) MinMax<T, TResult>(this IEnumerable<T> source, Func<T, TResult?> selector)
        {
            var initialized = false;
            TResult? minValue = default;
            TResult? maxValue = default;
            var comparer = Comparer<TResult>.Default;
            foreach (var item in source)
            {
                var currentValue = selector(item);

                if (currentValue == null)
                    continue;

                if (!initialized)
                {
                    initialized = true;
                    minValue = currentValue;
                    maxValue = currentValue;
                }

                if (comparer.Compare(currentValue, minValue) <= 0)
                    minValue = currentValue;

                if (comparer.Compare(currentValue, maxValue) >= 0)
                    maxValue = currentValue;
            }

            return (min: minValue, max: maxValue);
        }
    }
}