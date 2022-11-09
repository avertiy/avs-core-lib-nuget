using System;
using System.Collections.Generic;
using System.Linq;

namespace AVS.CoreLib.Extensions
{
    public static class StringExtensions
    {
        public static bool Contains(this string str, string value, int fromIndex = 0)
        {
            if (str.Length < fromIndex + value.Length)
                return false;
            var i = 0;
            while ((i < value.Length) && (str[fromIndex + i] == value[i]))
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

        public static string ReadWord(this string str, int fromIndex = 0)
        {
            if (str.Length <= fromIndex)
                throw new ArgumentOutOfRangeException($"{nameof(fromIndex)} {fromIndex} exceeds {nameof(str)} length {str.Length}");

            var end = str.IndexOfEndOfWord(fromIndex);
            return str.Substring(fromIndex, end - fromIndex);
        }
    }
}
