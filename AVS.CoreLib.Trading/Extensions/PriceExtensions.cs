using System;
using System.Globalization;
using System.Linq;
using AVS.CoreLib.Extensions.Numbers;
using AVS.CoreLib.Trading.Abstractions;
using AVS.CoreLib.Trading.Helpers;

namespace AVS.CoreLib.Trading.Extensions
{
    public static class PriceExtensions
    {
        #region double
        public static int GetRoundDecimals(this double price)
        {
            return price switch
            {
                > 10000 => 0,
                > 1000 => 1,
                > 100 => 2,
                > 10 => 3,
                > 1 => 4,
                > 0.1 => 5,
                > 0.01 => 6,
                > 0.001 => 7,
                _ => 8
            };
        }

        public static double PriceRound(this double price, int? roundDecimals = null)
        {
            var dec = roundDecimals ?? GetRoundDecimals(price);
            return price.Round(dec);
        }

        public static double PriceRoundUp(this double price, int? roundDecimals = null)
        {
            var dec = roundDecimals ?? GetRoundDecimals(price);
            return price.RoundUp(dec);
        }

        public static double PriceRoundDown(this double price, int? roundDecimals = null)
        {
            var dec = roundDecimals ?? GetRoundDecimals(price);
            return price.RoundDown(dec);
        }

        public static string PriceFormat(this double price, string symbol)
        {
            var quote = symbol.GetQuoteCurrency();
            var s = CoinHelper.GetCurrencySymbol(quote);

            if (s == "$")
                return price.ToString("C");

            return PriceRound(price) + s;
        }

        #endregion

        #region decimal
        /// <summary>
        /// determines number of meaningful digits based on price value
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public static int GetRoundDecimals(this decimal price)
        {
            return price switch
            {
                > 10000 => 0,
                > 1000 => 1,
                > 100 => 2,
                > 10 => 3,
                > 1 => 4,
                > 0.1m => 5,
                >0.01m => 6,
                >0.001m => 7,
                _ =>8
            };
        }

        public static decimal PriceRound(this decimal price, int? roundDecimals = null)
        {
            var dec = roundDecimals ?? GetRoundDecimals(price);
            return price.Round(dec);
        }

        public static decimal PriceRoundUp(this decimal price, int? roundDecimals = null)
        {
            var dec = roundDecimals ?? GetRoundDecimals(price);
            return price.RoundUp(dec);
        }

        public static decimal PriceRoundDown(this decimal price, int? roundDecimals = null)
        {
            var dec = roundDecimals ?? GetRoundDecimals(price);
            return price.RoundDown(dec);
        }

        /// <summary>
        /// compare prices if price difference is tiny returns true (about the same), otherwise false
        /// when tolerance is not provided, comparison relies on price meaningful digits see <see cref="GetRoundDecimals(decimal)"/> 
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
            var quote = symbol.GetQuoteCurrency();
            var s = CoinHelper.GetCurrencySymbol(quote);

            if (s == "$")
                return price.ToString("C");

            return PriceRound(price) + s;
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

        public static decimal VolumeRound(this decimal volume, decimal price, int? roundDecimals = null)
        {
            var dec = roundDecimals ?? GetVolumeRoundDecimals(price);
            return volume.Round(dec);
        }

        public static decimal VolumeRoundDown(this decimal volume, decimal price, int? roundDecimals = null)
        {
            var dec = roundDecimals ?? GetVolumeRoundDecimals(price);
            return volume.RoundDown(dec);
        }
        #endregion
    }
}