namespace AVS.CoreLib.Trading.Enums
{
    public enum CandleSize
    {
        Normal = 0,
        /// <summary>
        /// a short candle with a little price differences
        /// </summary>
        Small = 1,
        /// <summary>
        /// a candle with a major price differences
        /// </summary>
        Big = 2,
    }
}