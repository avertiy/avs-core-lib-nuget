using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AVS.CoreLib.DLinq;

internal static class DynamicListExtensions
{
    public static IEnumerable ToList<T>(this IEnumerable<T> source, PropertyInfo[] props, Type? paramType)
    {
        //to List{TResult} e.g. bars.Select(x =>x.Close).ToList() => List<decimal>();
        if (props.Length == 1)
            return source.ToList(props[0], paramType);

        var uniqueTypes = props.Select(x => x.PropertyType).Distinct().ToArray();

        // source.Select(x=> new Dictionary<string, decimal>(){ {"close": x.Close}, {"high" : x.High}}).ToList(); => List<Dictionary<string, decimal>>
        if (uniqueTypes.Length == 1)
            return source.ToListOfDictionary(uniqueTypes[0], props, paramType);

        // source.Select(x=> new Dictionary<string, object>(){ {"close": (object)x.Close}, {"time" : (object)x.Time}}).ToList(); => List<Dictionary<string, object>>
        return source.ToListOfDictionary(props, paramType);
    }

    private static IEnumerable ToList<T>(this IEnumerable<T> source, PropertyInfo prop, Type? paramType)
    {
        var selectFn = LambdaBag.Lambdas.GetSelectListFn<T>(prop, paramType);
        return selectFn.Invoke(source);
    }

    private static IEnumerable ToListOfDictionary<T>(this IEnumerable<T> source, Type valueType, PropertyInfo[] props, Type? paramType)
    {
        var selectFn = LambdaBag.Lambdas.GetSelectListOfDictFn<T>(valueType, props, paramType);
        return selectFn.Invoke(source);
    }

    private static IEnumerable ToListOfDictionary<T>(this IEnumerable<T> source, PropertyInfo[] props, Type? paramType)
    {
        var selectFn = LambdaBag.Lambdas.GetSelectListOfDictFn<T>(props, paramType);
        return selectFn.Invoke(source);
    }
}