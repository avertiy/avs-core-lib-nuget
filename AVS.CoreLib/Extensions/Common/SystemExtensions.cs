using System;
using System.Collections.Generic;

namespace AVS.CoreLib.Extensions
{
    public static class SystemExtensions
    {
        [Obsolete("use stringify extension")]    
        public static string AsString(this string[] arr, string separator = ", ")
        {
            return string.Join(separator, arr);
        }

        [Obsolete("use stringify extension")]
        public static string AsString(this IEnumerable<string> arr, string separator = ", ")
        {
            return string.Join(separator, arr);
        }
    }
}
