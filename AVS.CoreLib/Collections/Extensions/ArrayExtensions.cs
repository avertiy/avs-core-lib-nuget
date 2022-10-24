using System.Collections.Generic;
using AVS.CoreLib.Utilities;

namespace AVS.CoreLib.Collections.Extensions
{
    public static class ArrayExtensions
    {
        public static T[] Slice<T>(this T[] source, int startIndex, int endIndex)
        {
            Guard.MustBeWithinRange(startIndex, 0, endIndex);
            Guard.MustBeWithinRange(endIndex, startIndex, source.Length);

            var list = new List<T>(endIndex - startIndex);
            for (var i = startIndex; i < endIndex; i++)
            {
                list.Add(source[i]);
            }

            return list.ToArray();
        }
    }
}