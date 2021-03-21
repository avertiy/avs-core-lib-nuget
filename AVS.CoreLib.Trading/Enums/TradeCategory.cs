using System;

namespace AVS.CoreLib.Trading.Enums
{
    [Flags]
    public enum TradeCategory
    {
        Exchange = 1,
        MarginTrade = 2,
        Settlement = 4,
        LendingFees = 8
    }
}