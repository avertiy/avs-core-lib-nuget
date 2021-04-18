using System.Diagnostics;
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
    }
}