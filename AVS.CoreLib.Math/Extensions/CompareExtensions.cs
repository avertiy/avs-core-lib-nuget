using System.Linq;

namespace AVS.CoreLib.Math.Extensions
{
    public static class CompareExtensions
    {
        public static int GetGreaterValue(this int value, int value2)
        {
            return value >= value2 ? value : value2;
        }

        public static int GetSmallerValue(this int value, int value2)
        {
            return value <= value2 ? value : value2;
        }

        public static bool IsGreaterThan(this int value, params int[] values)
        {
            return values.All(x => x < value);
        }

        public static bool IsSmallerThan(this int value, params int[] values)
        {
            return values.All(x => x > value);
        }
    }
}