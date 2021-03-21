using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AVS.CoreLib.Extensions
{
    public static class StringExtensions
    {
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

        /// <summary>
        /// It is supposed separator splits str on 2 parts which are swapped 
        /// </summary>
        public static string Swap(this string str, char separator, char newSeparator = '_')
        {
            var parts = str.Split(separator);
            if (parts.Length > 2)
                throw new ArgumentException($"It is supposed separator '{separator}' splits the string '{str}' on 2 parts");
            var swap = parts[1] + newSeparator + parts[0];
            return swap;
        }

        public static string[] GetMatches(this string input, string regExpression = "@(?<value>\\w+)")
        {
            var regex = new Regex(regExpression);
            var matches = new List<string>();
            foreach (Match match in regex.Matches(input))
            {
                matches.Add(match.Groups["value"].Success ? match.Groups["value"].Value : match.Value);
            }
            return matches.ToArray();
        }

        public static string ReplaceAll(this string input, string[] values, string replacement ="")
        {
            StringBuilder sb = new StringBuilder(input);
            foreach (var value in values)
            {
                sb.Replace(value, replacement);
            }
            return sb.ToString();
        }
    }

    public static class RegexExtensions
    {
        public static string[] Replace(this Regex regex, ref string input, string replacement = "")
        {
            var matches = new List<string>();
            StringBuilder sb = new StringBuilder(input);
            foreach (Match match in regex.Matches(input))
            {
                matches.Add(match.Groups["value"].Success ? match.Groups["value"].Value : match.Value);
                sb.Replace(match.Value, replacement);
            }
            input = sb.ToString().TrimEnd(' ');
            return matches.ToArray();
        }
    }
}