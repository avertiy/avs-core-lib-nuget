#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.REST.Json;

namespace AVS.CoreLib.REST.Projections;

/// <summary>
/// Proj implements Projection functionallity without a dependency on Newtonsoft
/// </summary>
/// <typeparam name="TResult"></typeparam>
[DebuggerDisplay("{ToString()}")]
public sealed class Proj<TResult>
{
    /// <summary>
    /// response content
    /// </summary>
    public string? JsonText { get; }

    /// <summary>
    /// Initializes new projection instance
    /// </summary>
    [DebuggerStepThrough]
    public Proj(string? jsonText)
    {
        JsonText = jsonText;
    }

    /// <summary>
    /// Straight deserialization without any mapping or proxying tricks
    /// </summary>
    public T? Deserialize<T>()
    {
        return JsonHelper.Deserialize<T>(JsonText);
    }
    
    #region Map
    /// <summary>
    /// Deserializes json text into <typeparamref name="TResult"/> value
    /// (straight deserialization)
    /// <code>
    ///  var projection = new Proj{BinancePosition}(json);
    ///  var position = projection.Map();
    /// </code>
    /// </summary>
    public TResult? Map(Action<TResult>? action = null)
    {
        var obj = JsonHelper.Deserialize<TResult>(JsonText);

        if (action != null && obj != null)
            action(obj);

        return obj;
    }

    /// <summary>
    /// Deserializes json text into <typeparamref name="T"/> in case <typeparamref name="TResult"/> is an abtraction/interface
    /// <code>
    ///  var projection = new Proj{ITrade}(json);
    ///  var trade = projection.Map{BinanceTrade}();
    /// </code>
    /// </summary>
    public TResult? Map<T>(Action<T>? action = null) where T : class, TResult
    {
        var obj = JsonHelper.Deserialize<T>(JsonText);

        if (obj == null)
            return default;

        action?.Invoke(obj);
        return obj;
    }

    /// <summary>
    /// Deserilize json text into <typeparamref name="T"/>, <paramref name="convert"/> converts <typeparamref name="T"/> object into <typeparamref name="TResult"/>
    /// <code>
    /// Response{IFuturesOrder} response = restResponse.ToResponse{IFuturesOrder}(proj => 
    ///     proj.Map{OKXOrder}(x => new FuturesOrder(){...}));
    /// </code>
    /// </summary>
    public TResult? Map<T>(Func<T, TResult> convert)
    {
        var obj = JsonHelper.Deserialize<T>(JsonText);

        if (obj == null)
            return default;

        var result = convert(obj);
        return result;
    }

    /// <summary>
    /// Map with proxy. 
    /// Deserilizes json text into <typeparamref name="T"/> then calls proxy to add the deserialized object and produce the result
    /// </summary>
    public TResult? Map<T, TProxy>(Action<T>? action = null) where TProxy : IProxy<T, TResult>, new()
    {
        var proxy = new TProxy();

        var obj = JsonHelper.Deserialize<T>(JsonText);

        if (obj != null)
        {
            action?.Invoke(obj);
            proxy.Add(obj);
        }

        return proxy.Create();
    }

    /// <summary>
    /// Deserilize json text into <typeparamref name="T"/> then mapper will map it to <typeparamref name="TResult"/>
    /// </summary>
    public TResult? MapWithMapper<T>(IMapper<T, TResult> mapper)
    {
        var obj = JsonHelper.Deserialize<T>(JsonText);

        if (obj == null)
            return default;

        var result = mapper.Map(obj);
        return result;
    }
    /// <summary>
    /// Deserilize json text into <typeparamref name="T"/> then map with mapper to <typeparamref name="TResult"/>
    /// </summary>
    [Obsolete("Use proj.Map<T>(Func<T,TResult>)")]
    public TResult? MapWithMapper<T>(Func<T, TResult> convert)
    {
        var obj = JsonHelper.Deserialize<T>(JsonText);

        if (obj == null)
            return default;

        var result = convert(obj);
        return result;
    }

    #endregion

    #region MapData

    /// <summary>
    /// Map data using proxy.
    /// Deserializes json into <typeparamref name="TResponse"/>, than builds <typeparamref name="TResult"/> by means of proxy
    /// <code>
    /// IProxy{OKXPosition,UserPositions} builder = new PositionsBuilder&lt;OKXPosition&gt;() {...};
    /// Response{UserPositions} response = restResponse.ToResponse&lt;UserPositions&gt;(
    ///     proj =&gt; proj.MapData(builder));
    /// </code>
    /// </summary>
    [Obsolete("Use ProxyProj{T,TResult}.MapData(..) instead")]
    public TResult? MapData<TResponse, T>(Func<TResponse, T[]> selector, IProxy<T, TResult> proxy, Action<T>? action = null)
    {
        var obj = JsonHelper.Deserialize<TResponse>(JsonText);

        if (obj != null)
        {
            var data = selector(obj);
            foreach (var item in data)
            {
                action?.Invoke(item);
                proxy.Add(item);
            }
        }

        return proxy.Create();
    } 
    #endregion

