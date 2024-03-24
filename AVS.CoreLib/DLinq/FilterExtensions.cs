using System;
using System.Collections;
using System.Collections.Generic;

namespace AVS.CoreLib.DLinq;

public static class FilterExtensions
{
    private static bool IsAny(string filter)
    {
        return filter is "*" or ".*";
    }

    public static IEnumerable Filter<T>(this IList<T> source, string? filter)
    {
        if (string.IsNullOrEmpty(filter) || source.Count == 0 || IsAny(filter))
            return source;

        var typeArg = source[0]!.GetType();

        if (ExpressionEngine.IsSimple(filter))
        {
            var props = DynamicSelector.LookupProperties(filter, typeArg);
            return props.Length == 0 ? source : source.ToList(props, typeArg);
        }

        var engine = new ExpressionEngine();
        return engine.Process(source, filter, typeArg);
    }

    public static IEnumerable Filter<T>(this IEnumerable<T> source, string? filter, Type? type = null)
    {
        if (filter == null || IsAny(filter))
            return source;

        if (ExpressionEngine.IsSimple(filter))
        {
            var typeArg = type ?? typeof(T);
            var props = DynamicSelector.LookupProperties(filter, typeArg);

            return props.Length == 0 ? source : source.ToList(props, typeArg);
        }

        var engine = new ExpressionEngine();
        return engine.Process(source, filter, type);
    }
}