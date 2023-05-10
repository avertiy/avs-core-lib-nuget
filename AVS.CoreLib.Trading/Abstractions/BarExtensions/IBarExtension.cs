#nullable enable
namespace AVS.CoreLib.Trading.Abstractions.Bars
{
    /// <summary>
    /// Represent an extension for bar properties
    /// for example we can calc moving average value at this bar, bollinger bands etc. 
    /// </summary>
    public interface IBarExtension
    {
    }

    /// <summary>
    /// Top indicators
    /// </summary>
    public interface ITopIndicators : IBarExtension
    {
        I3MA MA { get; set; }
        IBollingerBands BB { get; set; }
    }

    
    /// <summary>
    /// these are my indicators: 3MAs + BB + RSI
    /// </summary>
    public class TopIndicators : ITopIndicators
    {
        public I3MA MA { get; set; }
        public IMovingAverage MA50 { get; set; }
        public IHullMA MA14 { get; set; }
        public IBollingerBands BB { get; set; }
        public IBarExtension[] GetAll()
        {
            ITopIndicators i;
            i.MA.
            return new IBarExtension[] { MA14, MA21, MA50, BB };
        }
    }
}