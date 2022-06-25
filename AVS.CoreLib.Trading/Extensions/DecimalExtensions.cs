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

        public static decimal Normalize(this decimal value, int? decimalPlaces = null)
        {
            int precisionDigits;
            if (value >= 1)
            {
                if (value < 100)
                    precisionDigits = 3;
                else if (value < 10000)
                    precisionDigits = 2;
                else if (value < 100000)
                    precisionDigits = 1;
                else
                    precisionDigits = 0;
            }
            else if (value > 0)
            {
                if (value > 0.1m)
                    precisionDigits = 4;
                else if (value > 0.0000003m)
                    precisionDigits = 8;
                else
                    precisionDigits = TradingConstants.PrecisionDigits;
            }
            else if (value == 0)
            {
                precisionDigits = 0;
            }
            else
            {
                return Normalize(value * -1);
            }

            if (decimalPlaces.HasValue && decimalPlaces < precisionDigits)
                precisionDigits = decimalPlaces.Value;

            return Math.Round(value, precisionDigits, MidpointRounding.AwayFromZero);
        }
    }
}