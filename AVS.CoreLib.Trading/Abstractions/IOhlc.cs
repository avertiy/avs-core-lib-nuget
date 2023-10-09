namespace AVS.CoreLib.Trading.Abstractions
{   
    public interface IOhlc
    {
        decimal Open { get; }
        decimal High { get; }
        decimal Low { get; }
        decimal Close { get; }
    }

    public interface IMutOhlc:IOhlc
    {
        new decimal Open { get; set; }
        new decimal High { get; set; }
        new decimal Low { get; set; }
        new decimal Close { get; set; }
    }

    public interface IOhlcv : IOhlc
    {
        /// <summary>
        /// Amount of base asset
        /// </summary>
        decimal Volume { get; }
    }

    public interface IMutOhlcv : IOhlcv
    {
        /// <summary>
        /// Amount of base asset
        /// </summary>
        new decimal Volume { get; set; }
    }

}