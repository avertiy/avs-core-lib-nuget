#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Reflection;
using AVS.CoreLib.REST.Json;

namespace AVS.CoreLib.REST.Projections;

[DebuggerDisplay("{ToString()}")]
public sealed class Proj<T>
{
    public string Content { get; }

    [DebuggerStepThrough]
    public Proj(string json)
    {
        Content = json;
    }

    

    public T? Map(Action<T>? action = null)
    {
        var obj = JsonHelper.Deserialize<T>(Content);

        if (action != null && obj != null)
            action(obj);

        return obj;
    }

    public T? Map<TType>(Action<TType>? action = null) where TType : class, T
    {
        var obj = JsonHelper.Deserialize<TType>(Content);

        if (obj == null)
            return default;

        action?.Invoke(obj);
        return obj;
    }

    public IList<T> MapArray<TType>(Action<TType>? action = null) where TType : class, T
    {
        if (Content.StartsWith('{'))
        {
            var obj = JsonHelper.Deserialize<TType>(Content);
            if (obj == null)
                return Array.Empty<T>();

            action?.Invoke(obj);
            return new List<T>() { obj };
        }

        var arr = JsonHelper.Deserialize<TType[]>(Content);

        if (arr == null)
            return Array.Empty<T>();

        var list = new List<T>();

        foreach (var item in arr)
        {
            action?.Invoke(item);
            list.Add(item);
        }

        return list;
    }

    public IDictionary<string, T> MapDictionary<TType>(Action<string, TType>? action = null) where TType : class, T
    {
        var dict = JsonHelper.Deserialize<Dictionary<string, TType>>(Content);

        if (dict == null)
            return new Dictionary<string, T>();

        var res = new Dictionary<string, T>(dict.Count);

        foreach (var kp in dict)
        {
            action?.Invoke(kp.Key, kp.Value);
            res.Add(kp.Key, kp.Value);
        }

        return res;
    }

    public T? Map<TType, TProxy>(Action<TType>? action = null) where TProxy : IProxy<TType, T>, new()
    {
        var proxy = new TProxy();

        var obj = JsonHelper.Deserialize<TType>(Content);

        if (obj != null)
        {
            action?.Invoke(obj);
            proxy.Add(obj);
        }

        return proxy.Create();
    }

    public T? MapArray<TType, TProxy>(Action<TType>? action = null) where TProxy : IProxy<TType, T>, new()
    {
        var proxy = new TProxy();

        if (Content.StartsWith('{'))
        {
            var obj = JsonHelper.Deserialize<TType>(Content);
            if (obj == null)
                return proxy.Create();

            action?.Invoke(obj);
            proxy.Add(obj);
            return proxy.Create();
        }

        var arr = JsonHelper.Deserialize<TType[]>(Content);

        if (arr == null || arr.Length == 0)
            return proxy.Create();

        if (action == null)
            foreach (var item in arr)
            {
                proxy.Add(item);
            }
        else 
            foreach (var item in arr)
            {
                action.Invoke(item);
                proxy.Add(item);
            }

        var res = proxy.Create();
        return res;
    }

    public T? MapDictionary<TType, TProxy>(Action<string, TType>? action = null) where TProxy : IKeyedCollectionProxy<T, TType>, new()
    {
        var proxy = new TProxy();

        var dict = JsonHelper.Deserialize<IDictionary<string, TType>>(Content);

        if (dict != null)
        {
            foreach (var kp in dict)
            {
                action?.Invoke(kp.Key, kp.Value);
                proxy.Add(kp.Key, kp.Value);
            }
        }

        var res = proxy.Create();
        return res;
    }
    
    public override string ToString()
    {
        return $"Proj<{typeof(T).Name}> Content={Content.Truncate(maxLength: 255, TruncateOptions.CutOffTheMiddle)}";
    }
}

[DebuggerDisplay("{ToString()}")]
public class ProxyProj<T>
{
    public string Content { get; }
    public IProxy<T> Proxy { get; }

    [DebuggerStepThrough]
    public ProxyProj(string json, IProxy<T> proxy)
    {
        Content = json;
        Proxy = proxy;
    }

    public T? Map<TType>(Action<TType>? action = null)
    {
        var proxy = (IProxy<TType, T>)Proxy;

        var obj = JsonHelper.Deserialize<TType>(Content);

        if (obj != null)
        {
            action?.Invoke(obj);
            proxy.Add(obj);
        }

        var builder = (IProxy<T>)proxy;
        // explicit interface call due to builder might implement few IProxy<T> interfaces 
        return builder.Create();
    }
    public T? MapArray<TType>(Action<TType>? action = null)
    {
        var proxy = (IProxy<TType, T>)Proxy;

        var arr = JsonHelper.Deserialize<TType[]>(Content);

        if (arr != null)
        {
            foreach (var item in arr)
            {
                action?.Invoke(item);
                proxy.Add(item);
            }
        }

        var builder = (IProxy<T>)proxy;
        // explicit interface call due to builder might implement few IProxy<T> interfaces 
        return builder.Create();
    }

    public override string ToString()
    {
        return $"ProxyProj<{typeof(T).Name}> Proxy={Proxy.GetTypeName()} Content={Content.Truncate(maxLength: 255, TruncateOptions.CutOffTheMiddle)}";
    }
}

[DebuggerDisplay("{ToString()}")]
public class KeyedProxyProj<T>
{
    public string Content { get; }
    public IProxy<T> Proxy { get; }

    [DebuggerStepThrough]
    public KeyedProxyProj(string json, IProxy<T> proxy)
    {
        Content = json;
        Proxy = proxy;
    }

    public T? MapDictionary<TType>(Action<string, TType>? action = null)
    {
        var proxy = (IKeyedCollectionProxy<T, TType>)Proxy;

        var dict = JsonHelper.Deserialize<IDictionary<string, TType>>(Content);

        if (dict != null)
        {
            foreach (var kp in dict)
            {
                action?.Invoke(kp.Key, kp.Value);
                proxy.Add(kp.Key, kp.Value);
            }
        }

        var builder = (IProxy<T>)proxy;
        return builder.Create();
    }

    public override string ToString()
    {
        return $"KeyedProxyProj<{typeof(T).Name}> Proxy={Proxy.GetTypeName()} Content={Content.Truncate(maxLength: 255, TruncateOptions.CutOffTheMiddle)}";
    }
}