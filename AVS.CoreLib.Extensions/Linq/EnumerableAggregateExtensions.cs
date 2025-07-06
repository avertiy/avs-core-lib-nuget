using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Guards;

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

    public static int FindIndexMax<T>(this IEnumerable<T> source) where T : struct, IComparable<T>
    {
        Guard.Against.Null(source, name: nameof(source));

        var ind = -1;
        var max = default(T);
        var i = 0;
        var foundAny = false;

        foreach (var item in source)
        {
            if (!foundAny || item.CompareTo(max) > 0)
            {
                max = item;
                ind = i;
                foundAny = true;
            }

            i++;
        }

        return ind;
    }

    public static int FindIndexMin<T>(this IEnumerable<T> source) where T : struct, IComparable<T>
    {
        Guard.Against.Null(source, name: nameof(source));

        var ind = -1;
        var min = default(T);
        var i = 0;
        var foundAny = false;

        foreach (var item in source)
        {
            if (!foundAny || item.CompareTo(min) < 0)
            {
                min = item;
                ind = i;
                foundAny = true;
            }

            i++;
        }

        return ind;
    }

    /// <summary>
    /// Finds index of the max element
    /// </summary>
    public static int FindIndexMax<T>(this IEnumerable<T> source, Func<T, decimal> selector)
    {
        var ind = -1;
        decimal max = 0;
        var i = 0;
        foreach (var item in source)
        {
            var value = selector(item);

            if (max == 0 || value > max)
            {
                max = value;
                ind = i;
            }

            i++;
        }

        return ind;
    }
}