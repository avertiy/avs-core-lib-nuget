using System;
using AVS.CoreLib.Trading.Constants;

namespace AVS.CoreLib.Trading.Extensions
{
    public static class DecimalExtensions
    {
        public static decimal Fraction(this decimal value, int percent)
        {
            return value / 100 * percent;
        }

        /// <summary>
        /// deducts specified percent from the value
        /// e.g. commission 0.25%, buy 100 xrp in real you will get 100-0.25% = 99.75 xrp 
        /// </summary>
        public static decimal Deduct(this decimal value, decimal percent)
        {
            return value * (1 - percent / 100);
        }

        public static bool Eq(this decimal value1, decimal value2)
        {
            return Math.Abs(value1 - value2) < TradingConstants.OneSatoshi;
        }

        public static decimal Normalize(this decimal value)
        {
            return Math.Round(value, TradingConstants.PrecisionDigits, MidpointRounding.AwayFromZero);
        }
    }
}