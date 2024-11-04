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

    

    public T? Map(Action<T>? process = null)
    {
        var obj = JsonHelper.Deserialize<T>(Content);

        if (process != null && obj != null)
            process(obj);

        return obj;
    }

    public T? Map<TType>(Action<TType>? process = null) where TType : class, T
    {
        var obj = JsonHelper.Deserialize<TType>(Content);

        if (obj == null)
            return default;

        process?.Invoke(obj);
        return obj;
    }

    public IList<T> MapArray<TType>(Action<TType>? process = null) where TType : class, T
    {
        var arr = JsonHelper.Deserialize<TType[]>(Content);

        if (arr == null)
            return Array.Empty<T>();

        var list = new List<T>();

        foreach (var item in arr)
        {
            process?.Invoke(item);
            list.Add(item);
        }

        return list;
    }

    public IDictionary<string, T> MapDictionary<TType>(Action<string, TType>? process = null) where TType : class, T
    {
        var dict = JsonHelper.Deserialize<Dictionary<string, TType>>(Content);

        if (dict == null)
            return new Dictionary<string, T>();

        var res = new Dictionary<string, T>(dict.Count);

        foreach (var kp in dict)
        {
            process?.Invoke(kp.Key, kp.Value);
            res.Add(kp.Key, kp.Value);
        }

        return res;
    }

    public T? Map<TType, TProxy>(Action<TType>? process = null) where TProxy : IProxy<TType, T>, new()
    {
        var proxy = new TProxy();

        var obj = JsonHelper.Deserialize<TType>(Content);

        if (obj != null)
        {
            process?.Invoke(obj);
            proxy.Add(obj);
        }

        return proxy.Create();
    }

    public T? MapArray<TType, TProxy>(Action<TType>? process = null) where TProxy : IProxy<TType, T>, new()
    {
        var proxy = new TProxy();
        var arr = JsonHelper.Deserialize<TType[]>(Content);

        if (arr == null || arr.Length == 0)
            return proxy.Create();

        if (process == null)
            foreach (var item in arr)
            {
                proxy.Add(item);
            }
        else 
            foreach (var item in arr)
            {
                process.Invoke(item);
                proxy.Add(item);
            }

        var res = proxy.Create();
        return res;
    }

    public T? MapDictionary<TType, TProxy>(Action<string, TType>? process = null) where TProxy : IKeyedCollectionProxy<T, TType>, new()
    {
        var proxy = new TProxy();

        var dict = JsonHelper.Deserialize<IDictionary<string, TType>>(Content);

        if (dict != null)
        {
            foreach (var kp in dict)
            {
                process?.Invoke(kp.Key, kp.Value);
                proxy.Add(kp.Key, kp.Value);
            }
        }

        var res = proxy.Create();
        return res;
    }

    /*
    #region MapWithProxy

    public IProxy<T>? Proxy { get; set; }

    public IProxyProj<T> UseProxy<TType>(IProxy<TType, T> proxy)
    {
        Proxy = proxy;
        return this;
    }

    public IKeyedProxyProj<T> UseProxy<TType>(IKeyedCollectionProxy<T, TType> proxy)
    {
        Proxy = proxy;
        return this;
    }

    T? IProxyProj<T>.Map<TType>(Action<TType>? process)
    {
        if (Proxy == null)
            throw new InvalidOperationException("Proxy is not initialized");

        var proxy = (IProxy<TType, T>)Proxy;

        var obj = JsonHelper.Deserialize<TType>(Content);

        if (obj != null)
        {
            process?.Invoke(obj);
            proxy.Add(obj);
        }

        return proxy.Create();
    }
    T? IProxyProj<T>.MapArray<TType>(Action<TType>? process)
    {
        if (Proxy == null)
            throw new InvalidOperationException("Proxy is not initialized");

        var proxy = (IProxy<TType, T>)Proxy;

        var arr = JsonHelper.Deserialize<TType[]>(Content);

        if (arr != null)
        {
            foreach (var item in arr)
            {
                process?.Invoke(item);
                proxy.Add(item);
            }
        }

        var res = proxy.Create();
        return res;
    }
    T? IKeyedProxyProj<T>.MapDictionary<TType>(Action<string, TType>? process)
    {
        if (Proxy == null)
            throw new InvalidOperationException("Proxy is not initialized");

        var proxy = (IKeyedCollectionProxy<T, TType>)Proxy;

        var dict = JsonHelper.Deserialize<IDictionary<string, TType>>(Content);

        if (dict != null)
        {
            foreach (var kp in dict)
            {
                process?.Invoke(kp.Key, kp.Value);
                proxy.Add(kp.Key, kp.Value);
            }
        }

        var res = proxy.Create();
        return res;
    }

    #endregion
    */
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

    public T? Map<TType>(Action<TType>? process = null)
    {
        var proxy = (IProxy<TType, T>)Proxy;

        var obj = JsonHelper.Deserialize<TType>(Content);

        if (obj != null)
        {
            process?.Invoke(obj);
            proxy.Add(obj);
        }

        return proxy.Create();
    }
    public T? MapArray<TType>(Action<TType>? process = null)
    {
        var proxy = (IProxy<TType, T>)Proxy;

        var arr = JsonHelper.Deserialize<TType[]>(Content);

        if (arr != null)
        {
            foreach (var item in arr)
            {
                process?.Invoke(item);
                proxy.Add(item);
            }
        }

        var res = proxy.Create();
        return res;
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

    public T? MapDictionary<TType>(Action<string, TType>? process = null)
    {
        var proxy = (IKeyedCollectionProxy<T, TType>)Proxy;

        var dict = JsonHelper.Deserialize<IDictionary<string, TType>>(Content);

        if (dict != null)
        {
            foreach (var kp in dict)
            {
                process?.Invoke(kp.Key, kp.Value);
                proxy.Add(kp.Key, kp.Value);
            }
        }

        var res = proxy.Create();
        return res;
    }

    public override string ToString()
    {
        return $"KeyedProxyProj<{typeof(T).Name}> Proxy={Proxy.GetTypeName()} Content={Content.Truncate(maxLength: 255, TruncateOptions.CutOffTheMiddle)}";
    }
}