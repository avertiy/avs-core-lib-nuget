using System;
using System.Collections.Generic;

namespace AVS.CoreLib.Extensions.Linq;

public static class EnumerableAggregateExtensions
{
    public static (decimal min, decimal max) MinMax<T>(this IEnumerable<T> source, Func<T, decimal> selector)
    {
        decimal min = 0, max = 0;
        foreach (var item in source)
        {
            var value = selector(item);

            if (min == 0 || value < min)
                min = value;

            if (max == 0 || value > max)
                max = value;
        }

        return (min, max);
    }

    public static (TResult? min, TResult? max) MinMax<T, TResult>(this IEnumerable<T> source, Func<T, TResult?> selector)
    {
        var initialized = false;
        TResult? minValue = default;
        TResult? maxValue = default;
        var comparer = Comparer<TResult>.Default;
        foreach (var item in source)
        {
            var currentValue = selector(item);

            if (currentValue == null)
                continue;

            if (!initialized)
            {
                initialized = true;
                minValue = currentValue;
                maxValue = currentValue;
            }

            if (comparer.Compare(currentValue, minValue) <= 0)
                minValue = currentValue;

            if (comparer.Compare(currentValue, maxValue) >= 0)
                maxValue = currentValue;
        }

        return (min: minValue, max: maxValue);
    }
}