using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AVS.CoreLib.Extensions
{
    public static class StringifyExtensions
    {
        /// <summary>
        /// converts input array to string by joining items  
        /// </summary>
        public static string Stringify<T>(this T[] array, string separator = ", ", bool wrapInSquareBrackets = true)
        {
            return wrapInSquareBrackets ? $"[{string.Join(separator, array)}]" : string.Join(separator, array);
        }
        public static string Stringify(this string[] array, string separator = ", ", bool wrapInSquareBrackets = true)
        {
            return wrapInSquareBrackets ? $"[{string.Join(separator, array)}]" : string.Join(separator, array);
        }

        public static string Stringify(this IEnumerable<string> enumerable, string separator = ", ", bool wrapInSquareBrackets = true)
        {
            return wrapInSquareBrackets ? $"[{string.Join(separator, enumerable)}]" : string.Join(separator, enumerable);
        }

        public static string Stringify<TKey,TValue>(this IDictionary<TKey,TValue> dict,
            string separator = ", ",
            string keyValueSeparator = ":",
            bool singleLine = true, 
            bool wrapInSquareBrackets = true,
            int maxStrLength = 0,
            bool addLength = true)
        {
            var sb = new StringBuilder();
            if (wrapInSquareBrackets)
                sb.Append('[');

            var count = 0;
            foreach (var kp in dict)
            {
                var key = kp.Key.ToString();
                var value = kp.Value?.ToString() ?? "null";

                if (singleLine)
                {
                    if (sb.Length > 2)
                    {
                        sb.Append(" ");
                    }

                    sb.Append(key);
                    sb.Append(keyValueSeparator);
                    sb.Append(value);
                    sb.Append(separator);
                }
                else
                {
                    sb.AppendLine();
                    sb.Append(key);
                    sb.Append(keyValueSeparator);
                    sb.Append(value);
                    sb.Append(separator);
                }

                if (maxStrLength > 0 && sb.Length +3 >= maxStrLength)
                {
                    sb.Length = maxStrLength-3;
                    sb.Append("..");
                    break;
                }

                count++;
            }

            if(separator.Length > 0 && sb[^1] == separator.Last())
                sb.Length -= separator.Length;

            if (wrapInSquareBrackets)
                sb.Append(']');

            if (addLength && count > 5)
                sb.Append($"(#{count})");

            return sb.ToString();
        }

        public static string Stringify<T>(this IEnumerable<T> enumerable, Func<T, string> formatter = null, bool wrapInSquareBrackets = true, bool addLength = true)
        {
            var sb = new StringBuilder();
            if (wrapInSquareBrackets)
                sb.Append('[');

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
            
            if(wrapInSquareBrackets)
                sb.Append(']');

            if (addLength && count > 5)
                sb.Append($"(#{count})");

            return sb.ToString();
        }
    }
}