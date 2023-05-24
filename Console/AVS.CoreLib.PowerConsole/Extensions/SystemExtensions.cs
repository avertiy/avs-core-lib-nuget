using System;
using System.Collections.Generic;
using System.Text;
using AVS.CoreLib.PowerConsole.Printers;

namespace AVS.CoreLib.PowerConsole.Extensions
{
    public static class SystemExtensions
    {
        [Obsolete("Use avs.corelib.extensions package")]
        public static string ToKeyValueString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, string keyValueSeparator = " => ", string separator = "\r\n")
        {
            var sb = new StringBuilder();
            foreach (var kp in dictionary)
            {
                sb.Append($"{kp.Key}{keyValueSeparator}{kp.Value}{separator}");
            }
            sb.Length -= separator.Length;
            return sb.ToString();
        }
    }

    internal static class StringExtensions
    {
        
    }
}

