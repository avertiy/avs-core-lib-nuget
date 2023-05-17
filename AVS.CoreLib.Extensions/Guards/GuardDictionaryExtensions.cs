using System;
using System.Collections.Generic;
using System.Linq;

namespace AVS.CoreLib.Guards;

public static class GuardDictionaryExtensions
{
    public static void CheckIndex<TKey, TValue>(this IDictionaryGuardClause guardClause, int index, IDictionary<TKey, TValue> dict, string? message = null)
    {
        if (index < 0 || index >= dict.Count)
            throw new ArgumentOutOfRangeException(message ?? $"[{index}] should be within range [0; {dict.Count - 1}]");
    }

    public static void MustContainKey<TKey, TValue>(this IDictionaryGuardClause guardClause, IDictionary<TKey, TValue> dict, TKey key, string name = "dictionary")
    {
        if (!dict.ContainsKey(key))
            throw new ArgumentException($"{name} must contain key {key}");
    }

    /// <summary>
    /// validates dictionary contains only specified (valid/supported) keys 
    /// </summary>
    public static void ValidKeys<TKey, TValue>(this IDictionaryGuardClause guardClause, IDictionary<TKey, TValue> dict, TKey[] validKeys, string name = "dictionary")
    {
        foreach (var key in dict.Keys)
            if (!validKeys.Contains(key))
                throw new ArgumentException($"{name} contains invalid key: {key}");
    }

    /// <summary>
    /// validates dictionary contains only specified (supported) keys 
    /// </summary>
    public static void SupportedKeys<TKey, TValue>(this IDictionaryGuardClause guardClause, IDictionary<TKey, TValue> dict, TKey[] supportedKeys, string name = "dictionary")
    {
        foreach (var key in dict.Keys)
            if (!supportedKeys.Contains(key))
                throw new ArgumentException($"{name} contains not supported key: {key}");
    }

    public static void MustContainKeys<TKey, TValue>(this IDictionaryGuardClause guardClause, IDictionary<TKey, TValue> dict, params TKey[] keys)
    {
        foreach (var key in keys)
            if (!dict.ContainsKey(key))
                throw new ArgumentException($"Must contain key {key}");
    }
}