#nullable enable

using System.Linq;
using AVS.CoreLib.Trading.Abstractions;

namespace AVS.CoreLib.Trading.Structs
{
    public struct OHLCV : IOhlcv
    {
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }

        public static OHLCV From(IBar[] items)
        {
            return new OHLCV()
            {
                Open = items.First().Open,
                High = items.Max(x => x.High),
                Low = items.Min(x => x.Low),
                Close = items.Last().Close,
                Volume = items.Sum(x => x.Volume)
            };
        }
    }
}