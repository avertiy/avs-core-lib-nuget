#nullable enable

using System.Collections.Generic;

namespace AVS.CoreLib.Trading.Abstractions.TA
{
    public interface ICalculator
    {
        string Key { get; }
        object? Invoke(IBar bar);
    }

    public interface IPropsCalculator
    {
        Dictionary<string, decimal>? Invoke(IBar bar);
    }
}