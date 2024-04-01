using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AVS.CoreLib.Extensions.Linq;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib.DLinq.Extensions;

public static class DynamicSelectExtensions
{
    /// <summary>
    /// Dynamic Select
    /// <code>
    ///     source.Select("close") => IEnumerable{decimal};
    ///     source.Select("close,high") => IEnumerable{Dictionary{string,decimal}};
    ///     source.Select("close,time") => IEnumerable{Dictionary{string,object}};
    ///     source.Select("*") => IEnumerable{Dictionary{string,object}};
    /// </code>
    /// </summary>
    public static IEnumerable DynamicSelect<T>(this IEnumerable<T> source, string? selector, Type? type = null, SelectMode mode = SelectMode.Default)
    {
        var typeArg = type ?? typeof(T);
        var props = typeArg.LookupProperties(selector ?? "*");
        
        if (props.Length == 0)
            return source;

        // source.Select(x =>x.Close) => IEnumerable<decimal>
        if (props.Length == 1)
            return source.DynamicSelect(props[0], typeArg, mode);

        // (i) source.Select(x=> {{"close": x.Close}, {"high" : x.High}}) => IEnumerable<Dictionary<string, decimal>>
        // (ii) source.Select(x=> new Dictionary<string, object>(){ {"close": (object)x.Close}, {"time" : (object)x.Time}}).ToList(); => IEnumerable<Dictionary<string, object>>
        return source.DynamicSelect(props, typeArg, mode);
    }

    /// <summary>
    /// Dynamic Select{T,TResult} 
    /// <code>
    ///     source.Select{Bar,decimal}("close") => IEnumerable{decimal};
    /// </code>
    /// </summary>
    public static IEnumerable<TResult> DynamicSelect<T, TResult>(this IEnumerable<T> source, string? selector, Type? type = null)
    {
        var typeArg = type ?? typeof(T);
        var props = typeArg.LookupProperties(selector ?? "*");
        var relevantProps = props.Where(x => x.PropertyType.IsAssignableTo(typeof(TResult))).ToArray();
        var prop = relevantProps.FirstOrDefault();

        if (prop == null)
            return source.Cast<T, TResult>();
        return source.DynamicSelect<T, TResult>(prop, typeArg);
    }
    

    #region DynamicSelect by PropertyInfo(s)
    public static IEnumerable<TResult> DynamicSelect<T, TResult>(this IEnumerable<T> source, PropertyInfo prop, Type? paramType)
    {
        var selector = LambdaBag.Lambdas.GetSelector<T, TResult>(prop, paramType);
        return source.Select(selector);
    }

    public static IEnumerable DynamicSelect<T>(this IEnumerable<T> source, PropertyInfo prop, Type? paramType, SelectMode mode = SelectMode.Default)
    {
        var selectFn = LambdaBag.Lambdas.GetSelectFn<T>(prop, paramType, mode);
        return selectFn.Invoke(source);
    }

    public static IEnumerable DynamicSelect<T>(this IEnumerable<T> source, PropertyInfo[] props, Type? paramType, SelectMode mode = SelectMode.Default)
    {
        var dictSelector = LambdaBag.Lambdas.GetSelectDictFn<T>(props, paramType, mode);
        return dictSelector(source);
    }

    #endregion
}