using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib.DLinq;

internal static class SelectLambdaExtensions
{
    internal static Func<IEnumerable<T>, IEnumerable> GetSelectFn<T>(this LambdaBag bag, PropertyInfo prop, Type? paramType)
    {
        var key = $"{nameof(Select)}<{typeof(T).Name},{prop.PropertyType.Name}>(source, {prop.Name}, {paramType?.Name}))";
        if (bag.TryGetFunc(key, out Func<IEnumerable<T>, IEnumerable>? fn))
            return fn!;

        // Construct and invoke Select<T, TResult> using expression trees
        var method = typeof(SelectLambdaExtensions).ConstructStaticMethod(nameof(Select),typeof(T), prop.PropertyType);
        var lambda = LambdaBuilder.InvokeExpr<T>(method, bag, prop, paramType);
        var func = lambda.Compile();
        bag[key] = func;
        return func;
    }


    private static IEnumerable<TValue> Select<T, TValue>(this IEnumerable<T> source, LambdaBag bag, PropertyInfo prop, Type? paramType)
    {
        var selector = bag.GetSelector<T, TValue>(prop, paramType);
        return source.Select(selector);
    }

    /// <summary>
    /// Gets delegate that selects a IEnumerable{Dictionary{string, object}} from source
    /// </summary>
    internal static Func<IEnumerable<T>, IEnumerable> GetSelectDictFn<T>(this LambdaBag bag, PropertyInfo[] props, Type? paramType)
    {
        var propsStr = string.Join(",", props.Select((x => x.Name)));
        var key = $"{nameof(SelectDict)}<{typeof(T).Name}>(source, props: [{propsStr}], {paramType?.Name})";
        if (bag.TryGetFunc(key, out Func<IEnumerable<T>, IEnumerable>? fn))
            return fn!;

        // Construct and invoke Select<T, TResult> using expression trees
        var method = typeof(SelectLambdaExtensions).ConstructStaticMethod(nameof(SelectDict), typeof(T));
        var lambda = LambdaBuilder.InvokeExpr<T>(method, bag, props, paramType);
        var func = lambda.Compile();
        bag[key] = func;
        return func;
    }

    private static IEnumerable<Dictionary<string, object>> SelectDict<T>(this IEnumerable<T> source, LambdaBag bag, PropertyInfo[] props, Type? paramType)
    {
        var selector = bag.GetDictSelector<T>(props, paramType);
        return source.Select(selector);
    }


    /// <summary>
    /// Gets delegate that selects a IEnumerable{Dictionary{string, TValue}} from source
    /// <see cref="SelectTypedDict{T,TValue}"/>
    /// </summary>
    internal static Func<IEnumerable<T>, IEnumerable> GetSelectDictFn<T>(this LambdaBag bag, Type valueType, PropertyInfo[] props, Type? paramType)
    {
        var propsStr = string.Join(",", props.Select((x => x.Name)));
        var key = $"{nameof(SelectTypedDict)}<{typeof(T).Name},{valueType.Name}>(source, props: [{propsStr}], {paramType?.Name})";
        if (bag.TryGetFunc(key, out Func<IEnumerable<T>, IEnumerable>? fn))
            return fn!;

        // Construct and invoke Select<T, TResult> using expression trees
        var method = typeof(SelectLambdaExtensions).ConstructStaticMethod(nameof(SelectTypedDict), typeof(T), valueType);
        var lambda = LambdaBuilder.InvokeExpr<T>(method, bag, props, paramType);
        var func = lambda.Compile();
        bag[key] = func;
        return func;
    }

    private static IEnumerable<Dictionary<string, TValue>> SelectTypedDict<T, TValue>(this IEnumerable<T> source, LambdaBag bag, PropertyInfo[] props, Type? paramType)
    {
        var selector = bag.GetDictSelector<T, TValue>(props, paramType);
        return source.Select(selector);
    }
}