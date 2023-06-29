namespace AVS.CoreLib.Trading.Enums
{
    public enum OrderType
    {
        /// <summary>
        /// Limit orders will be placed at a specific price. If the price isn't available in the order book for that asset the order will be added in the order book for someone to fill.
        /// </summary>
        Limit =0,
        /// <summary>
        /// Market order will be placed without a price. The order will be executed at the best price available at that time in the order book.
        /// </summary>
        Market = 1,
        Immediate = 2,

        /// <summary>
        /// Maker or "post-only" order is a conditional limit order, that serves to add liquidity to the order book so it is charged with a maker fee. 
        /// If order price crosses market spread the order will be rejected.
        /// </summary>
        LimitMaker = 3,
        /// <summary>
        /// Stop order. Execute a limit order when price reaches a specific Stop price
        /// </summary>
        Stop,
        /// <summary>
        /// Stop market order. Execute a market order when price reaches a specific Stop price
        /// </summary>
        StopMarket,
        /// <summary>
        /// Take profit order. Will execute a limit order when the price rises above a price to sell and therefor take a profit
        /// </summary>
        TakeProfit,
        /// <summary>
        /// Take profit market order. Will execute a market order when the price rises above a price to sell and therefor take a profit
        /// </summary>
        TakeProfitMarket,
        /// <summary>
        /// A trailing stop order will execute an order when the price drops below a certain percentage from its all time high since the order was activated
        /// </summary>
        TrailingStopMarket,
    }
}