using System;
using System.Diagnostics;
using AVS.CoreLib.Trading.Abstractions;
using AVS.CoreLib.Trading.Enums;

namespace AVS.CoreLib.Trading.Models
{
    [DebuggerDisplay("Trade [{Type} {Price}x:{Quantity}={Total}]")]
    public class Trade : ITrade
    {
        public TradeType Type { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal Total { get; set; }
        public DateTime Time { get; set; }
    }
}