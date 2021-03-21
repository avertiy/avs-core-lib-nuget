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

        public static void MustBeWithinRange(int value, (int, int) range)
        {
            Range<int> r = range;
            if (!r.Contains(value))
                throw new ArgumentOutOfRangeException($"{nameof(value)} is out of range {range}");
        }
    }
}
    