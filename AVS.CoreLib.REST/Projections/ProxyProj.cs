#nullable enable
using System;
using System.Diagnostics;
using AVS.CoreLib.Abstractions.Responses;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Reflection;
using AVS.CoreLib.REST.Json;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace AVS.CoreLib.REST.Projections;

/// <summary>
/// Provides mechanism to deserialize JSON text and map it to the <typeparamref name="TResult"/> object by means of proxy.
/// </summary>
[DebuggerDisplay("{ToString()}")]
public class ProxyProj<T, TResult> : Proj
{
    public IProxy<T, TResult> Proxy { get; }

    [DebuggerStepThrough]
    public ProxyProj(string jsonText, IProxy<T, TResult> proxy) : base(jsonText)
    {
        Proxy = proxy;
    }

    /// <summary>
    /// Creates <typeparamref name="TResult"/> from <typeparamref name="T"/> data by means of <see cref="Proxy"/>
    /// <code>
    ///     // usecase
    ///     proj => proj.MapData{BitGetResponse{BitGetPosition}}(x => x.Data); // deserialization and result creation are implicit
    ///     
    ///     // MapData could be split on 2 lines:
    ///     proj => {
    ///         var okxResponse = proj.Deserialize{OkxResponse{OkxPosition}}(); //explicit deserialization (better for debugging)
    ///         return proj.CreateResult(okxResponse.Data);
    ///     }
    /// </code>
    /// </summary>
    public TResult CreateResult(T? data)
    {
        if (data != null)
            Proxy.Add(data);
        return Proxy.Create();
    }

    /// <summary>
    /// Creates <typeparamref name="TResult"/> from <typeparamref name="T"/> data by means of <see cref="Proxy"/>
    /// <code>
    ///     // usecase
    ///     proj => proj.MapData{BitGetResponse{BitGetPosition}}(x => x.Data); // deserialization and result creation are implicit
    ///     
    ///     // MapData could be split on 2 lines:
    ///     proj => {
    ///         var okxResponse = proj.Deserialize{OkxResponse{OkxPosition}}(); //explicit deserialization (better for debugging)
    ///         return proj.CreateResult(okxResponse, x => x.Data);
    ///     }
    /// </code>
    /// </summary>
    public TResult CreateResult<TResponse>(TResponse? response, Func<TResponse, T?> selector)
        where TResponse : class, IRESTApiResponse
    {
        if (response == null)
            return Proxy.Create();

        if (response.Success)
        {
            var data = selector(response);

            if (data != null)
            {
                Proxy.Add(data);
            }
        }
        else
        {
            Error = response.GetError();
        }

        return Proxy.Create();
    }

    public TResult CreateResult<TResponse>(TResponse? response, Func<TResponse, T[]?> selector)
        where TResponse : class, IRESTApiResponse
    {
        if (response == null)
            return Proxy.Create();

        if (response.Success)
        {
            var items = selector(response);
            Proxy.AddRange(items);
        }
        else
        {
            Error = response.GetError();
        }

        return Proxy.Create();
    }

    /// <summary>
    /// Deserializes json text into <typeparamref name="T"/>, then produces the result by means of <see cref="Proxy"/>
    /// </summary>
    public TResult? Map(Action<T>? action = null)
    {
        var obj = Deserialize<T>();

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
    public TResult MapArray(Action<T>? action = null)
    {
        var arr = DeserializeArray<T>();
        Proxy.AddRange(arr, action);
        return Proxy.Create();
    }

    /// <summary>
    /// Deserializes json text into <typeparamref name="TType"/>, means of selector picks T[] data and produces the result by means of <see cref="Proxy"/>
    /// </summary>
    /// <typeparam name="TType">The concrete type for deserialization</typeparam>
    /// <param name="selector">Selector to pick the data from deserialized object</param>
    /// <param name="action">The action to process each array item [optional]</param>
    /// <param name="validate">optional validate deserialized TType object</param>
    public TResult MapData<TType>(Func<TType, T> selector, Action<T>? action = null, Action<TType, string>? validate = null)
    {
        var obj = Deserialize<TType>();

        if (obj != null)
        {
            validate?.Invoke(obj, JsonText);
            var data = selector(obj);
            Proxy.Add(data);
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
    public TResult MapDataArray<TType>(Func<TType, T[]> selector, Action<T>? action = null)
    {
        var obj = Deserialize<TType>();

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