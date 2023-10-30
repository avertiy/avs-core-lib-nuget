using System;
using AVS.CoreLib.Trading.Abstractions;
using AVS.CoreLib.Trading.Enums;

namespace AVS.CoreLib.Trading.Extensions
{
    public static class OhlcExtensions
    {
        /// <summary>
        /// not implemented to classify all candle types but some basic candle classification is possible
        /// </summary>
        [Obsolete("method is not fully implemented")]
        public static CandleType GetCandleType(this IOhlc ohlc)
        {
            var body = ohlc.Close - ohlc.Open;
            if (ohlc.IsBearish())
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