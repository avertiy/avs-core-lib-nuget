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
[DebuggerDisplay("Bag #{Count} {GetKeysPreview()}")]
[JsonConverter(typeof(BagJsonConverter))]
public record Bag : IBag
{
    private readonly Dictionary<string, object> _dict;
    [JsonIgnore]
    public int Count => _dict.Count;

    public Bag()
    {
        _dict = new();
    }

    public Bag(int capacity)
    {
        _dict = new(capacity);
    }

    public IDictionary<string, object> GetDictionary()
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

    public TValue? GetOrDefault<TValue>(string key)
    {
        return _dict.ContainsKey(key) ? (TValue)_dict[key] : default;
    }

    public TValue Get<TValue>(string key)
    {
        return (TValue)_dict[key];
    }

    public bool TryGetValue<TValue>(string key, out TValue? value)
    {
        if (_dict.TryGetValue(key, out var obj))
        {
            value = (TValue?)obj;
            return true;
        }

        value = default;
        return false;
    }

    public void Set(string key, object value)
    {
        _dict[key] = value;
    }

    public void CopyTo(IBag bag)
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

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
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

public class BagJsonConverter : JsonConverter<Bag>
{
    public override Bag Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(ref reader, options);
        var bag = new Bag(dict?.Count ?? 0);

        if (dict != null)
        {
            foreach (var kv in dict)
                bag.Set(kv.Key, kv.Value!);
        }

        return bag;
    }

    public override void Write(Utf8JsonWriter writer, Bag value, JsonSerializerOptions options)
    {
        var dict = value.GetDictionary();
        JsonSerializer.Serialize(writer, dict, options);
    }
}
