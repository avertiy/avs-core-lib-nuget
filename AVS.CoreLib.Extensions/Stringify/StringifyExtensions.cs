using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using AVS.CoreLib.Extensions.AutoFormatters;

namespace AVS.CoreLib.Extensions.Stringify
{
    public static class StringifyExtensions
    {
        public static string Stringify(this object obj, string key)
        {
            return obj switch
            {
                string s => s,
                IEnumerable<string> strings => strings.Stringify(StringifyOptions.Default),
                IEnumerable enumerable => enumerable.Stringify(StringifyOptions.Default),
                _ => AutoFormatter.Instance.Format(key, obj)
            };
        }

        public static string Stringify(this object obj)
        {
            return obj switch
            {
                string s => s,
                IEnumerable<string> strings => strings.Stringify(StringifyOptions.Default),
                IEnumerable enumerable => enumerable.Stringify(StringifyOptions.Default),
                _ => AutoFormatter.Instance.Format(obj)
            };
        }

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

        #region Stringify Enumerable
        public static string Stringify(this IEnumerable<string> enumerable, string separator = ", ", bool wrapInSquareBrackets = true)
        {
            return wrapInSquareBrackets ? $"[{string.Join(separator, enumerable)}]" : string.Join(separator, enumerable);
        }

        public static string Stringify<T>(this IEnumerable<T> enumerable, StringifyFormat format = StringifyFormat.Default,
            string separator = ",",
            Func<T, string>? formatter = null,
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
            var l = "..".Length + separator.Length + (brackets ? 1 : 0);
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


        public static string Stringify<T>(this IEnumerable<T> enumerable, StringifyOptions? options = null, Func<T, string>? formatter = null)
        {
            return Stringificator.Instance.Stringify(enumerable, options, formatter);
        }

        public static string Stringify(this IEnumerable enumerable, StringifyOptions? options = null, Func<object, string>? formatter = null)
        {
            var formattedItems = AutoFormatter.Instance.FormatAll(enumerable);
            return Stringificator.Instance.Stringify<string>(formattedItems, options, formatter);
        }

        #endregion

        public static string Stringify<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, StringifyOptions? options = null,
            Func<TKey, TValue, string>? formatter = null)
        {
            return Stringificator.Instance.Stringify(dictionary, options, formatter);
        }
    }


}
