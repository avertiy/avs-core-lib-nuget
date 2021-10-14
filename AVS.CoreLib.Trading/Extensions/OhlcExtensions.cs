using System;
using AVS.CoreLib.Trading.Abstractions;
using AVS.CoreLib.Trading.Enums;

namespace AVS.CoreLib.Trading.Extensions
{
    public static class OhlcExtensions
    {
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

        /// <summary>
        /// OHLC height in percentage
        /// </summary>
        public static decimal CandleHeight(this IOhlc ohlc)
        {
            return (ohlc.High - ohlc.Low) / ohlc.Low;
        }

        public static decimal CandleAvgPrice(this IOhlc ohlc)
        {
            return (ohlc.Open + ohlc.Close) / 2;
        }

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

        public static CandleSize GetCandleSize(this IOhlc ohlc, TimeFrame timeFrame = TimeFrame.M30)
        {
            var h = ohlc.CandleHeight();
            var size = CandleSize.Normal;
            switch (timeFrame)
            {
                case TimeFrame.H4:
                {
                    if (h < 0.025m)
                        size = CandleSize.Small;
                    else if (h > 0.06m)
                        size = CandleSize.Big;
                    break;
                }
                case TimeFrame.H1:
                {
                    if (h < 0.015m)
                        size = CandleSize.Small;
                    else if (h > 0.05m)
                        size = CandleSize.Big;
                    break;
                }
                case TimeFrame.D:
                {
                    if (h < 0.05m)
                        size = CandleSize.Small;
                    else if (h > 0.15m)
                        size = CandleSize.Big;
                    break;
                    }
                case TimeFrame.M30:
                default:
                {
                    if (h < 0.01m)
                        size = CandleSize.Small;
                    else if (h > 0.025m)
                        size = CandleSize.Big;
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
    }
}