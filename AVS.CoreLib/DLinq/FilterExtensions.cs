using System;
using System.Collections;
using System.Collections.Generic;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.DLinq;

public static class FilterExtensions
{
    public static IEnumerable Filter<T>(this IList<T> source, string? filter)
    {
        if (filter == null || source.Count == 0)
            return source;

        FilterHelper.ValidateFilterExpression(filter);

        var typeArg = source[0]!.GetType();
        var props = DynamicSelector.LookupProperties(typeArg, filter);

        if (props.Length == 0)
            return source;

        return source.ToList(props, typeArg);
    }

    public static IEnumerable Filter<T>(this IEnumerable<T> source, string? filter, Type? type = null)
    {
        if (filter == null)
            return source;

        FilterHelper.ValidateFilterExpression(filter);
        var typeArg = type ?? typeof(T);
        var props = DynamicSelector.LookupProperties(typeArg, filter);

        if (props.Length == 0)
            return source;

        return source.ToList(props, typeArg);
    }
}

internal static class FilterHelper
{
    internal static void ValidateFilterExpression(string filter)
    {
        if (filter.StartsWith(".."))
            throw new NotSupportedException($"Filter expression `{filter}` contains a recursive operator `..`, recursive search not supported");

        if (filter.ContainsAny('[', '*', '>', '<', '=', '@'))
            throw new NotSupportedException($"Filter expression `{filter}` contains not supported operators / symbols");

    }
}
