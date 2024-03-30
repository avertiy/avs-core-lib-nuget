using System;
using System.Collections;
using System.Collections.Generic;
using AVS.CoreLib.DLinq;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib.DLinq0;

public static class FilterExtensions0
{
    private static bool IsAny(string filter)
    {
        return filter is "*" or ".*";
    }

    public static IEnumerable Filter0<T>(this IList<T> source, string? filter)
    {
        if (source.Count == 0)
            return source;

        if (string.IsNullOrEmpty(filter) || IsAny(filter))
            return source;

        var typeArg = source[0]!.GetType();

        if (ExpressionEngine.IsSimpleExpression(filter))
        {
            var props = typeArg.LookupProperties(filter);
            return props.Length == 0 ? source : source.ToList(props, typeArg);
        }

        var engine = new ExpressionEngine();
        return engine.Process(source, filter, typeArg);
    }

    public static IEnumerable Filter0<T>(this IEnumerable<T> source, string? filter, Type? type = null)
    {
        if (filter == null || IsAny(filter))
            return source;

        if (ExpressionEngine.IsSimpleExpression(filter))
        {
            var typeArg = type ?? typeof(T);
            var props = typeArg.LookupProperties(filter);

            return props.Length == 0 ? source : source.ToList(props, typeArg);
        }

        var engine = new ExpressionEngine();
        return engine.Process(source, filter, type);
    }
}