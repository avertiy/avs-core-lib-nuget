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
/*
        public static string Stringify<TKey,TValue>(this IDictionary<TKey,TValue> dict,
            string separator = ", ",
            string keyValueSeparator = ":",
            bool inlineFormatting = true, 
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

                if (inlineFormatting)
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
*/
        public static string Stringify<TKey, TValue>(this IDictionary<TKey, TValue> dict,
            StringifyFormat format = StringifyFormat.Default,
            string separator = ", ",
            string keyValueSeparator = ":",
            Func<TKey,TValue, string> formatter = null,
            int maxLength = 256
            )
        {
            var brackets = format.HasFlag(StringifyFormat.Brackets);
            var displayCount = format.HasFlag(StringifyFormat.Count);
            var multiLine = format.HasFlag(StringifyFormat.MultiLine);
            var limit = format.HasFlag(StringifyFormat.Limit);
            var padding = " ";
            var sb = new StringBuilder();
            if (brackets)
                sb.Append('[');

            var count = 0;
            var l = "..".Length + separator.Length + (brackets ? 1 : 0);
            var reachedLimit = false;
            foreach (var kp in dict)
            {
                if (reachedLimit)
                {
                    // break if no need to count items  
                    if (!displayCount)
                        break;

                    count++;
                    continue;
                }

                string str = null;
                if (formatter == null)
                {
                    var key = kp.Key.ToString();
                    var value = kp.Value?.ToString() ?? "null";
                    str = key + keyValueSeparator + value;
                }
                else
                {
                    str = formatter(kp.Key, kp.Value);
                }

                if (count == 0 && (multiLine || str.Length > 10))
                    multiLine = true;

                if (multiLine)
                    sb.AppendLine();

                if (padding.Length > 0)
                    sb.Append(padding);

                sb.Append(str);
                sb.Append(separator);
                count++;

                if (limit && (multiLine && count > 20 || sb.Length + l + padding.Length > maxLength))
                {
                    if (multiLine)
                        sb.AppendLine();
                    else
                        sb.Length = maxLength - l - padding.Length;

                    if (padding.Length > 0)
                        sb.Append(padding);

                    sb.Append("..");
                    sb.Append(separator);
                    reachedLimit = true;
                }

                count++;
            }

            if (count > 0 && separator.Length > 0)
                sb.Length -= separator.Length;

            if (multiLine)
                sb.AppendLine();

            if (brackets)
                sb.Append(']');

            if (displayCount && (count > 4 || reachedLimit))
                sb.Append($" (#{count})");

            return sb.ToString();
        }

        public static string Stringify<T>(this IEnumerable<T> enumerable, StringifyFormat format = StringifyFormat.Default,
            string separator = ",",
            Func<T, string> formatter = null,
            int maxLength = 256)
        {
            var brackets = format.HasFlag(StringifyFormat.Brackets);
            var displayCount = format.HasFlag(StringifyFormat.Count);
            var multiLine = format.HasFlag(StringifyFormat.MultiLine);
            var limit = format.HasFlag(StringifyFormat.Limit);
            var padding = " ";

            var sb = new StringBuilder();
            
            if (brackets)
                sb.Append('[');

            var count = 0;
            var l = "..".Length + separator.Length + (brackets ?  1 : 0);
            var reachedLimit = false;
            foreach (var item in enumerable)
            {
                if (reachedLimit)
                {
                    // break if no need to count items  
                    if (!displayCount)
                        break;

                    count++;
                    continue;
                }

                var str = formatter == null ? item.ToString() : formatter(item);

                if (count == 0 && (multiLine || str.Length > 10))
                    multiLine = true;

                if (multiLine)
                    sb.AppendLine();

                if (padding.Length > 0)
                    sb.Append(padding);

                sb.Append(str);
                sb.Append(separator);
                count++;

                if (limit && (multiLine && count > 20 || sb.Length + l + padding.Length > maxLength))
                {
                    if (multiLine)
                        sb.AppendLine();
                    else
                        sb.Length = maxLength - l - padding.Length;

                    if (padding.Length > 0)
                        sb.Append(padding);

                    sb.Append("..");
                    sb.Append(separator);
                    reachedLimit = true;
                }
            }

            if(count > 0 && separator.Length > 0)
                sb.Length -= separator.Length;

            if (multiLine)
                sb.AppendLine();

            if (brackets)
                sb.Append(']');

            if (displayCount && (count > 4 || reachedLimit))
                sb.Append($" (#{count})");

            return sb.ToString();
        }
    }

    [Flags]
    public enum StringifyFormat
    {
        None = 0,
        Brackets = 1,
        Count =2,
        MultiLine = 4,
        Limit = 8,
        Default =  Brackets | Count | Limit
    }
}
