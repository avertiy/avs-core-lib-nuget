#nullable enable
namespace AVS.CoreLib.Trading.Abstractions.TA.Indicators
{
    public interface IBollingerBands : IMovingAverage
    {
        decimal StdDev { get; }
        decimal StdDevNarrow { get; }
    }
}