using System;
using AVS.CoreLib.Abstractions;
using AVS.CoreLib.Structs;

namespace AVS.CoreLib.Utilities
{
    public static class Guard
    {
        public static void AgainstNullOrEmpty(string param, string name = null)
        {
            if (string.IsNullOrEmpty(param))
            {
                throw new ArgumentNullException(name ?? "The Guarded argument was null or empty.");
            }
        }

        public static void AgainstNull(object param, string name = null)
        {
            if (param == null)
            {
                throw new ArgumentNullException(name ?? "The Guarded argument was null.");
            }
        }

        public static void AgainstDateTimeMin(DateTime param, string name = null)
        {
            if (param == DateTime.MinValue)
            {
                throw new ArgumentNullException(name ?? "The Guarded argument was DateTime min.");
            }
        }

        public static void MustBeEqual(string str1, string str2)
        {
            if (!str1.Equals(str2))
                throw new ArgumentException($"'{str1}' expected to be equal '{str2}'");
        }

        public static void MustHaveValue(IHasValue obj)
        {
            if (obj == null || !obj.HasValue)
                throw new ArgumentNullException(nameof(obj));
        }

        public static void MustBeWithinRange(int value, Range<int> range)
        {
            if (!range.Contains(value))
                throw new ArgumentOutOfRangeException($"{nameof(value)} is out of range {range}");
        }

        public static void MustBeWithinRange(int value, (int from, int to) range)
        {
            if (value < range.from || value > range.to)
                throw new ArgumentOutOfRangeException($"{nameof(value)} is out of range {range}");
        }

        public static void MustBeGreaterThan(int value, int number = 0)
        {
            if (value <= number)
                throw new ArgumentException($"{nameof(value)} must be greater than {value}");
        }

        public static void MustBeGreaterThan(double value, double number = 0)
        {
            if (value <= number)
                throw new ArgumentException($"{nameof(value)} must be greater than {value}");
        }

        public static void MustBeGreaterThan(decimal value, decimal number = 0)
        {
            if (value <= number)
                throw new ArgumentException($"{nameof(value)} must be greater than {value}");
        }
    }
}
