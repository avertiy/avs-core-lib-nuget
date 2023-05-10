#nullable enable
using AVS.CoreLib.Trading.Enums.TA;

namespace AVS.CoreLib.Trading.Abstractions.Bars
{
    public interface IMovingAverage : IBarExtension
    {
        decimal MA { get; set; }
        /// <summary>
        /// distance from Close price to MA
        /// </summary>
        decimal Distance { get; set; }
        MAColor Color { get; set; }
    }

    public interface IHullMA : IMovingAverage
    {
    }

    public interface I3MA : IBarExtension
    {
        IMovingAverage MA21 { get; set; }
        IMovingAverage MA50 { get; set; }
        IHullMA MA14 { get; set; }
    }

    public class ThreeMA : I3MA
    {
        public IMovingAverage MA21 { get; set; }
        public IMovingAverage MA50 { get; set; }
        public IHullMA MA14 { get; set; }
    }

}