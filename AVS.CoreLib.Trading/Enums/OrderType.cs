namespace AVS.CoreLib.Trading.Enums
{
    public enum OrderType
    {
        Limit = 0,
        Market = 1,
        Immediate = 2,
        FillOrKill,
        GoodTillCanceled,
        StopLimit,
        Stop,
        OneCancelsTheOther
    }

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
        FOK =2
    }
}