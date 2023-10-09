namespace AVS.CoreLib.Trading.Enums.Futures
{
    /// <summary>
    /// Type of futures income
    /// </summary>
    public enum IncomeType
    {
        /// <summary>
        /// Futures trading commission
        /// </summary>
        COMMISSION,        
        /// <summary>
        /// Futures funding fee
        /// </summary>
        FUNDING_FEE,
        /// <summary>
        /// Futures realized profit
        /// </summary>
        REALIZED_PNL,
        /// <summary>
        /// Transfer into account
        /// </summary>
        TRANSFER,
        /// <summary>
        /// Futures welcome bonus
        /// </summary>
        WELCOME_BONUS,
        /// <summary>
        /// Insurance clear
        /// </summary>
        INSURANCE_CLEAR,
        /// <summary>
        /// Referral kickback
        /// </summary>
        REFERRAL_KICKBACK,
        /// <summary>
        /// Commission rebate
        /// </summary>
        COMMISSION_REBATE,
        /// <summary>
        /// Api rebate
        /// </summary>
        API_REBATE,
        /// <summary>
        /// Contest reward
        /// </summary>
        CONTEST_REWARD,
        /// <summary>
        /// Cross collateral transfer
        /// </summary>
        CrossCollateralTransfer,
        /// <summary>
        /// Options premium fee
        /// </summary>
        OPTIONS_PREMIUM_FEE,
        /// <summary>
        /// Options settle profit
        /// </summary>
        OPTIONS_SETTLEMENT_PROFIT,
        /// <summary>
        /// Internal transfer
        /// </summary>
        INTERNAL_TRANSFER,
        /// <summary>
        /// Auto exchange
        /// </summary>
        AUTO_EXCHANGE,
        /// <summary>
        /// Delivered settlement
        /// </summary>
        DELIVERED_SETTLEMENT,
        /// <summary>
        /// Coin swap deposit
        /// </summary>
        COIN_SWAP_DEPOSIT,
        /// <summary>
        /// Coin swap withdraw
        /// </summary>
        COIN_SWAP_WITHDRAW,
        /// <summary>
        /// Position limit increase fee
        /// </summary>
        POSITION_LIMIT_INCREASE_FEE
    }
}