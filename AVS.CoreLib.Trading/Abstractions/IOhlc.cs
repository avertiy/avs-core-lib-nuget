namespace AVS.CoreLib.Trading.Abstractions
{
    public interface IOhlc
    {
        decimal Open { get; }
        decimal High { get; }
        decimal Low { get; }
        decimal Close { get; }
    }
}