    #region MapArray

    /// <summary>
    /// Deserializes json array into <typeparamref name="T[]"/> than fills in the list of <see cref="IList{TResult}"/>
    /// In case json text is an object { ... } it projects it as <see cref="IList{TResult}"/> with only one item
    /// </summary>
    public IList<TResult> MapArray<T>(Action<T>? action = null) where T : class, TResult
    {
        if (JsonText == null)
            return Array.Empty<TResult>();

        if (JsonText.StartsWith('{'))
        {
            var obj = JsonHelper.Deserialize<T>(JsonText);
            if (obj == null)
                return Array.Empty<TResult>();

            action?.Invoke(obj);
            return new List<TResult>() { obj };
        }

        var arr = JsonHelper.Deserialize<T[]>(JsonText);

        if (arr == null)
            return Array.Empty<TResult>();

        var list = new List<TResult>();

        foreach (var item in arr)
        {
            action?.Invoke(item);
            list.Add(item);
        }

        return list;
    }

    /// <summary>
    /// Deserializes json array into <typeparamref name="T[]"/>, than builds <typeparamref name="TResult"/> by means of a proxy builder
    /// </summary>
    public TResult? MapArray<T>(IProxy<T, TResult> proxy)
    {
        if (JsonText == null)
            return proxy.Create();

        if (JsonText.StartsWith('{'))
        {
            var obj = JsonHelper.Deserialize<T>(JsonText);

            if (obj == null)
                return proxy.Create();

            proxy.Add(obj);

            return proxy.Create();
        }

        var arr = JsonHelper.Deserialize<T[]>(JsonText);

        if (arr == null || arr.Length == 0)
            return proxy.Create();

        foreach (var item in arr)
        {
            proxy.Add(item);
        }

        var res = proxy.Create();
        return res;
    }

    /// <summary>
    /// Map json array of objects 
    /// <code>
    ///  // use cases
    ///  // map json array of ByBitPositions into proxy object UserPositions by means of a proxy class (PositionsBuilder)
    ///  var proj = new Proj{UserPositions}(json);
    ///  proj.MapArray{ByBitPosition, PositionsBuilder}(x => { x.Symbol = NormalizeSymbol(x.Symbol); });
    /// </code>
    /// </summary>
    public TResult? MapArray<T, TProxy>(Action<T>? action = null) where TProxy : IProxy<T, TResult>, new()
    {
        var proxy = new TProxy();

        if (JsonText == null)
            return proxy.Create();

        if (JsonText.StartsWith('{'))
        {
            var obj = JsonHelper.Deserialize<T>(JsonText);

            if (obj == null)
                return proxy.Create();

            action?.Invoke(obj);
            proxy.Add(obj);

            return proxy.Create();
        }

        var arr = JsonHelper.Deserialize<T[]>(JsonText);

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

    #endregion

    #region MapDictionary
    /// <summary>
    /// 
    /// </summary>
    public IDictionary<string, TResult> MapDictionary<TType>(Action<string, TType>? action = null) where TType : class, TResult
    {
        var dict = JsonHelper.Deserialize<Dictionary<string, TType>>(JsonText);

        if (dict == null)
            return new Dictionary<string, TResult>();

        var res = new Dictionary<string, TResult>(dict.Count);

        foreach (var kp in dict)
        {
            action?.Invoke(kp.Key, kp.Value);
            res.Add(kp.Key, kp.Value);
        }

        return res;
    }

    /// <summary>
    /// 
    /// </summary>
    public TResult? MapDictionary<TType, TProxy>(Action<string, TType>? action = null) where TProxy : IKeyedCollectionProxy<TResult, TType>, new()
    {
        var proxy = new TProxy();

        var dict = JsonHelper.Deserialize<IDictionary<string, TType>>(JsonText);

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
    #endregion

    /// <summary>
    /// 
    /// </summary>
    public override string ToString()
    {
        var jsonText = JsonText?.Truncate(maxLength: 255, TruncateOptions.CutOffTheMiddle) ?? string.Empty;
        return $"Proj<{typeof(TResult).Name}> Content={jsonText}";
    }
}
