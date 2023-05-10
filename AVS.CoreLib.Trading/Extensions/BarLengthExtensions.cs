using System;
using AVS.CoreLib.Trading.Abstractions;
using AVS.CoreLib.Trading.Enums;

namespace AVS.CoreLib.Trading.Extensions
{
    public static class BarLengthExtensions
    {
        #region length measurements
        /// <summary>
        /// Get candle length in % formula: (High - Low)/Low * 100
        /// </summary>
        public static decimal GetLength(this IOhlc candle, int roundDigits = 4)
        {
            var len = (candle.High - candle.Low) / candle.Low * 100;
            return Math.Round(len, roundDigits);
        }

        public static decimal GetBodyLength(this IOhlc candle, int roundDigits = 4)
        {
            var len = (candle.Open > candle.Close
                ? (candle.Open - candle.Close) / candle.Close
                : (candle.Close - candle.Open) / candle.Open) * 100;
            return Math.Round(len, roundDigits);
        }

        /// <summary>
        /// for bullish candles return upper shadow, for bearish - lower
        /// </summary>
        public static decimal GetShadowLength(this IOhlc candle, int roundDigits = 4)
        {
            var shadow = candle.Open > candle.Close
                ? (candle.Close - candle.Low) / ((candle.Close + candle.Low) / 2) * 100
                : (candle.High - candle.Open) / ((candle.High + candle.Open) / 2) * 100;

            return Math.Round(shadow, roundDigits);
        }

        public static (decimal, decimal) GetShadowLengths(this IOhlc candle, int roundDigits = 4)
        {
            var p1 = candle.Open > candle.Close ? candle.Open : candle.Close;
            var p2 = candle.Open <= candle.Close ? candle.Open : candle.Close;
            var upper = (candle.High - p1) / ((candle.High + p1) / 2) * 100;
            var bottom = (p2 - candle.Low) / ((p2 + candle.Low) / 2) * 100;

            return (Math.Round(bottom, roundDigits), Math.Round(upper, roundDigits));
        }
        #endregion

        /// <summary>
        /// long shadow means bar length is a <see cref="k"/> times bigger than body length
        /// </summary>
        public static bool HasLongShadow(this IOhlc bar, int k = 2)
        {
            var avg = (bar.Open + bar.Close) / 2;
            var len = bar.GetLength(8);
            if (avg == 0)
            {
                return len > k;
            }
                
            return len / avg > k;
        }

        public static BarSize GetBarSize(this IOhlc ohlc, decimal avgLength)
        {
            var len = ohlc.GetLength();

            if (len < avgLength)
                return BarSize.Short;

            if (len > 5 * avgLength)
                return BarSize.Paranormal;

            if (len >= 2 * avgLength && len < 5 * avgLength)
                return BarSize.Long;

            return BarSize.Normal;
        }

        /// <summary>
        /// without avg we can assume avg bar length by time frame, that's approximate classification but it works for top crypto coins
        /// </summary>
        public static BarSize GetBarSize(this IOhlc ohlc, TimeFrame timeFrame = TimeFrame.M5)
        {
            // for BTC and most other big cap coins we can assume avg bar lengths as the following:
            // tf:1D ~7-8%
            // tf:1H ~ 1.5-2% 
            // tf:M5 ~0.21%
            var tf = (int)timeFrame;
            var adjustment = tf <= 300 ? 2 : 1; 
            var h0 = 1.75m;
            var k = 3600 / (tf* adjustment);
            var avgH = h0 / k;
            return ohlc.GetBarSize(avgH);
        }
    }
}