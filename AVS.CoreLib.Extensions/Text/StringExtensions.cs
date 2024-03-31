using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AVS.CoreLib.Extensions
{
    public static class StringExtensions
    {
        #region Append methods
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

        #endregion

        #region Contains methods
        public static bool ContainsAt(this string str, string value, int index = 0)
        {
            if (str.Length < index + value.Length)
                return false;

            var i = 0;
            while ((i < value.Length) && (str[index + i] == value[i]))
                i++;

            return i == value.Length;
        }

        public static bool ContainsAny(this string str, params char[] symbols)
        {
            return str.Any(t => symbols.Any(x => t == x));
        }

        public static bool ContainsAny(this string str, params string[] values)
        {
            for (var i = 0; i < str.Length; i++)
            {
                var current = char.ToLower(str[i]);
                foreach (var value in values)
                {
                    if (i + value.Length >= str.Length)
                        continue;

                    if (current == char.ToLower(value[0]))
                    {
                        var length = value.Length;
                        var ii = 1;
                        while ((ii < length) && (char.ToLower(str[i + ii]) == char.ToLower(value[ii])))
                            ii++;

                        if (ii == length)
                            return true;
                    }
                }
            }

            return false;
        }

        public static bool ContainsAll(this string str, params string[] values)
        {
            var flags = new List<string>();
            for (var i = 0; i < str.Length; i++)
            {
                var current = char.ToLower(str[i]);
                foreach (var value in values)
                {
                    if (flags.Contains(value))
                        continue;

                    if (i + value.Length >= str.Length)
                        continue;

                    if (current == char.ToLower(value[0]))
                    {
                        var length = value.Length;
                        var ii = 1;
                        while ((ii < length) && (char.ToLower(str[i + ii]) == char.ToLower(value[ii])))
                            ii++;

                        if (ii == length)
                            flags.Add(value);
                        if (flags.Count == values.Length)
                            return true;
                    }
                }
            }

            return false;
        }

        public static bool ContainsAll(this string str, params char[] values)
        {
            var flags = new List<char>();
            for (var i = 0; i < str.Length; i++)
            {
                foreach (var value in values)
                {
                    if (str[i] != value)
                        continue;
                    flags.Add(value);
                    if (flags.Count == values.Length)
                        return true;
                }
            }

            return false;
        }

        public static string OneOf(this string value, params string[] values)
        {
            if (values.Contains(value))
                return value;

            throw new ArgumentOutOfRangeException(
                $"{nameof(value)} `{value}` is not one of allowed values: {string.Join(",", values)}");
        }


        #endregion

        #region StartsWith and EndsWith methods
        public static bool StartsWith(this string value, params string[] values)
        {
            return values.Any(value.StartsWith);
        }

        public static bool StartsWithAny(this string value, params char[] chars)
        {
            return chars.Any(value.StartsWith);
        }

        public static bool StartsWithAny(this string value, params string[] values)
        {
            return values.Any(value.StartsWith);
        }

        public static bool EndsWithAny(this string value, params char[] values)
        {
            return values.Any(value.EndsWith);
        }

        public static bool EndsWithAny(this string value, params string[] values)
        {
            return values.Any(value.EndsWith);
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

        #endregion

        #region Swap methods
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

        #endregion

        #region IndexOfAny / IndexOfEndOfWord / IndexesOfKeywords

        public static int IndexOfAny(this string str, params char[] anyOf)
        {
            return str.IndexOfAny(anyOf);
        }

        public static int IndexOfAny(this string? str, int startIndex, IEnumerable<string> values)
        {
            if (str == null)
            {
                return -1;
            }

            var minIndex = str.Length;
            foreach (var value in values)
            {
                var index = str.IndexOf(value, startIndex, StringComparison.Ordinal);
                if (index >= 0 && index < minIndex)
                {
                    minIndex = index;
                }
            }

            return minIndex == str.Length ? -1 : minIndex;
        }

        public static int IndexOfEndOfWord(this string str, int fromIndex = 0)
        {
            if (str.Length <= fromIndex)
                throw new ArgumentOutOfRangeException($"{nameof(fromIndex)} {fromIndex} exceeds {nameof(str)} length {str.Length}");

            var end = str.Length;
            for (var i = fromIndex + 1; i < str.Length; i++)
            {
                if (char.IsWhiteSpace(str[i]))
                {
                    end = i;
                    break;
                }
            }

            return end;
        }

        public static Dictionary<string, int> IndexesOfKeywords(this string str, params string[] keywords)
        {
            var dict = new Dictionary<string, int>(keywords.Length);

            var startIndex = 0;
            foreach (var keyword in keywords)
            {
                var index = str.IndexOf(keyword, startIndex, StringComparison.Ordinal);
                dict[keyword] = index;
                if (index >= 0)
                    startIndex = index + keyword.Length;
            }

            return dict;
        }
        #endregion

        public static string Truncate(this string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str) || str.Length <= maxLength)
                return str;

            return str.Substring(0, maxLength);
        }

        public static string Truncate(this string str, int maxLength, string append)
        {
            if (string.IsNullOrEmpty(str) || str.Length <= (maxLength + append.Length))
                return str;

            return str.Substring(0, maxLength)+append;
        }

        public static string ReplaceAll(this string input, char[] oldChars, char newChar)
        {
            var sb = new StringBuilder(input);
            foreach (var oldChar in oldChars)
            {
                sb.Replace(oldChar, newChar);
            }
            return sb.ToString();
        }

        public static string ReplaceAll(this string input, string[] values, string replacement = "")
        {
            var sb = new StringBuilder(input);
            foreach (var value in values)
            {
                sb.Replace(value, replacement);
            }

            return sb.ToString();
        }

        public static string ToCamelCase(this string str)
        {
            if (str.Length > 1)
                return str.Length < 4 ? str.ToLower() : char.ToLowerInvariant(str[0]) + str.Substring(1);
            return str;
        }

        public static string Capitalize(this string str)
        {
            return str.Length > 1 ? char.ToUpperInvariant(str[0]) + str.Substring(1) : str;
        }

        public static int Count(this string text, string str)
        {
            var count = 0;
            var index = 0;
            while ((index = text.IndexOf(str, index, StringComparison.Ordinal)) != -1)
            {
                count++;
                index += str.Length;
            }
            return count;
        }
        
        public static string ReadWord(this string str, int fromIndex = 0)
        {
            if (str.Length <= fromIndex)
                throw new ArgumentOutOfRangeException($"{nameof(fromIndex)} {fromIndex} exceeds {nameof(str)} length {str.Length}");

            var end = str.IndexOfEndOfWord(fromIndex);
            return str.Substring(fromIndex, end - fromIndex);
        }

        public static string WrapInQuotes(this string str, char quote = '"')
        {
            return quote + str + quote;
        }

        public static string WrapIn(this string str, string wrap)
        {
            return wrap + str + wrap;
        }

        public static string SanitizePropertyName(this string str, char replacement = '_')
        {
            var sb = new StringBuilder(str.Trim());
            for (var i = 0; i < sb.Length; i++)
            {
                if (!char.IsLetterOrDigit(sb[i]))
                    sb[i] = replacement;
            }

            if (char.IsLetter(sb[0]))
            {
                sb[0] = char.ToUpperInvariant(sb[0]);
                return sb.ToString();
            }

            return replacement + sb.ToString();
        }
    }

}
