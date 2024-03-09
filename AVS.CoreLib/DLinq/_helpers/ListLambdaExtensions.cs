using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib.DLinq;

internal static class ListLambdaExtensions
{
    internal static Func<IEnumerable<T>, IEnumerable> GetSelectListFn<T>(this LambdaBag bag, PropertyInfo prop, Type? paramType)
    {
        var key = $"{nameof(SelectList)}<{typeof(T).Name},{prop.PropertyType.Name}>(source, {prop.Name}, {paramType?.Name})";
        if (bag.TryGetFunc(key, out Func<IEnumerable<T>, IEnumerable>? fn))
            return fn!;

        // Construct and invoke Select<T, TResult> using expression trees
        var method = typeof(ListLambdaExtensions).ConstructStaticMethod(nameof(SelectList), typeof(T), prop.PropertyType);
        var lambda = LambdaBuilder.InvokeExpr<T>(method, bag, prop, paramType);
        var func = lambda.Compile();
        bag[key] = func;
        return func;
    }

    /// <summary>
    /// Gets delegate that selects a List{Dictionary{string, object}} from source
    /// </summary>
    internal static Func<IEnumerable<T>, IEnumerable> GetSelectListOfDictFn<T>(this LambdaBag bag, PropertyInfo[] props, Type? paramType)
    {
        var propsStr = string.Join(",", props.Select((x => x.Name)));
        var key = $"{nameof(SelectListOfDict)}<{typeof(T).Name}>(source, props:[{propsStr}], {paramType?.Name}))";
        if (bag.TryGetFunc(key, out Func<IEnumerable<T>, IEnumerable>? fn))
            return fn!;

        // Construct and invoke Select<T, TResult> using expression trees
        var method = typeof(ListLambdaExtensions).ConstructStaticMethod(nameof(SelectListOfDict), typeof(T));
        var lambda = LambdaBuilder.InvokeExpr<T>(method, bag, props, paramType);
        var func = lambda.Compile();
        bag[key] = func;
        return func;
    }

    
    /// <summary>
    /// Gets delegate that selects a List{Dictionary{string, TValue}} from source
    /// <see cref="SelectListOfDict{T}"/>
    /// </summary>
    internal static Func<IEnumerable<T>, IEnumerable> GetSelectListOfDictFn<T>(this LambdaBag bag, Type valueType, PropertyInfo[] props, Type? paramType)
    {
        var propsStr = string.Join(",", props.Select((x => x.Name)));
        var key = $"{nameof(SelectListOfTypedDict)}<{typeof(T).Name},{valueType.Name}>(source, props:[{propsStr}], {paramType?.Name}))";
        if (bag.TryGetFunc(key, out Func<IEnumerable<T>, IEnumerable>? fn))
            return fn!;

        // Construct and invoke Select<T, TResult> using expression trees
        var method = typeof(ListLambdaExtensions).ConstructStaticMethod(nameof(SelectListOfTypedDict), typeof(T), valueType);
        var lambda = LambdaBuilder.InvokeExpr<T>(method, bag, props, paramType);
        var func = lambda.Compile();
        bag[key] = func;
        return func;
    }

    private static List<TValue> SelectList<T, TValue>(this IEnumerable<T> source, LambdaBag bag, PropertyInfo prop, Type? paramType)
    {
        var selector = bag.GetSelector<T, TValue>(prop, paramType);
        return source.Select(selector).ToList();
    }

    private static List<Dictionary<string, TValue>> SelectListOfTypedDict<T, TValue>(this IEnumerable<T> source, LambdaBag bag, PropertyInfo[] props, Type? paramType)
    {
        var selector = bag.GetDictSelector<T, TValue>(props, paramType);
        return source.Select(selector).ToList();
    }

    private static List<Dictionary<string, object>> SelectListOfDict<T>(this IEnumerable<T> source, LambdaBag bag, PropertyInfo[] props, Type? paramType)
    {
        var selector = bag.GetDictSelector<T>(props, paramType);
        return source.Select(selector).ToList();
    }
}