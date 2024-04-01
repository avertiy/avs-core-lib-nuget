using System;
using System.Collections;
using System.Collections.Generic;

namespace AVS.CoreLib.DLinq.Extensions;

public static class FilterExtensions
{
    private static bool IsAny(string filter)
    {
        return filter is "*" or ".*";
    }

    public static IEnumerable Filter<T>(this IList<T> source, string? filter, SelectMode mode = SelectMode.ToList)
    {
        if (source.Count == 0)
            return source;

        if (string.IsNullOrEmpty(filter) || IsAny(filter))
            return source;

        var type = source[0]!.GetType();
        var engine = new DLinqEngine() { Mode = mode };
        return engine.Process(source, filter, type);
    }

    public static IEnumerable Filter<T>(this IEnumerable<T> source, string? filter, Type? type = null, SelectMode mode = SelectMode.ToList)
    {
        if (filter == null || IsAny(filter))
            return source;

        var engine = new DLinqEngine() { Mode = mode };
        return engine.Process(source, filter, type);
    }
}