using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AVS.CoreLib.Guards;

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

        public static int IndexOfAny(this string? str, int startIndex, IEnumerable<string> values, StringComparison comparison = StringComparison.InvariantCulture)
        {
            if (str == null)
            {
                return -1;
            }

            var minIndex = str.Length;
            foreach (var value in values)
            {
                var index = str.IndexOf(value, startIndex, comparison);
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

        public static Dictionary<string, int> IndexesOfKeywords(this string str, string[] keywords, StringComparison comparison = StringComparison.InvariantCulture)
        {
            var dict = new Dictionary<string, int>(keywords.Length);

            var startIndex = 0;
            foreach (var keyword in keywords)
            {
                var index = str.IndexOf(keyword, startIndex, comparison);
                dict[keyword] = index;
                if (index >= 0)
                    startIndex = index + keyword.Length;
            }

            return dict;
        }
        #endregion

        public static string[] Split(this string str, string[] separators, StringSplitOptions options = StringSplitOptions.None)
        {
            var result = new List<string>();
            var start = 0;

            // A OR B AND C OR AND A  

            for (var i = 0; i < str.Length; i++)
            {
                foreach (var separator in separators)
                {
                    if (str[i] != separator[0])
                        continue;

                    if (i + separator.Length >= str.Length)
                        continue;

                    var ind = str.IndexOf(separator, i, StringComparison.Ordinal);

                    if (ind != i)
                        continue;

                    add(str.Substring(start, i - start));
                    add(separator);

                    start = i + separator.Length;
                    i += separator.Length;
                }
            }

            if (start < str.Length)
                add(str.Substring(start));

            void add(string part)
            {
                if (options == StringSplitOptions.RemoveEmptyEntries && string.IsNullOrEmpty(part))
                    return;

                var item = options == StringSplitOptions.TrimEntries ? part.Trim() : part;
                result.Add(item);
            }

            return result.ToArray();
        }

        public static string Truncate(this string str, int maxLength = 1000, TruncateOptions options = TruncateOptions.None)
        {
            if (string.IsNullOrEmpty(str) || str.Length <= maxLength)
                return str;

            return options switch
            {
                TruncateOptions.None => str.Substring(0, maxLength),
                TruncateOptions.Json => TruncateJson(str, maxLength),
                TruncateOptions.Text => TruncateText(str, maxLength),
                _ => TruncateText(str, maxLength, options)
            };
        }

        public static string TruncateText(this string str, int maxLength = 1000, TruncateOptions options = TruncateOptions.Text)
        {
            if (string.IsNullOrEmpty(str) || str.Length <= maxLength)
                return str;

            string truncatedStr;

            if (options.HasFlag(TruncateOptions.CutOffTheMiddle))
            {
                var startIndex = maxLength / 2;
                var ind = str.IndexOf(',', startIndex, startIndex / 2);

                if (ind == -1)
                    return str.Substring(0, maxLength);

                var startStr = str.Substring(0, ind + 1);
                var count = maxLength - (ind + 1);
                var ind2 = str.LastIndexOf(',', str.Length - 1, count);

                var endStrLength = (str.Length - 1) - ind2;
                string? endStr = null;
                if (endStrLength < maxLength / 3)
                {
                    count = count - endStrLength;
                    var ind3 = str.LastIndexOf(',', ind2 - 1, count);

                    if (ind3 > 0)
                        endStr = str.Substring(ind3 + 1);
                }

                endStr ??= str.Substring(ind2 + 1);
                truncatedStr = $"{startStr} ... {endStr}";
            }
            else if (options.HasFlag(TruncateOptions.Text))
            {
                truncatedStr = str.Substring(0, maxLength - 3) + ".." + str[^1];
            }
            else
            {
                truncatedStr = str.Substring(0, maxLength);
            }

            if (str.Length > 20 && options.HasFlag(TruncateOptions.AppendLength))
            {
                truncatedStr += $"(Length={str.Length})";
            }

            return truncatedStr;
        }

        /// <summary>
        /// Truncates json cutting the middle if text is too long
        /// </summary>

        public static string TruncateJson(this string str, int maxLength = 1000)
        {
            if (string.IsNullOrEmpty(str) || str.Length <= maxLength)
                return str;

            var startIndex = maxLength / 2;
            var ind = str.IndexOf(',', startIndex, startIndex / 2);

            if (ind == -1)
                return str.Substring(0, maxLength);

            var startStr = str.Substring(0, ind);
            var rest = maxLength - startStr.Length - 6;
            var endStr = str.Substring(str.Length - rest);
            return $"{startStr}, ... {endStr}";
        }

        /// <summary>
        /// retrieves a substring from the end of the initial string
        /// <code>
        /// "abcdef".SubstringFromEnd(2) => "ef";
        /// </code>
        /// </summary>
        public static string SubstringFromEnd(this string str, int length)
        {
            Guard.MustBe.LessThanOrEqual(length, str.Length);
            return str.Substring(str.Length - length);
        }

        public static string TrimEnd(this string str, string end)
        {
            return str.EndsWith(end) ? str.Substring(0, str.Length - end.Length) : str;
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

        /// <summary>
        /// Camel Case - the first letter of the first word is lowercased, and subsequent words are capitalized
        /// this implementation implies that input string is in PascalCase or abbreviation
        /// thus it simply lower case the first letter or if abbreviation is met it tries to lower case it whole
        /// e.g. MyProperty => myProperty, SMA(21) => sma(21), PNL => pnl
        /// </summary>
        public static string ToCamelCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            if (char.IsLower(input[0]))
                return input;

            var count = input.TakeWhile(char.IsUpper).Count();
            return input.Substring(0, count).ToLowerInvariant() + input.Substring(count);
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

    [Flags]
    public enum TruncateOptions
    {
        /// <summary>
        /// truncates string from the end e.g. "abcdefghijklmnoprstuvwxyz".Truncate(6) => "abcdef"
        /// </summary>
        None = 0,
        /// <summary>
        /// truncates text, putting at the end a truncation mark `...` and the last symbol, e.g. "abcdefghijklmnoprstuvwxyz".Truncate(6, TruncateOptions.Text) => "abc..z"
        /// </summary>
        Text = 1,
        /// <summary>
        /// cuts off the middle "abcdefghijklmn".Truncate(8) => "abc..lmn"
        /// </summary>
        CutOffTheMiddle = 2,
        /// <summary>
        /// truncates text  appending length e.g. "abcdefghijklmnoprstuvwxyz".Truncate(6, TruncateOptions.AppendLength) => "abcdef(Length=25)"
        /// </summary>
        AppendLength = 4,
        /// <summary>
        /// truncates json
        /// </summary>
        Json = 8,
        /// <summary>
        /// truncates text  appending length e.g. "abcdefghijklmnoprstuvwxyz".Truncate(6, TruncateOptions.AppendLength) => "abc..z(Length=25)"
        /// </summary>
        TextWithLength = Text | AppendLength,
    }
}
