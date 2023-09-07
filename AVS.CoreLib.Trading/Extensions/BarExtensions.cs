using System;
using AVS.CoreLib.Trading.Abstractions;
using AVS.CoreLib.Trading.Enums;
using AVS.CoreLib.Extensions;
namespace AVS.CoreLib.Trading.Extensions
{
    public static class BarExtensions
    {
        #region BarType bull/bear
        public static bool IsBullish(this IOhlc bar)
        {
            return bar.Open < bar.Close;
        }

        public static bool IsBearish(this IOhlc bar)
        {
            return bar.Close < bar.Open;
        }

        public static BarType GetBarType(this IOhlc bar)
        {
            return bar.Open < bar.Close ? BarType.Bullish : BarType.Bearish;
        }

        #endregion

        public static decimal GetAvgPrice(this IOhlc bar, int? roundDecimals = null)
        {
            return ((bar.Open + bar.Close) / 2).Round(roundDecimals);
        }

        public static decimal GetMediana(this IOhlc bar, int? roundDecimals = null)
        {
            return ((bar.High + bar.Low) / 2).Round(roundDecimals);
        }

        /// <summary>
        /// determines whether price is within (Low, High) range;
        /// </summary>
        public static bool Contains(this IOhlc candle, decimal price, bool inclusive = false)
        {
            if (inclusive)
                return candle.Low <= price && price <= candle.High;
            return candle.Low < price && price < candle.High;
        }

        public static bool IsClosedAbove(this IOhlc bar, decimal priceLevel)
        {
            return bar.Close > priceLevel;
        }

        public static bool IsClosedBelow(this IOhlc bar, decimal priceLevel)
        {
            return bar.Close > priceLevel;
        }

        public static string ToString(this IOhlc ohlc, string format)
        {
            return format switch
            {
                "o" => $"{ohlc.Open.Round()}",
                "h" => $"{ohlc.High.Round()}",
                "l" => $"{ohlc.Low.Round()}",
                "c" => $"{ohlc.Close.Round()}",
                _ => ohlc.ToString()
            };
        }

        public static string ToString(this IBar bar, string format)
        {
            return format switch
            {
                "G" => $"{bar.Time:G}",
                "g" => $"{bar.Time:g}",
                "T" => $"{bar.Time:T}",
                "t" => $"{bar.Time:t}",
                _ => ((IOhlc)bar).ToString(format)
            };
        }

        public static ConsoleColor GetColor(this IOhlc ohlc)
        {
            return ohlc.Close >= ohlc.Open ? ConsoleColor.Green : ConsoleColor.Red;
        }
    }
}