using System;
using AVS.CoreLib.Trading.Abstractions;
using AVS.CoreLib.Trading.Enums;

namespace AVS.CoreLib.Trading.Extensions
{
    public static class OhlcExtensions
    {
        [Obsolete("Method does not look as common/generic purpose, so it will be removed")]
        public static void UpdateOhlc(this IOhlc ohlc, decimal price)
        {
            if (ohlc.Open <= 0)
                ohlc.Open = price;
            else
                ohlc.Close = price;

            if (price > ohlc.High)
                ohlc.High = price;
            else if (price < ohlc.Low || ohlc.Low <= 0)
                ohlc.Low = price;
        }

        [Obsolete("Use GetLength")]
        public static decimal CandleHeight(this IOhlc ohlc)
        {
            return (ohlc.High - ohlc.Low) / ohlc.Low;
        }

        #region color extensions for Japanese green/red candles
        public static ConsoleColor GetCandleColor(this IOhlc ohlc)
        {
            return ohlc.Close >= ohlc.Open ? ConsoleColor.Green : ConsoleColor.Red;
        }

        public static bool IsGreenCandle(this IOhlc ohlc)
        {
            return ohlc.Close > ohlc.Open;
        }

        public static bool IsRedCandle(this IOhlc ohlc)
        {
            return ohlc.Close < ohlc.Open;
        }

        #endregion

        #region classifications by size / type etc.

        public static BarSize GetBarSize(this IOhlc ohlc, TimeFrame timeFrame)
        {
            var h = ohlc.GetLength();
            var size = BarSize.Normal; 
            switch (timeFrame)
            {
                case TimeFrame.M5:
                {
                    if (h < 0.3m)
                        size = BarSize.Small;
                    else if (h > 0.9m)
                        size = BarSize.Paranormal;
                    break;
                }
                case TimeFrame.M30:
                {
                    if (h < 1m)
                        size = BarSize.Small;
                    else if (h > 2.5m)
                        size = BarSize.Paranormal;
                    break;
                }
                case TimeFrame.H1:
                {
                    if (h < 1.5m)
                        size = BarSize.Small;
                    else if (h > 5m)
                        size = BarSize.Paranormal;
                    break;
                }
                case TimeFrame.H4:
                    {
                        if (h < 2.5m)
                            size = BarSize.Small;
                        else if (h > 6m)
                            size = BarSize.Paranormal;
                        break;
                    }
                
                case TimeFrame.D:
                    {
                        if (h < 5m)
                            size = BarSize.Small;
                        else if (h > 15m)
                            size = BarSize.Paranormal;
                        break;
                    }
               
                default:
                    {
                        if (h < 1m)
                            size = BarSize.Small;
                        else if (h > 2.5m)
                            size = BarSize.Paranormal;
                        break;
                    }
            }

            return size;
        }

        /// <summary>
        /// not implemented to classify all candle types but some basic candle classification is possible
        /// </summary>
        public static CandleType GetCandleType(this IOhlc ohlc)
        {
            var body = ohlc.Close - ohlc.Open;
            if (ohlc.IsRedCandle())
            {
                body = ohlc.Open - ohlc.Close;

                if (body > 0)
                {
                    if (ohlc.High == ohlc.Open && ohlc.Close - ohlc.Low > body * 2)
                        return CandleType.HangingMan;

                    if (ohlc.Low == ohlc.Close && ohlc.High - ohlc.Open > body * 2)
                        return CandleType.ShootingStar;

                    if (ohlc.High - ohlc.Open == ohlc.Close - ohlc.Low)
                        return CandleType.Whirligig;
                }
            }

            if (body == 0)
            {
                if (ohlc.Low == ohlc.Close)
                {
                    return ohlc.High == ohlc.Close ? CandleType.TrueDoji : CandleType.Gravestone;
                }

                if (ohlc.High == ohlc.Close)
                    return CandleType.Dragonfly;

                if ((ohlc.Close - ohlc.Low) * 2 < (ohlc.High - ohlc.Close))
                    return CandleType.Gravestone;

                return (ohlc.High - ohlc.Close) * 2 < (ohlc.Close - ohlc.Low) ? CandleType.Dragonfly : CandleType.Doji;
            }

            if (ohlc.High == ohlc.Close && ohlc.Open - ohlc.Low > body * 2)
                return CandleType.Hammer;

            if (ohlc.Low == ohlc.Open && ohlc.High - ohlc.Close > body * 2)
                return CandleType.InverseHammer;

            if (ohlc.Open - ohlc.Low > body * 2 || ohlc.High - ohlc.Close > body * 2)
                return CandleType.LongTail;

            if (ohlc.High - ohlc.Close == ohlc.Open - ohlc.Low)
                return CandleType.Whirligig;

            return CandleType.None;
        }
        #endregion

        /// <summary>
        /// Get candle length in % formula: (High - Low)/Low * 100
        /// </summary>
        public static decimal GetLength(this IOhlc candle)
        {
            return Math.Round((candle.High - candle.Low) / candle.Low * 100, 2);
        }

        public static decimal GetBodyLength(this IOhlc candle)
        {
            return Math.Round((candle.Open > candle.Close
                ? (candle.Open - candle.Close) / candle.Close
                : (candle.Close - candle.Open) / candle.Open) * 100, 2);
        }


        public static decimal GetAvgPrice(this IOhlc ohlc)
        {
            return ((ohlc.Open + ohlc.Close) / 2).PriceRound();
        }

        public static decimal GetMediana(this IOhlc candle)
        {
            return ((candle.High + candle.Low) / 2).PriceRound();
        }

        public static bool HasLongShadow(this IOhlc candle)
        {
            return candle.GetLength() / candle.GetBodyLength() > 2;
        }

        public static bool IsGrowing(this IOhlc candle)
        {
            return candle.Open < candle.Close;
        }

        /// <summary>
        /// determines whether price is within (Low, High) range;
        /// </summary>
        public static bool Contains(this IOhlc candle, decimal price, bool inclusive = false)
        {
            if(inclusive)
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
                "o" => $"{ohlc.Open.PriceRound()}",
                "h" => $"{ohlc.High.PriceRound()}",
                "l" => $"{ohlc.Low.PriceRound()}",
                "c" => $"{ohlc.Close.PriceRound()}",
                _ => ohlc.ToString()
            };
        }
    }
}