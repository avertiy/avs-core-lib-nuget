namespace AVS.CoreLib.Trading.Enums.Futures
{
    /// <summary>
    /// Position mode
    /// 1. One-way mode [DEFAULT] - you can only hold positions in one direction under one contract.
    ///    Opening positions in both directions would cancel one another out or reduce their sizes.
    /// 
    /// 2. Hedge mode - you can simultaneously hold positions in both long and short directions under the same contract.
    ///    If you open long position on higher TF, but anticipate that in short term the price will move down, you want to open a short position as well
    /// </summary>
    /// <remarks>
    /// Hedge mode is a trading strategy used by futures traders to mitigate their risk exposure to the market. It involves opening two opposite positions, a long and a short, to profit from any market movement while minimizing potential losses.
    /// </remarks>
    public enum PositionSide
    {
        /// <summary>
        /// In ONE-WAY mode they return BOTH i.e. you can identify the actual long/short direction by amount sign
        /// </summary>
        Both = 0,
        Short = 1,
        Long =2
    }
    
    public enum PositionMode
    {
        OneWay = 0,
        Hedge
    }

    /// <summary>
    /// clearly defines position direction SHORT / LONG
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// Undefined means position amount is 0 
        /// On practice the exchange will close current position, and open a new one with the next trade
        /// But for testing purposes it's easier to track one position instead of multiple small ones
        /// </summary>
        None = 0,
        Short = 1,
        Long = 2
    }
}