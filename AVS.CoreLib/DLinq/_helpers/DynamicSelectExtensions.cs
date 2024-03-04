using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AVS.CoreLib.DLinq;

internal static class DynamicSelectExtensions
{
    public static IEnumerable Select<T>(this IEnumerable<T> source, PropertyInfo[] props, Type? paramType)
    {
        //to List{TResult} e.g. bars.Select(x =>x.Close).ToList() => List<decimal>();
        if (props.Length == 1)
            return source.Select(props[0], paramType);

        var uniqueTypes = props.Select(x => x.PropertyType).Distinct().ToArray();

        // source.Select(x=> new Dictionary<string, decimal>(){ {"close": x.Close}, {"high" : x.High}}).ToList(); => IEnumerable<Dictionary<string, decimal>>
        if (uniqueTypes.Length == 1)
            return source.SelectDictionary(uniqueTypes[0], props, paramType);

        // source.Select(x=> new Dictionary<string, object>(){ {"close": (object)x.Close}, {"time" : (object)x.Time}}).ToList(); => IEnumerable<Dictionary<string, object>>
        return source.SelectDictionary(props, paramType);
    }

    private static IEnumerable Select<T>(this IEnumerable<T> source, PropertyInfo prop, Type? paramType)
    {
        var selectFn = LambdaBag.Lambdas.GetSelectListFn<T>(prop, paramType);
        return selectFn.Invoke(source);
    }

    private static IEnumerable SelectDictionary<T>(this IEnumerable<T> source, Type valueType, PropertyInfo[] props, Type? paramType)
    {
        var selectFn = LambdaBag.Lambdas.GetSelectDictFn<T>(valueType, props, paramType);
        return selectFn.Invoke(source);
    }

    private static IEnumerable SelectDictionary<T>(this IEnumerable<T> source, PropertyInfo[] props, Type? paramType)
    {
        var selectFn = LambdaBag.Lambdas.GetSelectDictFn<T>(props, paramType);
        return selectFn.Invoke(source);
    }

    public static IEnumerable<TResult> Select<T,TResult>(this IEnumerable<T> source, PropertyInfo prop, Type? paramType)
    {
        var selector = LambdaBag.Lambdas.GetSelector<T, TResult>(prop, paramType);
        return source.Select(selector);
    }

    public static IEnumerable<Dictionary<string, TResult>> SelectDict<T, TResult>(this IEnumerable<T> source, PropertyInfo[] props, Type? paramType)
    {
        var dictSelector = LambdaBag.Lambdas.GetDictSelector<T, TResult>(props, paramType);
        return source.Select(dictSelector);
    }

    public static IEnumerable<Dictionary<string, object>> SelectDict<T>(this IEnumerable<T> source, PropertyInfo[] props,
        Type? paramType)
    {
        var dictSelector = LambdaBag.Lambdas.GetDictSelector<T>(props, paramType);
        return source.Select(dictSelector);
    }
}
