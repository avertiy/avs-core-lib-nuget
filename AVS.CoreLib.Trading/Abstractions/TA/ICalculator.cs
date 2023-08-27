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

    //public static class CalculatorExtensions
    //{
    //    public static bool Invoke<T>(this ICalculator calc, IBar bar, out T? result)
    //    {
    //        result = (T?)calc.Invoke(bar);
    //        return result != null;
    //    }

    //    public static T? Invoke<T>(this ICalculator calc, IBar bar)
    //    {
    //        var result = (T?)calc.Invoke(bar);
    //        return result;
    //    }
    //}
}