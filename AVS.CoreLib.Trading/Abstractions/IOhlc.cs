using System;

namespace AVS.CoreLib.Trading.Abstractions
{
    public interface IOhlc
    {
        decimal Open { get; set; }
        decimal High { get; set; }
        decimal Low { get; set; }
        decimal Close { get; set; }
    }

    [Obsolete("I think this interface better to remove, use IBar if you need volume")]
    public interface IOhlcv : IOhlc
    {
        decimal Volume { get; set; }
    }
}