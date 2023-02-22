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
        FOK =2
    }
}