using System;
using System.Collections.Generic;
using System.Reflection;
using AVS.CoreLib.Guards;

namespace AVS.CoreLib.Utilities;

/// <summary>
/// XActivator helps to build lambda expressions:
/// <code>
///     x => XActivator.CreateList(x.Prop1, x.Prop2[0], x.Prop3[key1]);
///     x => XActivator.CreateDictionary(keys, x.Prop1, x.Prop2[0], x.Prop3[key1])
///     x => XActivator.CreateValueDictionary{decimal}(keys, x.Prop1, x.Prop2[0], x.Prop3[key1])
/// </code>
/// </summary>
public static class XActivator
{
    public static IList<TValue> CreateList<TValue>(params TValue[] values)
    {
        return new List<TValue>(values);
    }

    public static IDictionary<string, object> CreateDictionary(string[] keys, params object[] values)
    {
        Guard.MustBe.Equal(keys.Length, values.Length, $"keys count {keys.Length} must equal values count {values.Length}");
        var dict = new Dictionary<string, object>(keys.Length);
        for (var i = 0; i < keys.Length; i++)
            dict.Add(keys[i], values[i]);
        return dict;
    }

    /// <summary>
    /// Typed dictionary is used to avoid boxing/unboxing
    /// </summary>
    public static IDictionary<string, TValue> CreateValueDictionary<TValue>(string[] keys, params TValue[] values)
    {
        Guard.MustBe.Equal(keys.Length, values.Length, $"keys count {keys.Length} must equal values count {values.Length}");
        var dict = new Dictionary<string, TValue>(keys.Length);
        for (var i = 0; i < keys.Length; i++)
            dict.Add(keys[i], values[i]);
        return dict;
    }

    internal static MethodInfo CreateDictionaryMethodInfo()
    {
        return typeof(XActivator).GetMethod(nameof(CreateDictionary), BindingFlags.Static | BindingFlags.Public)!;
    }

    internal static MethodInfo CreateDictionaryMethodInfo(Type typeArg)
    {
        var method = typeof(XActivator).GetMethod(nameof(CreateValueDictionary), BindingFlags.Static | BindingFlags.Public)!;
        return method.MakeGenericMethod(typeArg);
    }
}