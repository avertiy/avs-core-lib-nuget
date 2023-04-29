namespace AVS.CoreLib.Trading.Enums.Futures
{
    /// <summary>
    /// Type of working
    /// Stop price is triggered by: "MARK_PRICE" or "CONTRACT_PRICE". Default "CONTRACT_PRICE") 
    /// </summary>
    /// <remarks>
    /// MARK PRICE refers to an estimated true value of a contract, it takes into consideration the fair value of an asset
    /// to prevent unnecessary liquidations during a volatile market
    /// Calculated as an average of the Last Price and the underlying asset’s Spot Price to avoid price manipulation of a single order book or exchange.
    /// (проливы и пампы и даже тыки цены на минутке все равно стопы рвёт так что сглаживание ну такое себе, хотя может потому что by default is Contract price)
    /// </remarks>
    public enum WorkingType
    {
        /// <summary>
        /// Contract price type
        /// </summary>
        Contract = 0,

        /// <summary>
        /// Mark price type
        /// </summary>
        Mark
    }
}