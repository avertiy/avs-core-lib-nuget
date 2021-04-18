namespace AVS.CoreLib.Trading.Abstractions
{
    public interface IOhlc
    {
        decimal Open { get; set; }
        decimal High { get; set; }
        decimal Low { get; set; }
        decimal Close { get; set; }
    }

    public interface IOhlcv : IOhlc
    {
        decimal Volume { get; set; }
    }
}