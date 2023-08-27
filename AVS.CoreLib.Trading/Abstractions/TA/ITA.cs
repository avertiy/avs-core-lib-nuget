#nullable enable
using AVS.CoreLib.Trading.Enums;
using AVS.CoreLib.Trading.Enums.TA;
using AVS.CoreLib.Trading.TA.Indicators;

namespace AVS.CoreLib.Trading.Abstractions.TA
{
    public interface ITa : ITABag, IMainIndicators
    {
        IHTF HTF { get; }
    }

    /// <summary>
    /// Represent a container (bag) of decimal (simple) values and object values
    /// </summary>
    public interface ITABag
    {
        decimal? Get(string key);
        T? Get<T>(string key);
        bool ContainsKey(string key);
    }

    public interface IMainIndicators
    {
        MA? MA(int period, MAType type = MAType.SMA);
        BB? BB(int period = 21);
        ADX? ADX(int period = 14, int smoothing = 14);
        RSI? RSI(int period = 14);
        Stoch? Stoch(int period1 = 14, int smoothing = 5, int period2 = 3);
        MACD? MACD(int period1 = 12, int period2 = 26, int signal = 9);
    }

    public interface IHTF
    {
        MA? MA(HTFEnum htf);
        BB? BB(HTFEnum htf);
        MA? MA(TimeFrame timeframe = TimeFrame.D);
        BB? BB(TimeFrame timeframe = TimeFrame.D);
    }
}