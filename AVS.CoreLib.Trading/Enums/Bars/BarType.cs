namespace AVS.CoreLib.Trading.Enums
{
    public enum BarType
    {
        /// <summary>
        /// doji Open=Close
        /// </summary>
        None =0,
        /// <summary>
        /// Open > Close
        /// </summary>
        Bearish = 1,
        /// <summary>
        /// Open < Close
        /// </summary>
        Bullish = 2,
    } 
}