using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AVS.CoreLib.DLinq;

internal static class LinqHelper
{
    internal static MethodInfo GetSelectMethodInfo(Type type, Type resultType)
    {
        var generic = typeof(LinqHelper).GetMethod(nameof(Select), BindingFlags.Static | BindingFlags.NonPublic)!;
        var method = generic.MakeGenericMethod(type, resultType);
        return method;
    }

    internal static MethodInfo GetToListMethodInfo(Type type)
    {
        var generic = typeof(Enumerable).GetMethod("ToList", BindingFlags.Static | BindingFlags.Public)!;

        //var generic = typeof(LinqHelper).GetMethod(nameof(ToList), BindingFlags.Static | BindingFlags.NonPublic)!;
        var method = generic.MakeGenericMethod(type);
        return method;
    }

    private static IEnumerable<TResult> Select<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, TResult> selector)
    {
        return source.Select(selector);
    }

    private static List<T> ToList<T>(IEnumerable<T> source)
    {
        return source.ToList();
    }
}