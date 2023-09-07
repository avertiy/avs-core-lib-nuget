#nullable enable
using System;

namespace AVS.CoreLib.Trading.Abstractions
{
    /// <summary>
    /// represent a bar chart unit (synonym candle or simply bar)  
    /// </summary>
    public interface IBar : IOhlcv
    {
        /// <summary>
        /// Amount of quote asset
        /// </summary>
        decimal Total { get; }
        DateTime Time { get; }
    }
}