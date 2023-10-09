#nullable enable
using System;
using AVS.CoreLib.Trading.Abstractions.TA;
using AVS.CoreLib.Trading.Enums;
using AVS.CoreLib.Trading.TA.Tools;

namespace AVS.CoreLib.Trading.TA
{
    //bar.TA.Get(TAProp.High, TimeFrame.D)
    //bar.TA.SMA(21)
    //bar.TA.RSI(21)
    //bar.TA.ATR(21)
    /// <summary>
    /// Represent bar extension that aggregate top (main) TA indicators MA (21, 50), BB, etc.
    /// </summary>
    public class XBar : IXBar
    {
        #region IBar (OHLCV+Time) props
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
        public decimal Total { get; set; }
        public DateTime Time { get; set; }
        #endregion

        #region IXBar props

        public BarType Type { get; set; }
        public decimal Length { get; set; }
        public decimal BodyLength { get; set; }

        /// <summary>
        /// (Open+Close)/2
        /// </summary>
        public decimal Avg { get; set; }
        /// <summary>
        /// (High+Low)/2
        /// </summary>
        public decimal Hl2 { get; set; }

        /// <summary>
        /// (High+Low+Close)/3
        /// </summary>
        public decimal Hlc3 { get; set; }

        /// <summary>
        /// Indicates bar size (length) relative to avg bar length
        /// </summary>
        public BarSize SizeAbs { get; set; }

        #endregion        

        public BarSize Size { get; set; }
        public VolumeSize VolumeSize { get; set; }

        public TAExt? TA { get; set; }       
    }
}