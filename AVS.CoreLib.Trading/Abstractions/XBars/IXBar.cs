#nullable enable
using AVS.CoreLib.Trading.Enums;

namespace AVS.CoreLib.Trading.Abstractions.XBars
{
    /// <summary>
    /// XBar is <see cref="IBar"/> with extra properties such as calculated lengths, avg price, size, type etc.
    /// </summary>
    public interface IXBar : IBar
    {
        decimal Length { get; set; }
        decimal BodyLength { get; set; }
        BarType Type { get; set; }
        /// <summary>
        /// Size relative to other bars based on avg bar length metric
        /// </summary>
        BarSize Size { get; set; }
        /// <summary>
        /// absolute size based on a timeframe i.e. M5 
        /// regardless avg bar length
        /// </summary>
        BarSize SizeAbs { get; set; }
        bool IncreasedVolume { get; set; }

        /// <summary>
        /// (Open+Close)/2
        /// </summary>
        decimal Avg { get; set; }
        /// <summary>
        /// (High+Low)/2
        /// </summary>
        decimal Mid { get; set; }
    }
}