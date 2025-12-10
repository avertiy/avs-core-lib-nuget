#nullable enable
using System.Collections.Generic;

namespace AVS.CoreLib.Abstractions.Collections;

/// <summary>
/// Represents a mutable collection of key-value pairs, where key is a string and value is an object.
/// </summary>
public interface IBag : IEnumerable<KeyValuePair<string, object>>
{
    int Count { get; }
    bool ContainsKey(string key);
    string[] GetAllKeys();
    void EnsureCapacity(int capacity);
    
    TValue? GetOrDefault<TValue>(string key);
    TValue Get<TValue>(string key);
    bool TryGetValue<TValue>(string key, out TValue? value);
    void Set(string key, object value);
    void CopyTo(IBag bag);

    IDictionary<string, object> GetDictionary();
}

/// <summary>
/// Typed bag represents a mutable collection of key-value pairs, where key is a string
/// </summary>
public interface IBag<T> : IEnumerable<KeyValuePair<string, T>>
{
    int Count { get; }
    bool ContainsKey(string key);
    string[] GetAllKeys();
    void EnsureCapacity(int capacity);
    T? GetOrDefault(string key);
    T Get(string key);
    bool TryGetValue(string key, out T? value);
    void Set(string key, T value);
    void CopyTo(IBag<T> bag);
    string GetTypeName();
}