using System;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Trading.Constants;

namespace AVS.CoreLib.Trading.Extensions
{
    public static class DecimalExtensions
    {
        public static int GetPricePrecision(this decimal price)
        {
            if (price >= 10_000)
                return 0;

            if (price >= 100)
                return 2;

            var dec = price.GetDecimalPlaces();

            return price switch
            {
                >= 1 => dec > 2 ? 3 : 2,
                >= 0.1m => dec > 3 ? 4 : 3,
                >= 0.01m => dec > 4 ? 5 : 4,
                >= 0.001m => dec > 5 ? 6 : 5,
                >= 0.0001m => dec > 6 ? 7 : 6,
                >= 0.00001m => dec > 7 ? 8 : 7,
                _ => 8
            };
        }


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

        [Obsolete("User PriceRound or VolumeRound extensions instead")]
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