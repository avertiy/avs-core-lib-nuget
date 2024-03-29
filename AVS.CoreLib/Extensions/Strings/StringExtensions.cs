﻿using System;
using System.Linq;
using System.Text;

namespace AVS.CoreLib.Extensions
{
    [Obsolete("Use AVS.CoreLib.Extensions package")]
    public static class StringExtensions
    {
        public static string OneOf(this string value, params string[] values)
        {
            if (values.Contains(value))
                return value;

            throw new ArgumentOutOfRangeException(
                $"{nameof(value)} `{value}` is not one of allowed values: {values.Stringify()}");
        }

        public static string ToCamelCase(this string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Length > 1)
            {
                return Char.ToLowerInvariant(str[0]) + str.Substring(1);
            }

            return str;
        }

        /// <summary>
        /// appends string.Format(format, value); if value is neither null or empty
        /// </summary>
        public static string Append(this string str, string value, string format)
        {
            if (string.IsNullOrEmpty(value))
                return str;
            return str + string.Format(format, value);
        }

        public static string Append<T>(this string str, T value, string format)
        {
            if (Object.Equals(value, default(T)))
                return str;
            return str + string.Format(format, value);
        }

        public static string Truncate(this string str, int maxLength = -1)
        {
            if (string.IsNullOrEmpty(str) || maxLength < 0)
                return str;
            return str.Substring(0, Math.Min(str.Length, maxLength));
        }

        public static bool StartsWith(this string value, params string[] values)
        {
            return values.Any(value.StartsWith);
        }

        public static bool EndsWith(this string value, params string[] values)
        {
            return values.Any(value.EndsWith);
        }

        public static bool EndsWith(this string str, out string end, params string[] values)
        {
            foreach (var val in values)
            {
                if (str.EndsWith(val))
                {
                    end = val;
                    return true;
                }
            }
            end = null;
            return false;
        }

        /// <summary>
        /// It is supposed separator splits str on 2 parts which are swapped 
        /// </summary>
        public static string Swap(this string str, char separator, char newSeparator = '_')
        {
            var parts = str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 2)
                throw new ArgumentException(
                    $"It is supposed separator '{separator}' splits the string '{str}' on 2 parts");
            var swap = parts[1] + newSeparator + parts[0];
            return swap;
        }

        public static string Swap(this string str, string separator = "_", string newSeparator = "_")
        {
            var parts = str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 2)
                throw new ArgumentException(
                    $"It is supposed separator '{separator}' splits the string '{str}' on 2 parts");
            var swap = parts[1] + newSeparator + parts[0];
            return swap;
        }

        public static string ReplaceAll(this string input, string[] values, string replacement = "")
        {
            StringBuilder sb = new StringBuilder(input);
            foreach (var value in values)
            {
                sb.Replace(value, replacement);
            }

            return sb.ToString();
        }
    }
}