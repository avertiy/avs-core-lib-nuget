using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib.DLinq.Extensions;

internal static class ListOfDictLambdaBagExtensions
{
    /// <summary>
    /// Gets Func delegate to select List of typed dictionary
    /// <see cref="SelectListOfObjectDict{T}"/>
    /// <code>
    ///  List{Dictionary{string, object}} list = source.Select(selector).ToList()
    /// </code>
    /// </summary>
    internal static Func<IEnumerable<T>, IEnumerable> GetSelectListOfObjectDictFn<T>(this LambdaBag bag, ListDictLambdaSpec<T> spec)
    {
        var key = spec.GetCacheKey(nameof(SelectListOfObjectDict));
        if (bag.TryGetFunc(key, out Func<IEnumerable<T>, IEnumerable>? fn))
            return fn!;

        // Construct and invoke Select<T, TResult> using expression trees
        var method = typeof(ListOfDictLambdaBagExtensions).ConstructStaticMethod(nameof(SelectListOfObjectDict), typeof(T));
        var lambda = InvokeExpr.GetExpr(method, bag, spec);
        var func = lambda.Compile();
        bag[key] = func;
        return func;
    }

    /// <summary>
    /// Gets Func delegate to select List of typed dictionary
    /// <see cref="SelectListOfTypedDict{T,TValue}"/>
    /// <code>
    ///  List{Dictionary{string, TValue}} list = source.Select(selector).ToList()
    /// </code>
    /// </summary>
    internal static Func<IEnumerable<T>, IEnumerable> GetSelectListOfTypedDictFn<T>(this LambdaBag bag, ListDictLambdaSpec<T> spec)
    {
        var key = spec.GetCacheKey(nameof(SelectListOfTypedDict));
        if (bag.TryGetFunc(key, out Func<IEnumerable<T>, IEnumerable>? fn))
            return fn!;

        // Construct and invoke Select<T, TResult> using expression trees
        var method = typeof(ListOfDictLambdaBagExtensions).ConstructStaticMethod(nameof(SelectListOfTypedDict), typeof(T), spec.ValueType);
        var lambda = InvokeExpr.GetExpr(method, bag, spec);
        var func = lambda.Compile();
        bag[key] = func;
        return func;
    }

    private static List<Dictionary<string, TValue>> SelectListOfTypedDict<T, TValue>(this IEnumerable<T> source, LambdaBag bag, ListDictLambdaSpec<T> spec)
    {
        var selector = bag.GetDictSelector<T, TValue>(spec);
        return source.Select(selector).ToList();
    }

    private static List<Dictionary<string, object>> SelectListOfObjectDict<T>(this IEnumerable<T> source, LambdaBag bag, ListDictLambdaSpec<T> spec)
    {
        var selector = bag.GetDictSelector<T>(spec);
        return source.Select(selector).ToList();
    }
}