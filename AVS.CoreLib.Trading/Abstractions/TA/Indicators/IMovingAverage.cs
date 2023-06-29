#nullable enable
namespace AVS.CoreLib.Trading.Abstractions.TA.Indicators
{
    public interface IMovingAverage : ITAIndicator
    {
        int Length { get; }
    }

    public interface IHullMA : IMovingAverage
    {
    }
}