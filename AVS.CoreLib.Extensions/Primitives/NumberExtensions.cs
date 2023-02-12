using System;

namespace AVS.CoreLib.Extensions
{
    public static class NumberExtensions
    {
        #region doubles

        public static double Round(this double value, int decimals)
        {
            return Math.Round(value, decimals, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// determines number of meaningful digits based on price value
        /// </summary>
        public static int GetRoundDecimals(this double value)
        {
            return value switch
            {
               // > 10000 => 0,
               // > 1000 => 1,
                > 100 => 2,
                > 10 => 3,
                > 1 => 4,
                > 0.1 => 5,
                > 0.01 => 6,
                > 0.001 => 7,
                _ => 8
            };
        }

        public static double Round(this double value, int? roundDecimals = null)
        {
            var dec = roundDecimals ?? GetRoundDecimals(value);
            return Math.Round(value, dec, MidpointRounding.AwayFromZero);
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

        public static bool IsNotEqual(this double value, double valueToCompare, double tolerance)
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

        /// <summary>
        /// determines number of meaningful digits based on price value
        /// </summary>
        public static int GetRoundDecimals(this decimal value)
        {
            return value switch
            {
                > 100 => 2,
                > 10 => 3,
                > 1 => 4,
                > 0.1m => 5,
                > 0.01m => 6,
                > 0.001m => 7,
                _ => 8
            };
        }

        public static decimal Round(this decimal value, int? roundDecimals = null, int extraPrecision = 0, int minPrecision =0)
        {
            var dec = (roundDecimals ?? GetRoundDecimals(value)) + extraPrecision;
            if (minPrecision > 0 && dec < minPrecision)
                dec = minPrecision;
            return Math.Round(value, dec, MidpointRounding.AwayFromZero);
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

        public static bool IsNotEqual(this decimal value, decimal valueToCompare, decimal tolerance)
        {
            var equal = Math.Abs(value - valueToCompare) <= tolerance;
            return !equal;
        }

        public static decimal Sqrt(this decimal value)
        {
            return (decimal)Math.Sqrt((double)(value));
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