#nullable enable

using AVS;
using AVS.CoreLib.Trading.Abstractions;

namespace AVS.CoreLib.Trading.Structs
{
    public struct OHLCV : IOhlc
    {
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
    }
}