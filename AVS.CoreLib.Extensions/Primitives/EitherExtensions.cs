using System;
using System.Linq;

namespace AVS.CoreLib.Extensions
{
    public static class EitherExtensions
    {
        public static bool Either<T>(this T value, params T[] values)
        {
            return values.Contains(value);
        }

        public static bool Either<T>(this T value, params object[] values)
        {
            return values.Any(x => Equals(value, (T)x));
        }

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
    }
}