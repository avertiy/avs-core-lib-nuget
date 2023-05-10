#nullable enable
using System.Collections;
using System.Collections.Generic;
using AVS.CoreLib.Trading.Enums;

namespace AVS.CoreLib.Trading.Abstractions.Bars
{
    /// <summary>
    /// Bar with extra properties lengths, avg price, indicators etc.
    /// </summary>
    public interface IXBar : IBar
    {
        decimal Length { get; set; }
        decimal BodyLength { get; set; }
        BarType Type { get; set; }
        BarSize Size { get; set; }
        bool IncreasedVolume { get; set; }

        /// <summary>
        /// (Open+Close)/2
        /// </summary>
        decimal Avg { get; set; }
        /// <summary>
        /// (High+Low)/2
        /// </summary>
        decimal Mid { get; set; }
        ITopIndicators? Indicators { get; set; }
    }
}