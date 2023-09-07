using System;

namespace AVS.CoreLib.Trading.Abstractions
{
    /*
    public interface IOhlc
    {
        decimal Open { get; set; }
        decimal High { get; set; }
        decimal Low { get; set; }
        decimal Close { get; set; }
    }

    public interface IOhlcv : IOhlc
    {
        /// <summary>
        /// Amount of base asset
        /// </summary>
        decimal Volume { get; set; }
    }
    */
    public interface IOhlc
    {
        decimal Open { get; }
        decimal High { get; }
        decimal Low { get; }
        decimal Close { get; }
    }

    public interface IOhlcv : IOhlc
    {
        /// <summary>
        /// Amount of base asset
        /// </summary>
        decimal Volume { get; }
    }

}