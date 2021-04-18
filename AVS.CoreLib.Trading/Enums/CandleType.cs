namespace AVS.CoreLib.Trading.Enums
{
    public enum CandleType
    {
        None = 0,
        Small,
        Average,
        Big,
        LongTail,
        /// <summary>
        /// opens and closes at the same price with bi-directional price fluctuation
        /// </summary>
        Doji,
        LongLeggedDoji,
        TrueDoji,
        /// <summary>
        /// open and close at the same price with upper price fluctuation
        /// </summary>
        Gravestone,
        /// <summary>
        /// open and close at the same price with down price fluctuation
        /// </summary>
        Dragonfly,
        /// <summary>
        /// High - Open = Close - Low or High - Close = Open - Low with short body
        /// </summary>
        Whirligig,
        Hammer,
        /// <summary>
        /// ohlc.High == ohlc.Open and ohlc.Close - ohlc.Low > body * 2
        /// </summary>
        HangingMan,
        /// <summary>
        /// ohlc.Low == ohlc.Open and ohlc.High - ohlc.Close > body * 2
        /// </summary>
        InverseHammer,
        /// <summary>
        /// ohlc.Low == ohlc.Close and ohlc.High - ohlc.Open > body * 2
        /// </summary>
        ShootingStar
    }
}