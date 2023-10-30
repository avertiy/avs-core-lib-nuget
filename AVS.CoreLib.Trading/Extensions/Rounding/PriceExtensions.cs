using System;
using System.Linq;
using AVS.CoreLib.Trading.Abstractions;
using AVS.CoreLib.Trading.Helpers;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.Trading.Extensions
{
    public static class PriceExtensions
    {
        #region double
        
        public static double PriceRound(this double price, int? roundDecimals = null, int extraPrecision = 0)
        {
            var dec = roundDecimals ?? price.GetRoundDecimals();
            return Math.Round(price, dec + extraPrecision, MidpointRounding.AwayFromZero);
        }

        public static double PriceRoundUp(this double price, int? roundDecimals = null, int extraPrecision = 0)
        {
            var dec = roundDecimals ?? price.GetRoundDecimals();
            return price.RoundUp(dec + extraPrecision);
        }

        public static double PriceRoundDown(this double price, int? roundDecimals = null, int extraPrecision = 0)
        {
            var dec = roundDecimals ?? price.GetRoundDecimals();
            return price.RoundDown(dec + extraPrecision);
        }

        public static string PriceFormat(this double price, string symbol)
        {
            var quote = symbol.Q();
            var s = CoinHelper.GetCurrencySymbol(quote);

            if (s == "$")
                return price.ToString("C");

            return PriceRound(price) + s;
        }

        #endregion

        #region decimal

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

        //[Obsolete("use Round(this decimal value, int? roundDecimals = null, int extraPrecision = 0, int minPrecision = 0)")]
        //public static decimal PriceRound(this decimal price, int? roundDecimals = null, int extraPrecision = 0)
        //{
        //    var dec = roundDecimals ?? price.GetRoundDecimals();
        //    return price.Round(dec+ extraPrecision);
        //}

        public static decimal PriceRoundUp(this decimal price, int? roundDecimals = null, int extraPrecision = 0)
        {
            var dec = roundDecimals ?? price.GetRoundDecimals();
            return price.RoundUp(dec+ extraPrecision);
        }

        public static decimal PriceRoundDown(this decimal price, int? roundDecimals = null, int extraPrecision = 0)
        {
            var dec = roundDecimals ?? price.GetRoundDecimals();
            return price.RoundDown(dec+ extraPrecision);
        }

        /// <summary>
        /// compare prices if price difference is tiny returns true (about the same), otherwise false
        /// when tolerance is not provided, comparison relies on price meaningful digits
        /// </summary>
        public static bool IsAboutTheSame(this decimal price, decimal priceToCompare, decimal? tolerance = null)
        {
            var diff = (price - priceToCompare).Abs();
            if(tolerance.HasValue)
                return diff / price <= tolerance.Value;

            var k = price.GetK();
            return diff <= k;
        }

        public static bool IsAboutTheSame(this decimal price, decimal price1ToCompare, decimal price2ToCompare, decimal? tolerance = null)
        {
            var diff1 = (price - price1ToCompare).Abs();
            var diff2 = (price - price2ToCompare).Abs();
            if (tolerance.HasValue)
                return diff1 / price <= tolerance.Value || diff2 / price <= tolerance.Value;

            var k = price.GetK();
            return diff1 <= k || diff2 <=k;
        }

        private static decimal GetK(this decimal price)
        {
            var k = price switch
            {
                > 10000 => 1m,
                > 1000 => 0.1m,
                > 100 => 0.01m,
                > 10 => 0.001m,
                > 1 => 0.0001m,
                > 0.1m => 0.00001m,
                _ => 0.000001m
            };

            return k;
        }

        public static bool PriceAnyMatch(this decimal price, params decimal[] values)
        {
            var k = price.GetK();
            return values.Any(x => (price - x).Abs() <= k);
        }

        public static bool IsTouched(this decimal priceLevel, IOhlc bar, decimal toleranceAdjustment = 20)
        {
            return priceLevel.IsTouched(toleranceAdjustment, bar.Low, bar.Close);
        }

        public static bool IsTouched(this decimal priceLevel, decimal toleranceAdjustment = 10, params decimal[] values)
        {
            var k = priceLevel.GetK() * toleranceAdjustment;
            return values.Any(x => (priceLevel - x).Abs() <= k);
        }

        public static bool IsCrossed(this decimal priceLevel, IOhlc bar)
        {
            return bar.Contains(priceLevel);
        }
        #endregion

        public static string PriceFormat(this decimal price, string symbol)
        {
            var quote = symbol.Q();
            var s = CoinHelper.GetCurrencySymbol(quote);

            if (s == "$")
                return price.ToString("C");

            return price.Round() + s;
        }
    }

    public static class VolumeExtensions
    {
        #region double
        public static int GetVolumeRoundDecimals(this double price)
        {
            //on Binance volume round decimals looks as the following:
            //price 5 000$ => 5
            //price > 50$ => 3
            //price > 2$ => 2
            //price > 1$ => 1
            // 0
            return price switch
            {
                > 5_000 => 5,
                > 500 => 4,
                > 50 => 3,
                > 2 => 2,
                > 1 => 1,
                _ => 0
            };
        }

        public static double VolumeRoundUp(this double volume, double price, int? roundDecimals = null)
        {
            var dec = roundDecimals ?? GetVolumeRoundDecimals(price);
            return volume.RoundUp(dec);
        }

        public static double VolumeRound(this double volume, double price, int? roundDecimals = null)
        {
            var dec = roundDecimals ?? GetVolumeRoundDecimals(price);
            return volume.Round(dec);
        }

        public static double VolumeRoundDown(this double volume, double price, int? roundDecimals = null)
        {
            var dec = roundDecimals ?? GetVolumeRoundDecimals(price);
            return volume.RoundDown(dec);
        }


        #endregion

        #region decimal
        public static int GetVolumeRoundDecimals(this decimal price)
        {
            //on Binance volume round decimals looks as the following:
            //price 5 000$ => 5
            //price > 50$ => 3
            //price > 2$ => 2
            //price > 1$ => 1
            // 0
            return price switch
            {
                > 5_000 => 5,
                > 500 => 4,
                > 50 => 3,
                > 2 => 2,
                > 1 => 1,
                _ => 0
            };
        }

        public static decimal VolumeRoundUp(this decimal volume, decimal price, int? roundDecimals = null)
        {
            var dec = roundDecimals ?? GetVolumeRoundDecimals(price);
            return volume.RoundUp(dec);
        }

        public static decimal TotalRoundUp(this decimal volume, decimal price, int? roundDecimals = null)
        {
            var dec = roundDecimals ?? GetVolumeRoundDecimals(price);
            return volume.RoundUp(dec);
        }

        public static decimal VolumeRound(this decimal volume, decimal price, int? roundDecimals = null)
        {
            var dec = roundDecimals ?? GetVolumeRoundDecimals(price);
            return volume.Round(dec);
        }

        public static decimal TotalRound(this decimal volume, decimal price, int? roundDecimals = null)
        {
            var dec = roundDecimals ?? GetVolumeRoundDecimals(price);
            return volume.Round(dec);
        }

        public static decimal VolumeRoundDown(this decimal volume, decimal price, int? roundDecimals = null)
        {
            var dec = roundDecimals ?? GetVolumeRoundDecimals(price);
            return volume.RoundDown(dec);
        }

       
        public static decimal TotalRoundDown(this decimal volume, decimal price, int? roundDecimals = null)
        {
            var dec = roundDecimals ?? GetVolumeRoundDecimals(price);
            return volume.RoundDown(dec);
        }
        #endregion
    }
}