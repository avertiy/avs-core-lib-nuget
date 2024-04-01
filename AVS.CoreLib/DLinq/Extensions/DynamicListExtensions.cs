using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AVS.CoreLib.Extensions.Reflection;


namespace AVS.CoreLib.DLinq.Extensions;
/*
internal static class DynamicListExtensions
{
    /// <summary>
    /// Dynamic Select list
    /// <code>
    ///     source.Select("close").ToList() => List{decimal};
    ///     source.Select("close,high").ToList() => List{Dictionary{string,decimal}};
    ///     source.Select("close,time").ToList() => List{Dictionary{string,object}};
    ///     source.Select("*").ToList() => List{Dictionary{string,object}};
    /// </code>
    /// </summary>
    public static IEnumerable DynamicSelectList<T>(this IEnumerable<T> source, string? selector, Type? type = null)
    {
        var typeArg = type ?? typeof(T);
        var props = typeArg.LookupProperties(selector ?? "*");

        if (props.Length == 0)
            return source;

        // source.Select(x =>x.Close) => IEnumerable<decimal>
        if (props.Length == 1)
            return source.DynamicSelectList(props[0], typeArg);

        // (i) source.Select(x=> {{"close": x.Close}, {"high" : x.High}}) => IEnumerable<Dictionary<string, decimal>>
        // (ii) source.Select(x=> new Dictionary<string, object>(){ {"close": (object)x.Close}, {"time" : (object)x.Time}}).ToList(); => IEnumerable<Dictionary<string, object>>
        return source.DynamicSelect(props, typeArg);
    }

    #region DynamicSelect by PropertyInfo(s)
    public static IEnumerable<TResult> DynamicSelectList<T, TResult>(this IEnumerable<T> source, PropertyInfo prop, Type? paramType)
    {
        var selector = LambdaBag.Lambdas.GetSelector<T, TResult>(prop, paramType);
        return source.Select(selector).ToList();
    }

    public static IEnumerable DynamicSelect<T>(this IEnumerable<T> source, PropertyInfo prop, Type? paramType)
    {
        var selectFn = LambdaBag.Lambdas.GetSelectFn<T>(prop, paramType);
        return selectFn.Invoke(source);
    }

    public static IEnumerable DynamicSelect<T>(this IEnumerable<T> source, PropertyInfo[] props, Type? paramType)
    {
        var dictSelector = LambdaBag.Lambdas.GetSelectDictFn<T>(props, paramType);
        return dictSelector(source);
    }

    #endregion



    public static IEnumerable ToList<T>(this IEnumerable<T> source, PropertyInfo[] props, Type? paramType)
    {
        var typeArg = type ?? typeof(T);
        var props = typeArg.LookupProperties(selector ?? "*");

        if (props.Length == 0)
            return source;

        // source.Select(x =>x.Close) => IEnumerable<decimal>
        if (props.Length == 1)
            return source.DynamicSelect(props[0], typeArg);

        // (i) source.Select(x=> {{"close": x.Close}, {"high" : x.High}}) => IEnumerable<Dictionary<string, decimal>>
        // (ii) source.Select(x=> new Dictionary<string, object>(){ {"close": (object)x.Close}, {"time" : (object)x.Time}}).ToList(); => IEnumerable<Dictionary<string, object>>
        return source.DynamicSelect(props, typeArg);


        ////to List{TResult} e.g. bars.Select(x =>x.Close).ToList() => List<decimal>();
        //if (props.Length == 1)
        //    return source.ToList(props[0], paramType);

        //var uniqueTypes = props.Select(x => x.PropertyType).Distinct().ToArray();

        //// source.Select(x=> new Dictionary<string, decimal>(){ {"close": x.Close}, {"high" : x.High}}).ToList(); => List<Dictionary<string, decimal>>
        //if (uniqueTypes.Length == 1)
        //    return source.ToListOfDictionary(uniqueTypes[0], props, paramType);

        //// source.Select(x=> new Dictionary<string, object>(){ {"close": (object)x.Close}, {"time" : (object)x.Time}}).ToList(); => List<Dictionary<string, object>>
        //return source.ToListOfDictionary(props, paramType);
    }

    internal static IEnumerable ToList<T>(this IEnumerable<T> source, PropertyInfo prop, Type? paramType)
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
*/