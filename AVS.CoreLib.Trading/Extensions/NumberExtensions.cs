using System;
using System.Globalization;
using AVS.CoreLib.Extensions.Numbers;
using AVS.CoreLib.Trading.Constants;

namespace AVS.CoreLib.Trading.Extensions
{
    public static class NumberExtensions
    {
        /// <summary>
        /// Determines the lowest exp for the value so the value*10^exp. > n
        /// </summary>
        public static int GetLeastExpDiff(this double value, double n)
        {
            //0.025*10^x >= 10
            var x = 0;
            while (value * Math.Pow(10, x) <= n)
                x++;
            return x > 0 ? x - 1 : x;
        }

        public static double DiffPercentage(this double value1, double value2)
        {
            var diff = (value1 - value2);
            var diffPercentage = diff / value1 * 100;
            return diffPercentage;
        }

        public static double NormalizeForDisplay(this double value)
        {
            if (value >= 1)
            {
                if (value > 1000.0)
                    return value.RoundUp(2);
                if (value > 100.0)
                    return value.RoundUp(3);
                if (value > 10.0)
                    return value.RoundUp(4);
                return value.RoundUp(5);
            }
            else
            {
                if (value <= 0)
                    return value.RoundUp(0);
                if (value < 0.0000001)
                    return value.RoundUp(8);
                if (value > 0.1)
                    return value.RoundUp(4);

            }
            return value.RoundUp(8);
        }

        /// <summary>
        /// Rounds value to 8 digits AwayFromZero
        /// </summary>
        public static double Normalize(this double value)
        {
            return Math.Round(value, TradingConstants.PrecisionDigits, MidpointRounding.AwayFromZero);
        }

        public static string ToStringNormalized(this double value)
        {
            return value.ToString("0." + new string('#', TradingConstants.PrecisionDigits), CultureInfo.InvariantCulture);
        }

        public static string ToStringNormalized(this decimal value)
        {
            return value.ToString("0." + new string('#', TradingConstants.PrecisionDigits), CultureInfo.InvariantCulture);
        }

    }
}