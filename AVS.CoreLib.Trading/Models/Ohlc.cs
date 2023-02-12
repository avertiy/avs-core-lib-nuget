using System.Diagnostics;
using System.Linq;
using AVS.CoreLib.Trading.Abstractions;

namespace AVS.CoreLib.Trading.Models
{
    [DebuggerDisplay("OHLC [{Open} {High} {Low} {Close}]")]
    public class Ohlc : IOhlc
    {
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }

        public override string ToString()
        {
            return $"{Open:C};{High:C};{Low:C};{Close:C}";
        }

        public static implicit operator Ohlc(IOhlc[] items)
        {
            return new Ohlc()
            {
                Open = items.First().Open,
                High = items.Max(x => x.High),
                Low = items.Min(x => x.Low),
                Close = items.Last().Close
            };
        }
    }
}