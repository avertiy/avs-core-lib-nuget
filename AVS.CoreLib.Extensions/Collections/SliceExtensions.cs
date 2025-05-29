using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Guards;

namespace AVS.CoreLib.Extensions.Collections
{
    public static class SliceExtensions
    {
        public static IEnumerable<T[]> Slice<T>(this IList<T> source, int n, int startIndex = 0)
        {
            Guard.MustBe.GreaterThan(n, 0);
            Guard.MustBe.WithinRange(startIndex, 0, source.Count, nameof(startIndex));

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
            Guard.MustBe.WithinRange(startIndex, 0, source.Count, nameof(startIndex));

            while (startIndex < source.Count)
            {
                var arr = source.Skip(startIndex).Take(n).ToArray();
                yield return arr;
                startIndex += n;
            }
        }
    }
}