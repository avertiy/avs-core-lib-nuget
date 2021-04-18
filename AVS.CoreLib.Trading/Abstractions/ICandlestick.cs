using AVS.CoreLib.Trading.Enums;

namespace AVS.CoreLib.Trading.Abstractions
{
    public interface ICandlestick : IOhlc
    {
        TimeFrame TimeFrame { get; set; } 
    }
}