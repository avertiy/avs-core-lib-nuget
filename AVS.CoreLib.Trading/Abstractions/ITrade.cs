using System;
using AVS.CoreLib.Trading.Enums;

namespace AVS.CoreLib.Trading.Abstractions
{
    public interface ITrade
    {
        TradeType Type { get; }
        decimal Price { get; }
        decimal Quantity { get; }
        decimal Total { get; }
        DateTime Time { get; }
    }
}
