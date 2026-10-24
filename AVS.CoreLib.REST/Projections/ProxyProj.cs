#nullable enable
using System;
using System.Diagnostics;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Reflection;
using AVS.CoreLib.REST.Json;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace AVS.CoreLib.REST.Projections;

/// <summary>
/// Provides mechanism to deserialize JSON text and map it to the <typeparamref name="TResult"/> object by means of proxy.
/// </summary>
[DebuggerDisplay("{ToString()}")]
public class ProxyProj<T, TResult>
{
    public string? JsonText { get; }
    public IProxy<T, TResult> Proxy { get; }

    [DebuggerStepThrough]
    public ProxyProj(string? jsonText, IProxy<T, TResult> proxy)
    {
        JsonText = jsonText;
        Proxy = proxy;
    }

    /// <summary>
    /// Deserializes json text into <typeparamref name="T"/>, then produces the result by means of <see cref="Proxy"/>
    /// </summary>
    public TResult? Map(Action<T>? action = null)
    {
        var obj = JsonHelper.Deserialize<T>(JsonText);

        if (obj != null)
        {
            action?.Invoke(obj);
            Proxy.Add(obj);
        }

        return Proxy.Create();
    }

    /// <summary>
    /// Deserializes json text into array of <typeparamref name="T"/>, then produces the result by means of <see cref="Proxy"/>
    /// </summary>
    public TResult? MapArray(Action<T>? action = null)
    {
        var arr = JsonHelper.Deserialize<T[]>(JsonText);

        if (arr != null)
        {
            foreach (var item in arr)
            {
                action?.Invoke(item);
                Proxy.Add(item);
            }
        }

        return Proxy.Create();
    }

    /// <summary>
    /// Deserializes json text into <typeparamref name="TType"/>, means of selector picks T[] data and produces the result by means of <see cref="Proxy"/>
    /// <code>
    ///  var proxy = (IProxy{OKXPosition,UserPositions})builder;
    ///  Response{UserPositions} response = restResponse.ToResponse(proxy, proj => proj.MapArray{OKXResponse{OKXPosition}});
    /// </code>
    /// </summary>
    /// <typeparam name="TType">The concrete type for deserialization</typeparam>
    /// <param name="selector">Selector to pick the data (array) from deserialized object</param>
    /// <param name="action">The action to process each array item [optional]</param>
    public TResult? MapData<TType>(Func<TType, T[]> selector, Action<T>? action = null)
    {
        var obj = JsonHelper.Deserialize<TType>(JsonText);

        if (obj != null)
        {
            var data = selector(obj);
            foreach (var item in data)
            {
                action?.Invoke(item);
                Proxy.Add(item);
            }
        }

        return Proxy.Create();
    }

    /// <summary>
    /// Returns a string representation of the proxy object.
    /// </summary>
    public override string ToString()
    {
        var text = JsonText?.Truncate(maxLength: 110, TruncateOptions.CutOffTheMiddle);
        return $"ProxyProj<{typeof(T).Name},{typeof(TResult).Name}> Proxy={Proxy.GetTypeName()} Content={text}";
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member