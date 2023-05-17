using System;
using System.Collections.Generic;

namespace AVS.CoreLib.Extensions
{
    public static class Enumerable
    {
        public static IEnumerable<dynamic> Create<T>(IList<T> list1, IList<T> list2, Func<T, T, dynamic> selector, bool iterateAll = true)
        {
            int i = 0;
            for (; i < list1.Count && i < list2.Count; i++)
            {
                yield return selector(list1[i], list2[i]);
            }

            if (iterateAll)
            {
                var n = list1.Count >= list2.Count ? list1.Count : list2.Count;
                for (; i < n; i++)
                {
                    if (i < list2.Count)
                    {
                        yield return selector(default, list2[i]);
                    }
                    else if (i < list1.Count)
                    {
                        yield return selector(list1[i], default);
                    }
                }
            }
        }

        //public static IEnumerable<T> OrderBy<T,Key>(this IEnumerable<T> source, Func<T,Key> selector, OrderBy orderBy)
        //{
        //    if(orderBy == Enums.OrderBy.None)
        //        return source;

        //    return orderBy == Enums.OrderBy.Asc ? source.OrderBy(selector) : source.OrderByDescending(selector);
        //}

        //public static IEnumerable<T> ThenBy<T, Key>(this IOrderedEnumerable<T> source, Func<T, Key> selector, OrderBy orderBy)
        //{
        //    if (orderBy == Enums.OrderBy.None)
        //        return source;

        //    return orderBy == Enums.OrderBy.Asc ? source.ThenBy(selector) : source.ThenByDescending(selector);
        //}
    }
}