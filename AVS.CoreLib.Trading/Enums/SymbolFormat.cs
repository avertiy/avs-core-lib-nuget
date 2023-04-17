using System;

namespace AVS.CoreLib.Trading.Enums
{
    [Flags]
    public enum SymbolFormat
    {
        None = 0,
        /// <summary>
        /// BTC_USDT
        /// </summary>
        Normalized = 1,
        /// <summary>
        /// BTCUSDT
        /// </summary>
        NoUnderscore = 2,
        /// <summary>
        /// btcusdt
        /// </summary>
        LowerCase =4,
        /// <summary>
        /// USDT_BTC
        /// </summary>
        Flipped = 8
    }
}