using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Extensions.Linq;

namespace AVS.CoreLib.DLinq;

public static class DynamicExtensions
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
    public static IEnumerable Select<T>(this IEnumerable<T> source, string? selector, Type? type = null)
    {
        var typeArg = type ?? typeof(T);
        var props = DynamicSelector.LookupProperties(typeArg, selector ?? "*");
        return source.Select(props, typeArg);
    }

    /// <summary>
    /// Dynamic Select{T,TResult} 
    /// <code>
    ///     source.Select{Bar,decimal}("close") => IEnumerable{decimal};
    /// </code>
    /// </summary>
    public static IEnumerable<TResult> Select<T, TResult>(this IEnumerable<T> source, string? selector, Type? type = null)
    {
        var typeArg = type ?? typeof(T);
        var props = DynamicSelector.LookupProperties(typeArg, selector ?? "*");
        var relevantProps = props.Where(x => x.PropertyType.IsAssignableTo(typeof(TResult))).ToArray();
        return relevantProps.Length == 0 ? source.Cast<T, TResult>() : source.Select<T, TResult>(relevantProps[0], typeArg);
    }

    /// <summary>
    /// Dynamic Select
    /// <code>
    ///     // properties will be boxed to object
    ///     source.SelectDict("close") => IEnumerable{Dictionary{string,object}}; 
    ///     source.SelectDict("close,high") => IEnumerable{Dictionary{string,object}};
    ///     source.SelectDict("close,time") => IEnumerable{Dictionary{string,object}};
    ///     source.SelectDict("*") => IEnumerable{Dictionary{string,object}};
    /// </code>
    /// </summary>
    public static IEnumerable<Dictionary<string, object>> SelectDict<T>(this IEnumerable<T> source, string? selector, Type? type = null)
    {
        var typeArg = type ?? typeof(T);
        var props = DynamicSelector.LookupProperties(typeArg, selector ?? "*");
        return source.SelectDict(props, typeArg);
    }

    /// <summary>
    /// Dynamic Select
    /// <code>
    ///     source.SelectDict("close") => IEnumerable{Dictionary{string,decimal}};
    ///     source.SelectDict("close,high") => IEnumerable{Dictionary{string,decimal}};
    ///     source.SelectDict("close,time") => IEnumerable{Dictionary{string,decimal}}; //will pick only close property
    ///     source.SelectDict("*") => IEnumerable{Dictionary{string,decimal}};// will pick only props of a decimal type
    /// </code>
    /// </summary>
    public static IEnumerable<Dictionary<string, TResult>> SelectDict<T, TResult>(this IEnumerable<T> source,
        string? selector, Type? type = null)
    {
        var typeArg = type ?? typeof(T);
        var props = DynamicSelector.LookupProperties(typeArg, selector ?? "*");
        var relevantProps = props.Where(x => x.PropertyType.IsAssignableTo(typeof(TResult))).ToArray();
        return source.SelectDict<T, TResult>(relevantProps, typeArg);
    }
}