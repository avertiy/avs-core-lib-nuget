using System;
using System.Linq;

namespace AVS.CoreLib.Extensions.Numbers
{
    public static class NumberExtensions
    {
        #region doubles

        public static double Round(this double value, int decimals)
        {
            return Math.Round(value, decimals, MidpointRounding.AwayFromZero);
        }

        public static double RoundUp(this double value, int decimals)
        {
            var k = Math.Pow(10, decimals);
            return Math.Ceiling((value * k)) / k;
        }
        public static double RoundDown(this double value, int decimals)
        {
            var k = Math.Pow(10, decimals);
            return Math.Floor((value * k)) / k;
        }

        public static double Abs(this double value)
        {
            if (value < 0)
                value = value * -1;
            return value;
        }

        public static bool IsEqual(this double value, double valueToCompare, double tolerance)
        {
            var equal = Math.Abs(value - valueToCompare) <= tolerance;
            return equal;
        }

        #endregion

        #region deciamls
        public static decimal Round(this decimal value, int decimals)
        {
            return Math.Round(value, decimals, MidpointRounding.AwayFromZero);
        }

        public static decimal RoundUp(this decimal value, int decimals)
        {
            var k = (decimal)Math.Pow(10, decimals);
            return Math.Ceiling((value * k)) / k;
        }

        public static decimal RoundUp(this decimal number, decimal round)
        {
            var n = Math.Ceiling((number + round) / round) - 1;
            return (n * round);
        }

        public static decimal RoundDown(this decimal number, decimal round)
        {
            var n = Math.Floor((number + round) / round) - 1;
            return (n * round);
        }

        public static decimal RoundDown(this decimal value, int decimals)
        {
            var k = (decimal)Math.Pow(10, decimals);
            return Math.Floor((value * k)) / k;
        }

        public static decimal Abs(this decimal value)
        {
            if (value < 0)
                value = value * -1;
            return value;
        }

        public static bool IsEqual(this decimal value, decimal valueToCompare, decimal tolerance)
        {
            var equal = Math.Abs(value - valueToCompare) <= tolerance;
            return equal;
        }

        #endregion

        #region int

        public static int RoundUp(this int number, int roundBasis = 10)
        {
            if (number <= 1)
                return number;
            var n = Math.Ceiling((double)(number + roundBasis) / roundBasis) - 1;
            return Convert.ToInt32(n) * roundBasis;
        }

        public static int RoundDown(this int number, int roundBasis = 10)
        {
            if (number <= 1)
                return number;
            var n = Math.Floor((double)(number + roundBasis) / roundBasis) - 1;
            return Convert.ToInt32(n) * roundBasis;
        }

        #endregion
    }
}