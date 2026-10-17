using System;
using System.Diagnostics;
using AVS.CoreLib.Guards;
using Microsoft.VisualBasic;

namespace AVS.CoreLib.Extensions
{
    public static class NumberExtensions
    {
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

        public static int TakeSmallest(this int value, int other)
        {
            return value <= other ? value : other;
        }

        public static int Abs(this int value)
        {
            return value < 0 ? -value : value;
        }

        #endregion

        #region long

        public static long Abs(this long value)
        {
            return value < 0 ? -value : value;
        }

        public static long TakeSmallest(this long value, long other)
        {
            return value <= other ? value : other;
        }

        #endregion
    }
}