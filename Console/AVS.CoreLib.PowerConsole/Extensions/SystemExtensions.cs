using System;
using System.Collections.Generic;
using System.Text;

namespace AVS.CoreLib.PowerConsole.Extensions
{
    public static class SystemExtensions
    {
        [Obsolete("Use avs.corelib.extensions")]
        public static string Truncate(this string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            return str.Substring(0, Math.Min(str.Length, maxLength));
        }

        [Obsolete("Use avs.corelib.extensions Stringify")]
        public static string ToArrayString<T>(this IEnumerable<T> enumerable, Func<T, string> formatter = null, bool addLength = true)
        {
            var sb = new StringBuilder("[");

            var count = 0;
            var inRow = false;
            foreach (var element in enumerable)
            {
                var str = formatter == null ? element.ToString() : formatter(element);
                if (inRow || (count == 0 && str.Length > 10))
                {
                    inRow = true;
                    sb.AppendLine();
                    sb.Append(str);
                    sb.Append(",");
                }
                else
                {
                    if (count > 0)
                    {
                        sb.Append(" ");
                    }
                    sb.Append(str);
                    sb.Append(",");
                }
                count++;
            }
            sb.Length -= 1;
            sb.Append("]");

            if (addLength && count > 5)
                sb.Append($"(#{count})");

            return sb.ToString();
        }

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
}

