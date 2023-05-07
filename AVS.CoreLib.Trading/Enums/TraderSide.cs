namespace AVS.CoreLib.Trading.Enums
{
    /// <summary>
    /// trader side (Maker/Taker) usually influence an exchange's trade fees
    /// </summary>
    public enum TraderSide
    {
        /// <summary>
        /// trader played a limit order 
        /// </summary>
        Maker,
        /// <summary>
        /// trader placed a market order
        /// </summary>
        Taker,
    }
}