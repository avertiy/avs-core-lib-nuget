using System;
using System.Linq;

namespace AVS.CoreLib.Extensions
{
    [Obsolete("Use package AVS.CoreLib.Extensions")]
    public static class EitherExtensions
    {
        public static bool Either(this string value, params string[] values)
        {
            return values.Contains(value);
        }

        public static bool Either(this char @char, params char[] chars)
        {
            return chars.Contains(@char);
        }

        public static bool Either(this int value, params int[] values)
        {
            return values.Contains(value);
        }

        public static bool Either(this decimal value, params decimal[] values)
        {
            return values.Contains(value);
        }
        
        public static bool StartsWithEither(this string value, params string[] values)
        {
            return values.Any(value.StartsWith);
        }

        public static bool EndsWithEither(this string value, params string[] values)
        {
            return values.Any(value.EndsWith);
        }
    }
}