#nullable enable
using AVS.CoreLib.Trading.Enums.Bars;
using AVS.CoreLib.Trading.Enums.TA;

namespace AVS.CoreLib.Trading.Abstractions.XBars
{
    /// <summary> 
    /// Defines TA extension interface to access calculated indicator values such as Moving Averages <see cref="IMAValue"/>, Bollinger Bands <seealso cref="IBBValue"/> etc.
    /// </summary>
    public interface ITAExt
    {
        IMAValue? MA(int length, MAType type = MAType.SMA);
        IBBValue? BB(int length);
    }

    /// <summary>
    /// Common basic indicators set
    /// </summary>
    public interface ITAProps
    {
        IMAValue? MA14 { get; }
        IMAValue? MA21 { get; }
        IMAValue? MA50 { get; }
        IMAValue? MA100 { get; }
        IBBValue? BB21 { get; }
    }
   

    public interface IValue
    {
    }

    public interface IMAValue : IValue
    {
        /// <summary>
        /// moving average value
        /// </summary>
        decimal Average { get; set; }
        decimal Distance { get; set; }
        Color Color { get; set; }
    }

    public interface IBBValue : IMAValue
    {
        decimal UpperBand { get; set; }
        decimal LowerBand { get; set; }
        decimal UpperNarrowBand { get; set; }
        decimal LowerNarrowBand { get; set; }
    }
}