﻿using System;
using System.Collections.Generic;
using System.Reflection;
using AVS.CoreLib.Collections;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib.Lambdas;

/// <summary>
/// Represent a cache layer to store a compiled lambdas (delegates)
/// </summary>
public class LambdaBag
{
    /// <summary>
    /// contains compiled lambdas that makes dynamics much faster
    /// </summary>
    public static LambdaBag Lambdas { get; set; } = new();

    private readonly FixedList<string> _keys = new(20);

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

    public static bool TryGetFunc<T1, T2, TResult>(this LambdaBag bag, string key, out Func<T1, T2, TResult>? func)
    {
        func = null;

        if (!bag.ContainsKey(key))
            return false;

        if (bag[key] is Func<T1, T2, TResult> fn)
        {
            func = fn;
            return true;
        }

        return false;
    }

    public static Func<T, TResult> GetSelector<T, TResult>(this LambdaBag bag, PropertyInfo prop, Type? paramType)
    {
        var key = $"Func<{typeof(T).Name},{typeof(TResult).Name}>(x => x.{prop.Name}, type: {paramType?.Name})";
        if (bag.TryGetFunc(key, out Func<T, TResult>? fn))
            return fn!;

        var lambda = Lmbd.SelectProp<T, TResult>(prop, paramType);
        var func = lambda.Compile();
        bag[key] = func;
        return func;
    }

    public static Func<T, object> CastTo<T>(this LambdaBag bag, Type targetType)
    {
        var key = $"Func<{typeof(T).GetReadableName()},object>(x => ({targetType.Name})x)";
        if (bag.TryGetFunc(key, out Func<T, object>? fn))
            return fn!;

        var lambda = Lmbd.Cast<T>(targetType);
        var func = lambda.Compile();
        bag[key] = func;
        return func;
    }
}