using System;
using System.Collections.Generic;

namespace AVS.CoreLib.Extensions
{
    public static class Enumerable
    {
        public static IEnumerable<dynamic> Create<T>(IList<T> list1, IList<T> list2, Func<T, T, dynamic> selector,
            bool iterateAll = true)
        {
            var i = 0;
            for (; i < list1.Count && i < list2.Count; i++)
                yield return selector(list1[i], list2[i]);

            if (!iterateAll) 
                yield break;

            var n = list1.Count >= list2.Count ? list1.Count : list2.Count;
            for (; i < n; i++)
            {
                if (i < list2.Count)
                    yield return selector(default!, list2[i]);
                else if (i < list1.Count)
                    yield return selector(list1[i], default!);
            }
        }
    }
}