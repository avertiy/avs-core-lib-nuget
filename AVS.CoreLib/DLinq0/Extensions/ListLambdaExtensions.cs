using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AVS.CoreLib.DLinq;
using AVS.CoreLib.DLinq.Extensions;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib.DLinq0.Extensions;

internal static class ListLambdaExtensions
{
    /// <summary>
    /// GetSelectListFn represent an expression source.SelectList(prop, type) => source.Select(x => (type) x.Property).ToList()
    /// </summary>
    internal static Func<IEnumerable<T>, IEnumerable> GetSelectListFn<T>(this LambdaBag bag, PropertyInfo prop, Type? paramType)
    {
        var key = $"source.{nameof(SelectList)}<{typeof(T).Name},{prop.PropertyType.Name}>(bag, {prop.Name}, {paramType?.Name})";
        if (bag.TryGetFunc(key, out Func<IEnumerable<T>, IEnumerable>? fn))
            return fn!;

        // Construct and invoke Select<T, TResult> using expression trees
        var method = typeof(ListLambdaExtensions).ConstructStaticMethod(nameof(SelectList), typeof(T), prop.PropertyType);
        var lambda = LambdaBuilder.InvokeExpr<T>(method, bag, prop, paramType);
        var func = lambda.Compile();
        bag[key] = func;
        return func;
    }

    private static List<TValue> SelectList<T, TValue>(this IEnumerable<T> source, LambdaBag bag, PropertyInfo prop, Type? paramType)
    {
        var selector = bag.GetSelector<T, TValue>(prop, paramType);
        return source.Select(selector).ToList();
    }

    /// <summary>
    /// GetSelectListFn represent an expression source.SelectByIndexAsList(prop, type) source.Select(x =>x.Property[index]).ToList()
    /// </summary>
    internal static Func<IEnumerable<T>, IEnumerable> GetSelectListByIndexFn<T>(this LambdaBag bag, PropertyInfo prop, int index, Type? paramType)
    {
        var methodInfo = prop.PropertyType.GetIndexer();

        if (methodInfo == null)
            throw new ArgumentException($"Indexer [get_Item(int index)] is missing in {prop.PropertyType.Name} type definition.");

        var retType = methodInfo.ReturnType;

        //prop.PropertyType.Name
        var key = $"source.{nameof(SelectByIndex)}<{typeof(T).Name},{retType.Name}>(bag, prop:{prop.Name}, {index}, {paramType?.Name})";
        if (bag.TryGetFunc(key, out Func<IEnumerable<T>, IEnumerable>? fn))
            return fn!;

        // Construct source.SelectByIndexAsList<T, TResult>(bag, prop, index, paramType) 
        var method = typeof(ListLambdaExtensions).ConstructStaticMethod(nameof(SelectByIndex), typeof(T), retType);
        var lambda = LambdaBuilder.InvokeExpr<T>(method, bag, prop, index, paramType);
        var func = lambda.Compile();
        bag[key] = func;
        return func;
    }

    internal static Func<IEnumerable<T>, IEnumerable> GetSelectListByKeyFn<T>(this LambdaBag bag, PropertyInfo prop, string key, Type? paramType)
    {
        var methodInfo = prop.PropertyType.GetKeyIndexer();

        if (methodInfo == null)
            throw new ArgumentException($"Indexer [get_Item(string key)] is missing in {prop.PropertyType.Name} type definition.");

        var retType = methodInfo.ReturnType;

        var fnKey = $"source.{nameof(SelectByKey)}<{typeof(T).Name},{retType.Name}>(bag, {prop.Name}, {key}, {paramType?.Name})";

        if (bag.TryGetFunc(fnKey, out Func<IEnumerable<T>, IEnumerable>? fn))
            return fn!;

        // Construct source.SelectByIndexAsList<T, TResult>(bag, prop, key, paramType) 
        var method = typeof(ListLambdaExtensions).ConstructStaticMethod(nameof(SelectByKey), typeof(T), retType);
        var lambda = LambdaBuilder.InvokeExpr<T>(method, bag, prop, key, paramType);
        var func = lambda.Compile();
        bag[fnKey] = func;
        return func;
    }


    /// <summary>
    /// Gets delegate that selects a List{Dictionary{string, object}} from source
    /// </summary>
    internal static Func<IEnumerable<T>, IEnumerable> GetSelectListOfDictFn<T>(this LambdaBag bag, PropertyInfo[] props, Type? paramType)
    {
        var propsStr = string.Join(",", props.Select(x => x.Name));
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
        var propsStr = string.Join(",", props.Select(x => x.Name));
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



    private static List<TValue> SelectByIndex<T, TValue>(this IEnumerable<T> source, LambdaBag bag, PropertyInfo prop, int index, Type? paramType)
    {
        var selector = bag.GetSelector<T, TValue>(prop, index, paramType);
        return source.Select(selector).ToList();
    }

    private static List<TValue> SelectByKey<T, TValue>(this IEnumerable<T> source, LambdaBag bag, PropertyInfo prop, string key, Type? paramType)
    {
        var selector = bag.GetSelector<T, TValue>(prop, key, paramType);
        //todo add predicate source.Where(x => ... contains key) or select expression add statements need to be wrapped into try...catch
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