#nullable enable
using System;
using System.Diagnostics;
using AVS.CoreLib.REST.Json;

namespace AVS.CoreLib.REST.Projections;
/// <summary>
/// Base class for json projection (mapping) utils <see cref="Proj{TResult}"/> and <see cref="ProxyProj{T, TResult}"/>
/// </summary>
public abstract class Proj
{
    /// <summary>
    /// response content
    /// </summary>
    public string JsonText { get; }
    /// <summary>
    /// contains error when map fails
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Initializes new projection instance
    /// </summary>
    [DebuggerStepThrough]
    public Proj(string jsonText)
    {
        JsonText = jsonText;
    }

    /// <summary>
    /// Straight deserialization parses <see cref="JsonText"/> into <typeparamref name="T"/> value
    /// </summary>
    public T? Deserialize<T>()
    {
        return JsonHelper.Deserialize<T>(JsonText);
    }

    /// <summary>
    /// Deserialization with validation callback
    /// </summary>
    public T? Deserialize<T>(Action<T, string> validate)
    {
        var obj = Deserialize<T>();

        if (obj != null)
            validate.Invoke(obj, JsonText);

        return obj;
    }

    /// <summary>
    /// Use DeserializeArray when REST API returns for many items json array [], for single item json object {}
    /// </summary>
    public T[]? DeserializeArray<T>()
    {
       return JsonHelper.DeserializeArray<T>(JsonText);
    }

    /// <summary>
    /// Use DeserializeArray when REST API returns for many items json array [], for single item json object {}
    /// </summary>
    public T[]? DeserializeArray<T>(Action<T[], string> validate)
    {
        var arr = DeserializeArray<T>();

        if (arr != null)
            validate.Invoke(arr, JsonText);

        return arr;
    }

    internal static bool IsNullOrEmpty(string text)
    {
        return string.IsNullOrEmpty(text) || text.Equals("null", StringComparison.OrdinalIgnoreCase);
    }
}
