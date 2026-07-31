#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Reflection;
using AVS.CoreLib.REST.Json;

namespace AVS.CoreLib.REST.Projections;

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