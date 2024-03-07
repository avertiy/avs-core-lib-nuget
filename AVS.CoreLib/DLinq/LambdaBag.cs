using System;
using System.Linq;
using System.Collections.Generic;
using AVS.CoreLib.Collections;
using System.Reflection;

namespace AVS.CoreLib.DLinq;

/// <summary>
/// Represent a cache layer to store a compiled lambdas (delegates)
/// </summary>
public class LambdaBag
{
    /// <summary>
    /// contains compiled lambdas that makes dynamics much faster
    /// </summary>
    public static LambdaBag Lambdas { get; set; } = new();

    private readonly FixedList<string> _keys = new(10);

    private readonly Dictionary<string, Delegate> _delegates = new();
    public int Capacity { get; set; } = 1000;
    public bool ContainsKey(string key) => _delegates.ContainsKey(key);

    public Delegate this[string key]
    {
        get
        {
            RefreshKey(key);
            return _delegates[key];
        }
        set
        {
            CleanUp();
            RefreshKey(key);
            _delegates[key] = value;
        }
    }

    public void CleanUp()
    {
        if (_delegates.Count < Capacity)
            return;

        foreach (var kp in _delegates)
        {
            if (_keys.Contains(kp.Key))
                continue;

            _delegates.Remove(kp.Key);
        }
    }

    public object? DynamicInvoke(string key, params object?[]? args)
    {
        return _delegates[key].DynamicInvoke(args);
    }

    public T? DynamicInvoke<T>(string key, params object?[]? args)
    {
        return (T?)_delegates[key].DynamicInvoke(args);
    }

    private void RefreshKey(string key)
    {
        _keys.Put(key);
    }
}

public static class LambdaBagExtensions
{
    public static bool TryGetFunc<T, TResult>(this LambdaBag bag, string key, out Func<T, TResult>? func)
    {
        func = null;

        if (!bag.ContainsKey(key))
            return false;

        if (bag[key] is Func<T, TResult> fn)
        {
            func = fn;
            return true;
        }

        return false;
    }

    public static bool TryGetAction<TSource, TValue>(this LambdaBag bag, string key, out Action<TSource, TValue>? action)
    {
        action = null;
        if (!bag.ContainsKey(key))
            return false;

        if (bag[key] is Action<TSource, TValue> fn)
        {
            action = fn;
            return true;
        }

        return false;
    }

    public static Func<T, TResult> GetSelector<T, TResult>(this LambdaBag bag, PropertyInfo prop, Type? paramType)
    {
        var key = $"Func<{typeof(T).Name},{typeof(TResult).Name}>(x => x.{prop.Name})";
        if (bag.TryGetFunc(key, out Func<T, TResult>? fn))
            return fn!;

        var lambda = LambdaBuilder.SelectPropertyExpr<T, TResult>(prop, paramType);
        var func = lambda.Compile();
        bag[key] = func;
        return func;
    }

    public static Func<T, Dictionary<string, TValue>> GetDictSelector<T, TValue>(this LambdaBag bag, PropertyInfo[] props, Type? paramType)
    {
        var propsStr = string.Join(",", props.Select((x => x.Name)));
        var key = $"Func<{typeof(T).Name},Dict<string, {typeof(TValue).Name}>>(props:{propsStr})";
        if (bag.TryGetFunc(key, out Func<T, Dictionary<string, TValue>>? fn))
            return fn!;

        //x => new Dictionary<string,TValue>(props.Length) { {Prop1 = x.Prop1}, {Prop2 = x.Prop2},... }
        var lambda = LambdaBuilder.SelectDictionaryExpr<T, TValue>(props, paramType);
        var func = lambda.Compile();
        bag[key] = func;
        return func;
    }

    public static Func<T, Dictionary<string, object>> GetDictSelector<T>(this LambdaBag bag, PropertyInfo[] props, Type? paramType)
    {
        var propsStr = string.Join(",", props.Select((x => x.Name)));
        var key = $"Func<{typeof(T).Name},Dict<string, object>>(props:{propsStr})";
        if (bag.TryGetFunc(key, out Func<T, Dictionary<string, object>>? fn))
            return fn!;

        //x => new Dictionary<string,object>(props.Length) { {Prop1 = (object)x.Prop1}, {Prop2 = (object)x.Prop2},... }
        var lambda = LambdaBuilder.SelectDictionaryExpr<T>(props, paramType);
        var func = lambda.Compile();
        bag[key] = func;
        return func;
    }

    public static Func<object, object> Cast(this LambdaBag bag, Type targetType)
    {
        var key = $"Func<object,object>(x => ({targetType.Name})x)";
        if (bag.TryGetFunc(key, out Func<object, object>? fn))
            return fn!;

        var lambda = LambdaBuilder.CastExpr(targetType);
        var func = lambda.Compile();
        bag[key] = func;
        return func;
    }
}