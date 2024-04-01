using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AVS.CoreLib.Expressions;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib.DLinq.Extensions;

public static class SelectDictExtensions
{
    /// <summary>
    /// Dynamic Select Dictionary{string, object}
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
        var props = typeArg.LookupProperties(selector ?? "*");
        var dictSelector = LambdaBag.Lambdas.GetDictSelector<T>(props, typeArg);
        return source.Select(dictSelector);
    }

    /// <summary>
    /// Dynamic Select Dictionary{string, TResult}
    /// <code>
    ///     source.SelectDict("close") => IEnumerable{Dictionary{string,decimal}};
    ///     source.SelectDict("close,high") => IEnumerable{Dictionary{string,decimal}};
    ///     source.SelectDict("close,time") => IEnumerable{Dictionary{string,decimal}}; //will pick only close property
    ///     source.SelectDict("*") => IEnumerable{Dictionary{string,decimal}};// will pick only props of a decimal type
    /// </code>
    /// </summary>
    public static IEnumerable<Dictionary<string, TResult>> SelectDict<T, TResult>(this IEnumerable<T> source, string? selector, Type? type = null)
    {
        var typeArg = type ?? typeof(T);
        var props = typeArg.LookupProperties(selector ?? "*");
        var relevantProps = props.Where(x => x.PropertyType.IsAssignableTo(typeof(TResult))).ToArray();
        return source.SelectDict<T, TResult>(relevantProps, typeArg);
    }

    public static IEnumerable<Dictionary<string, TResult>> SelectDict<T, TResult>(this IEnumerable<T> source, PropertyInfo[] props, Type? paramType)
    {
        var dictSelector = LambdaBag.Lambdas.GetDictSelector<T, TResult>(props, paramType);
        return source.Select(dictSelector);
    }
}