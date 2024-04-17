using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Extensions.Enums;

namespace AVS.CoreLib.Extensions.Linq;

public static class EnumerableAggregateExtensions
{
    public static decimal Aggregate<T>(this IEnumerable<T> source, Func<T, decimal> selector, AggregateFn fn)
    {
        switch (fn)
        {
            case AggregateFn.Max:
                return source.Max(selector);
            case AggregateFn.Min:
                return source.Min(selector);
            case AggregateFn.Sum:
                return source.Sum(selector);
            case AggregateFn.Avg:
                return source.Average(selector);
            default:
                throw new ArgumentException("Aggregate function expected");
        }
    }

    public static double Aggregate<T>(this IEnumerable<T> source, Func<T, double> selector, AggregateFn fn)
    {
        return fn switch
        {
            AggregateFn.Max => source.Max(selector),
            AggregateFn.Min => source.Min(selector),
            AggregateFn.Sum => source.Sum(selector),
            AggregateFn.Avg => source.Average(selector),
            _ => throw new ArgumentException("Aggregate function expected")
        };
    }

    public static int Aggregate<T>(this IEnumerable<T> source, Func<T, int> selector, AggregateFn fn)
    {
        return fn switch
        {
            AggregateFn.Max => source.Max(selector),
            AggregateFn.Min => source.Min(selector),
            AggregateFn.Sum => source.Sum(selector),
            _ => throw new NotSupportedException($"AggregateFn {fn} not supported with Func<T, int> selector")
        };
    }

    //public static double Avg<T>(this IEnumerable<T> source, Func<T, int> selector)
    //{
    //    return source.Avg(selector);
    //}
}