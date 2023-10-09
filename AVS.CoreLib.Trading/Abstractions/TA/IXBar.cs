#nullable enable
using AVS;
using AVS.CoreLib.Trading.Enums;
using AVS.CoreLib.Trading.TA.Tools;

namespace AVS.CoreLib.Trading.Abstractions.TA
{
    /// <summary>
    /// XBar is <see cref="IBar"/> with extra properties such as calculated lengths, avg price, size, type etc.
    /// </summary>
    public interface IXBar : IBar
    {
        /// <summary>
        /// Indicates the bar is bullish or bearish
        /// </summary>
        BarType Type { get;  }
        /// <summary>
        /// Bar Length in %, formula: 100*(High - Low)/Low
        /// </summary>
        decimal Length { get; }

        /// <summary>
        /// Candle body length in %, , formula: 100*  (Bullish ? (Close - Open)/Open : (Open - Close)/Close)
        /// </summary>
        decimal BodyLength { get; }

        /// <summary>
        /// (Open+Close)/2
        /// </summary>
        decimal Avg { get; }
        /// <summary>
        /// (High+Low)/2
        /// </summary>
        decimal Hl2 { get; }

        /// <summary>
        /// (High+Low+Close)/3
        /// </summary>
        decimal Hlc3 { get; }

        /// <summary>
        /// absolute size based on a timeframe 
        /// regardless avg bar length
        /// </summary>
        BarSize SizeAbs { get; }

        /// <summary>
        /// TA extension adds calculated values based on moving statistic like ATR, MA, RSI etc.         
        /// </summary>
        TAExt? TA { get; }
        //ITAExt? TA { get; }

        /// <summary>
        /// Indicates how big (long) the bar is relative to avg length
        /// avg length is sma(Length, 12)
        /// </summary>
        BarSize Size { get; }
        /// <summary>
        /// Indicates how big volume is relative to avg volume        
        /// avg volumw is sma(Volume, 12)
        /// </summary>
        VolumeSize VolumeSize { get; }

    }
}