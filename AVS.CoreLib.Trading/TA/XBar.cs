#nullable enable
using System;
using AVS.CoreLib.Trading.Abstractions.XBars;
using AVS.CoreLib.Trading.Enums;
using AVS.CoreLib.Trading.Enums.TA;

namespace AVS.CoreLib.Trading.TA
{
    /// <summary>
    /// Represent bar extension that aggregate top (main) TA indicators MA (21, 50), BB, etc.
    /// </summary>
    public class XBar : IXBar, ITAProps //ITAExt
    {
        #region IBar props
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
        public decimal Total { get; set; }
        public DateTime Time { get; set; }
        #endregion

        #region IXBar props
        public decimal Length { get; set; }
        public decimal BodyLength { get; set; }
        public BarType Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public BarSize Size { get; set; }
        /// <summary>
        /// absolute size based on a timeframe regardless avg bar length
        /// </summary>
        public BarSize SizeAbs { get; set; }
        /// <summary> 
        /// indicates whether the volume is bigger comparatively to prev 10 bars 
        /// </summary>
        public bool IncreasedVolume { get; set; }
        public decimal Avg { get; set; }
        public decimal Mid { get; set; }
        #endregion

        public IMAValue? MA14 { get; set; }
        public IMAValue? MA21 { get; set; }
        public IMAValue? MA50 { get; set; }
        public IMAValue? MA100 { get; set; }
        public IBBValue? BB21 { get; set; }

        //public virtual IMAValue? MA(int length, MAType type = MAType.SMA)
        //{
        //    if (type != MAType.SMA)
        //        throw new NotSupportedException("At the moment only SMA type supported");

        //    return length switch
        //    {
        //        14 => MA14,
        //        21 => MA21,
        //        50 => MA50,
        //        100 => MA100,
        //        _ => throw new NotSupportedException($"{type}({length}) not supported")
        //    };
        //}

        //public virtual IBBValue? BB(int length)
        //{
        //    return length switch
        //    {
        //        21 => BB21,
        //        _ => throw new NotSupportedException($"BB({length}) not supported")
        //    };
        //}
    }

    public class XBar2 : PropertyBag, ITAProps
    {
        public IMAValue? MA14 => (IMAValue?)Get("MA14");
        public IMAValue? MA21 => (IMAValue?)Get("MA21");//this.MA(21, MAType.SMA)
        public IMAValue? MA50 => (IMAValue?)Get("MA50");
        public IMAValue? MA100 => (IMAValue?)Get("MA100");
        public IBBValue? BB21 => (IBBValue?)Get(nameof(BB21));
    }
}