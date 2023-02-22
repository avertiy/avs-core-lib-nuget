namespace AVS.CoreLib.Trading.Enums
{
    public enum OrderType
    {
        Limit = 0,
        Market = 1,
        Immediate = 2,
        /// <summary>
        /// fill or kill
        /// </summary>
        FOK,
        /// <summary>
        /// good till canceled
        /// </summary>
        GTC,
        /// <summary>
        /// one cancels the other
        /// </summary>
        OCO,
        /// <summary>
        /// stop limit
        /// </summary>
        StopLimit,
        /// <summary>
        /// stop by market price i.e. can squeeze
        /// </summary>
        Stop
    }
}