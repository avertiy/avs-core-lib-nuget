using System;
using System.Collections.Generic;
using AVS.CoreLib.Guards;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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

    public static MethodCallExpression CreateDictionaryExpr(string[] keys, params Expression[] expressions)
    {
        Guard.Array.MustHaveAtLeast(expressions, 1);

        var useTypedDict = expressions.All(x => x.Type == expressions[0].Type);

        var method =  useTypedDict
            ? CreateDictionaryMethodInfo(expressions[0].Type)
            : CreateDictionaryMethodInfo();
        
        var keysExpr = Expression.Constant(keys);

        var valuesExpr = useTypedDict 
            ? Expression.NewArrayInit(expressions[0].Type, expressions)
            : Expression.NewArrayInit(typeof(object), expressions.Select(x => Expression.Convert(x, typeof(object))));

        var expr = Expression.Call(null, method, keysExpr, valuesExpr);
        return expr;
    }

    private static MethodInfo CreateDictionaryMethodInfo()
    {
        return typeof(XActivator).GetMethod(nameof(CreateDictionary), BindingFlags.Static | BindingFlags.Public)!;
    }

    private static MethodInfo CreateDictionaryMethodInfo(Type typeArg)
    {
        var method = typeof(XActivator).GetMethod(nameof(CreateValueDictionary), BindingFlags.Static | BindingFlags.Public)!;
        return method.MakeGenericMethod(typeArg);
    }
}