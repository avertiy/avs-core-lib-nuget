using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AVS.CoreLib.Expressions;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib._archive;

internal static class LambdaBagDictSelectorExtensions
{
    public static Func<T, Dictionary<string, TValue>> GetDictSelector<T, TValue>(this LambdaBag bag, PropertyInfo[] props, Type? paramType)
    {
        var propsStr = string.Join(",", props.Select(x => x.Name));
        var key = FuncHelper.GetKey<T, Dictionary<string, TValue>>(propsStr, paramType?.Name);
        if (bag.TryGetFunc(key, out Func<T, Dictionary<string, TValue>>? fn))
            return fn!;

        // build lambda expression as the following:
        //  x => {
        //      var dict = new Dictionary{string,TValue}(props.Length);
        //      dict.Add(prop1.Name.ToCamelCase(), (ParamType)x).Prop1);
        //      dict.Add(prop2.Name.ToCamelCase(), (ParamType)x).Prop2);
        //      ....
        //      return dict;
        // }
        var lambda = LambdaBuilder.SelectDictionaryExpr<T, TValue>(props, paramType);
        var func = lambda.Compile();
        bag[key] = func;
        return func;
    }

    public static Func<T, Dictionary<string, object>> GetDictSelector<T>(this LambdaBag bag, PropertyInfo[] props, Type? paramType)
    {
        var propsStr = string.Join(",", props.Select(x => x.Name));
        var key = FuncHelper.GetKey<T, Dictionary<string, object>>(propsStr, paramType?.Name);
        if (bag.TryGetFunc(key, out Func<T, Dictionary<string, object>>? fn))
            return fn!;

        // build lambda expression as the following:
        //  x => {
        //      var dict = new Dictionary{string,object}(props.Length);
        //      dict.Add(prop1.Name.ToCamelCase(), (ParamType)x).Prop1);
        //      dict.Add(prop2.Name.ToCamelCase(), (ParamType)x).Prop2);
        //      ....
        //      return dict;
        // }

        var lambda = LambdaBuilder.SelectDictionaryExpr<T>(props, paramType);
        var func = lambda.Compile();
        bag[key] = func;
        return func;
    }
}

internal static class FuncHelper
{
    public static string GetKey<T, TResult>(string arg1, string? arg2)
    {
        if (arg2 == null)
            return $"Func<{typeof(T).GetReadableName()},{typeof(TResult).GetReadableName()}>({arg1})";

        return $"Func<{typeof(T).GetReadableName()},{typeof(TResult).GetReadableName()}>({arg1}, {arg2})";
    }
}