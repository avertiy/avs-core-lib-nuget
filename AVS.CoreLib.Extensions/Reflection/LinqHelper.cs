using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AVS.CoreLib.Extensions.Enums;
using AVS.CoreLib.Extensions.Linq;

namespace AVS.CoreLib.Extensions.Reflection;

public static class LinqHelper
{
    public static MethodInfo GetSelectMethodInfo(Type type, Type resultType)
    {
        var generic = typeof(LinqHelper).GetMethod(nameof(Select), BindingFlags.Static | BindingFlags.NonPublic)!;
        var method = generic.MakeGenericMethod(type, resultType);
        return method;
    }

    private static IEnumerable<TResult> Select<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, TResult> selector)
    {
        return source.Select(selector);
    }

    public static MethodInfo GetToListMethodInfo(Type type)
    {
        var generic = typeof(Enumerable).GetMethod("ToList", BindingFlags.Static | BindingFlags.Public)!;
        //var generic = typeof(LinqHelper).GetMethod(nameof(ToList), BindingFlags.Static | BindingFlags.NonPublic)!;
        var method = generic.MakeGenericMethod(type);
        return method;
    }

    private static List<T> ToList<T>(IEnumerable<T> source)
    {
        return source.ToList();
    }

    public static MethodInfo GetOrderByMethodInfo(Type type, Type resultType)
    {
        var generic = typeof(LinqHelper).GetMethod(nameof(OrderBy), BindingFlags.Static | BindingFlags.NonPublic)!;
        var method = generic.MakeGenericMethod(type, resultType);
        return method;
    }

    private static IEnumerable<T> OrderBy<T, TKey>(IEnumerable<T> source, Func<T, TKey> selector, Sort direction)
    {
        try
        {
            return source.OrderBy(selector, direction);
        }
        catch (Exception ex)
        {
            return source;
        }
    }

    public static MethodInfo GetThenByMethodInfo(Type type, Type resultType)
    {
        var generic = typeof(LinqHelper).GetMethod(nameof(ThenBy), BindingFlags.Static | BindingFlags.NonPublic)!;
        var method = generic.MakeGenericMethod(type, resultType);
        return method;
    }

    private static IEnumerable<T> ThenBy<T, TKey>(IOrderedEnumerable<T> source, Func<T, TKey> selector, Sort direction)
    {
        return source.ThenBy(selector, direction);
    }
}