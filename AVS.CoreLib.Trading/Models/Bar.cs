using System;
using System.Diagnostics;
using AVS.CoreLib.Trading.Abstractions;
using AVS.CoreLib.Trading.Enums;
using AVS.CoreLib.Trading.Extensions;

namespace AVS.CoreLib.Trading.Models
{
    [DebuggerDisplay("Bar [{Time} {Open}; {High}; {Low}; {Close}; L={BodyLength}]")]
    public class Bar : IBar
    {
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Total { get; set; }
        public decimal Volume { get; set; }

        public DateTime Time { get; set; }

        public decimal BodyLength => this.GetBodyLength();

        public BarType Type => Open < Close ? BarType.Bullish : Open > Close ? BarType.Bearish : BarType.None;

        public bool IsBearish()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"{Time:g} {Open:C};{High:C};{Low:C};{Close:C} Length={this.GetBodyLength()}";
        }
    }
}