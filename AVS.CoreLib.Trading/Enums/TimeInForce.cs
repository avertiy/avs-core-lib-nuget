namespace AVS.CoreLib.Trading.Enums
{
    public enum TimeInForce
    {
        /// <summary>
        /// good till cancel
        /// </summary>
        GTC = 0,
        /// <summary>
        /// immediate or cancel
        /// </summary>
        IOC =1,
        /// <summary>
        /// Fill or kill
        /// </summary>
        FOK =2,
        /// <summary>
        /// A day order remains in effect only for the given trading session (NYSE/Nasdaq)
        /// </summary>
        DAY = 3,
        /// <summary>
        /// Good Till Crossing (Post Only) 
        /// </summary>
        GTX = 4,
        ///// <summary>
        ///// market open price (NYSE/Nasdaq)
        ///// </summary>
        //MOO =4,
        ///// <summary>
        ///// market on market open price (NYSE/Nasdaq)
        ///// </summary>
        //MOC = 5,
        ///// <summary>
        ///// limit on market open price (NYSE/Nasdaq)
        ///// </summary>
        //LOO = 6,
        ///// <summary>
        ///// limit close price (NYSE/Nasdaq)
        ///// </summary>
        //LOC = 7

    }
}