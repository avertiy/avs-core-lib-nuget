using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using AVS.CoreLib.Abstractions.Collections;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Stringify;

namespace AVS.CoreLib.Collections;

/// <summary>
/// Represents a mutable collection of key-value pairs, where key is a string and value is an object.
/// </summary>
[DebuggerDisplay("Bag<{GetTypeName()}> #{Count} {GetKeysPreview()}")]
[JsonConverter(typeof(BagJsonConverterFactory))]
public record Bag<T> : IBag<T>
{
    private readonly Dictionary<string, T> _dict;
    [JsonIgnore]
    public int Count => _dict.Count;

    public Bag(int capacity = 5)
    {
        _dict = new(capacity);
    }

    public IDictionary<string, T> GetDictionary()
    {
        return _dict;
    }

    public string[] GetAllKeys()
    {
        return _dict.Keys.ToArray();
    }


    public void EnsureCapacity(int capacity)
    {
        _dict.EnsureCapacity(capacity);
    }

    public bool ContainsKey(string key)
    {
        return _dict.ContainsKey(key);
    }

    public T? GetOrDefault(string key)
    {
        return _dict.ContainsKey(key) ? (T)_dict[key] : default;
    }

    public T Get(string key)
    {
        return _dict[key];
    }

    public bool TryGetValue(string key, out T? value)
    {
        if (_dict.TryGetValue(key, out var obj))
        {
            value = obj;
            return true;
        }

        value = default;
        return false;
    }

    public void Set(string key, T value)
    {
        _dict[key] = value;
    }

    public void CopyTo(IBag<T> bag)
    {
        foreach (var kp in _dict)
        {
            bag.Set(kp.Key, kp.Value);
        }
    }

    public override string ToString()
    {
        return _dict.Stringify();
    }

    public string GetTypeName()
    {
        return typeof(T).Name;
    }

    public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
    {
        return _dict.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    protected string GetKeysPreview()
    {
        return string.Join(',', _dict.Keys.Take(10)).Truncate(30, TruncateOptions.CutOffTheMiddle);
    }
}

public class BagJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType &&
               typeToConvert.GetGenericTypeDefinition() == typeof(Bag<>);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var valueType = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(BagJsonConverter<>).MakeGenericType(valueType);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}


public class BagJsonConverter<T> : JsonConverter<Bag<T>>
{
    public override Bag<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dict = JsonSerializer.Deserialize<Dictionary<string, T>>(ref reader, options);
        var bag = new Bag<T>(dict?.Count ?? 5);

        if (dict != null)
        {
            foreach (var kv in dict)
                bag.Set(kv.Key, kv.Value);
        }

        return bag;
    }

    public override void Write(Utf8JsonWriter writer, Bag<T> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.GetDictionary(), options);
    }
}

