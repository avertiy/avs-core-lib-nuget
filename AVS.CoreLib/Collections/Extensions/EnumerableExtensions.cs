using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AVS.CoreLib.Collections.Extensions
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
                action(item);
        }

        public static IEnumerable<TResult> ConvertAll<T, TResult>(this IEnumerable<T> items, Func<T, TResult> func)
        {
            var res = new List<TResult>();
            foreach (var item in items)
            {
                res.Add(func(item));
            }
            return res;
        }

        public static IEnumerable<string> Match(this IEnumerable<string> items, string pattern)
        {
            var re = new Regex(pattern);
            return items.Where(i => re.IsMatch(i));
        }
    }
}