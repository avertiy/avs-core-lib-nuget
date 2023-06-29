using System;
using System.Collections.Generic;

namespace AVS.CoreLib.Extensions.AutoFormatters;

/// <summary>
/// Generic formatter might be used to automate formatting
/// for example print object via reflection retrieve properties and its values
/// <code>
///     //register props
///     formatter.Register&lt;DateTime&gt;("Timestamp", x.ToString("T"))
///     formatter.Register(x=&gt; x.Updated, x.ToString("g"))
///
///     //usage
///     formatter.Format("Timestamp", value);
///     formatter.Format("MyType:Updated", value);
/// </code>
/// </summary>
public class FormatterRegistry : IFormatterRegistry
{
    /// <summary>
    /// key is either PropName or Type:PropName 
    /// </summary>
    protected Dictionary<string, Func<object, string>> Formatters { get; set; } = new();

    /// <inheritdoc />
    public void Register(string key, Func<object, string> format)
    {
        Formatters.Add(key, format);
    }

    /// <inheritdoc />
    public bool ContainsKey(string key)
    {
        return Formatters.ContainsKey(key);
    }

    /// <inheritdoc />
    public Func<object, string> GetFormatter(string key)
    {
        return Formatters[key];
    }

    /// <inheritdoc />
    public Func<object, string> this[string key]
    {
        get => GetFormatter(key);
        set => Formatters[key] = value;
    }

    public void Remove(string key)
    {
        Formatters.Remove(key);
    }

    public void Clear()
    {
        Formatters.Clear();
    }
}

/// <summary>
/// slightly improved version of the registry
/// support pick formatter by keyword & type e.g. Decimal:Price 
/// </summary>
public class XFormatterRegistry : FormatterRegistry
{
    /// <summary>
    /// special words dictionary key=keyword, value=formatter key e.g. Price=>Decimal:Price
    /// </summary>
    private readonly Dictionary<string, string> _dictionary = new();

    public void AddSpecialFormatter<T>(string keyword, Func<T, string> format)
    {
        var type = typeof(T);
        var fullKey = $"{type.Name}:{keyword}";
        Formatters.Add(fullKey, x => format((T)x));
        _dictionary.Add(keyword, fullKey);
    }

    public Func<object, string> PickFormatter(string key, object? value)
    {
        if (Formatters.ContainsKey(key))
            return Formatters[key];

        if(value == null)
            return Formatters[AutoFormatter.DEFAULT_FORMATTER];

        var type = value.GetType();

        if (Match(key, type, out var specialFormatterKey))
        {
            return Formatters[specialFormatterKey];
        }

        return Formatters.ContainsKey(type.Name) ? Formatters[type.Name] : Formatters[AutoFormatter.DEFAULT_FORMATTER];
    }

    private bool Match(string propName, Type type, out string specialFormatterKey)
    {
        foreach (var kp in _dictionary)
        {
            if (propName.Contains(kp.Key) && kp.Value.StartsWith(type.Name + ":"))
            {
                specialFormatterKey = kp.Value;
                return true;
            }
        }

        specialFormatterKey = default!;
        return false;
    }
}

