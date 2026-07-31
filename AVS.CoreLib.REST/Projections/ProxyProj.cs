#nullable enable
using System;
using System.Diagnostics;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Reflection;
using AVS.CoreLib.REST.Json;

namespace AVS.CoreLib.REST.Projections;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
[DebuggerDisplay("{ToString()}")]
public class ProxyProj<T>
{
    public string? JsonText { get; }
    public IProxy<T> Proxy { get; }

    /// <summary>
    /// 
    /// </summary>
    [DebuggerStepThrough]
    public ProxyProj(string? jsonText, IProxy<T> proxy)
    {
        JsonText = jsonText;
        Proxy = proxy;
    }

    /// <summary>
    /// 
    /// </summary>
    public T? Map<TType>(Action<TType>? action = null)
    {
        var proxy = (IProxy<TType, T>)Proxy;

        var obj = JsonHelper.Deserialize<TType>(JsonText);

        if (obj != null)
        {
            action?.Invoke(obj);
            proxy.Add(obj);
        }

        var builder = (IProxy<T>)proxy;
        // explicit interface call due to builder might implement few IProxy<T> interfaces 
        return builder.Create();
    }

    /// <summary>
    /// 
    /// </summary>
    public T? MapArray<TType>(Action<TType>? action = null)
    {
        var proxy = (IProxy<TType, T>)Proxy;

        var arr = JsonHelper.Deserialize<TType[]>(JsonText);

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

    /// <summary>
    /// 
    /// </summary>
    public override string ToString()
    {
        return $"ProxyProj<{typeof(T).Name}> Proxy={Proxy.GetTypeName()} Content={JsonText.Truncate(maxLength: 255, TruncateOptions.CutOffTheMiddle)}";
    }
}
