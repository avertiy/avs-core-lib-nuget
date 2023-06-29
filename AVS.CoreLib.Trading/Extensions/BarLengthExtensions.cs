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

        /// <summary>
        /// Estimate bar size based on avg bar length  
        /// </summary>
        /// <param name="ohlc">bar</param>
        /// <param name="avgLength">bar length in % e.g. 1% not 0.01</param>
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
        /// Estimate absolute bar size based on a timeframe and assumption that roughly avg bar size on H1 ~1.75% (1.5%-2% for big cap markets like BTC/ETH/TRX etc.)
        /// Thus we can omit avg bar length calculation, hence the absolute size as it is estimated comparatively to absolute mean
        /// </summary>
        /// <param name="ohlc">bar</param>
        /// <param name="timeFrame">timeframe</param>
        /// <param name="avgH1Length">(optional)</param>
        public static BarSize GetBarSizeAbs(this IOhlc ohlc, TimeFrame timeFrame = TimeFrame.M5, decimal avgH1Length = 1.75m)
        {
            // for BTC and most other big cap coins we can assume avg bar lengths as the following:
            // tf:1D ~7-8%
            // tf:1H ~ 1.5-2% 
            // tf:M5 ~0.21%
            var tf = (int)timeFrame;
            var adjustment = tf <= 300 ? 2 : 1;
            var k = 3600 / (tf* adjustment);
            var avgH = avgH1Length / k;
            return ohlc.GetBarSize(avgH);
        }
    }
}