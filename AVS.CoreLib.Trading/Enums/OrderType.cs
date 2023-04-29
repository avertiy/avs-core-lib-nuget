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
        /// Stop loss order. Will execute a limit order when the price drops below a price to sell and therefor limit the loss
        /// </summary>
        StopLimit,
        /// <summary>
        /// Stop loss order. Will execute a market order when the price drops below a price to sell and therefor limit the loss
        /// </summary>
        Stop,
        /// <summary>
        /// Take profit order. Will execute a market order when the price rises above a price to sell and therefor take a profit
        /// </summary>
        TakeProfit,
        /// <summary>
        /// Take profit limit order. Will execute a limit order when the price rises above a price to sell and therefor take a profit
        /// </summary>
        TakeProfitLimit,
        /// <summary>
        /// Same as a limit order, however it will fail if the order would immediately match, therefor preventing taker orders
        /// </summary>
        LimitMaker
    }
}