using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AVS.CoreLib.DLinq.Enums;
using AVS.CoreLib.Extensions.Reflection;
using AVS.CoreLib.Lambdas;

namespace AVS.CoreLib._archive;

internal static class LambdaBagGetSelectFnExtensions
{
    /// <summary>
    /// Gets delegate: source => source.Select(x => ((Type)x).Prop);
    /// </summary>
    internal static Func<IEnumerable<T>, IEnumerable> GetSelectFn<T>(this LambdaBag bag, PropertyInfo prop, Type? paramType, SelectMode mode = SelectMode.Default)
    {
        // invoke: source.Select(x => x.Prop);
        var type = typeof(T);
        var typeName = type.GetReadableName();

        var key = $"source.Select<{typeName},{prop.PropertyType.GetReadableName()}>(x => ({paramType?.GetReadableName() ?? typeName})x.{prop.Name})) [mode:{mode}]";
        if (bag.TryGetFunc(key, out Func<IEnumerable<T>, IEnumerable>? fn))
            return fn!;


        var selectorExpr = Lmbd.SelectProp<T>(prop, paramType);
        var lambda = Lmbd.Select<T>(selectorExpr, mode);
        var func = lambda.Compile();
        bag[key] = func;
        return func;
    }

    /// <summary>
    /// Gets delegate: source => source.Select(x => XActivator.CreateDictionary(props, paramType)) => IEnumerable of Dictionary{string, object or TValue} 
    /// </summary>
    internal static Func<IEnumerable<T>, IEnumerable> GetSelectDictFn<T>(this LambdaBag bag, PropertyInfo[] props, Type? paramType, SelectMode mode = SelectMode.Default)
    {
        var propsStr = string.Join(",", props.Select(x => x.Name));
        var key = $"source.Select<{typeof(T).GetReadableName()}>(x => CreateDictionary({propsStr}, {paramType?.GetReadableName()})) [mode:{mode}]";
        if (bag.TryGetFunc(key, out Func<IEnumerable<T>, IEnumerable>? fn))
            return fn!;

        var selectorExpr = Lmbd.SelectDict<T>(props, paramType);
        var lambda = Lmbd.Select<T>(selectorExpr, mode);
        var func = lambda.Compile();
        bag[key] = func;
        return func;
    }
}