using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.DLinq;

internal static class LambdaBuilder
{
    /// <summary>
    /// Creates a lambda expression to invoke static method for one property:
    /// <code>
    ///  SelectList{T, TValue}(IEnumerable{T} source, LambdaBag bag, PropertyInfo prop, Type? paramType)
    /// </code>
    /// </summary>
    public static Expression<Func<IEnumerable<T>, IEnumerable>> InvokeExpr<T>(MethodInfo mi, LambdaBag bag, PropertyInfo prop, Type? paramType)
    {
        var sourceParam = Expression.Parameter(typeof(IEnumerable<T>), "source");
        var bagParam = Expression.Constant(bag);
        var propParam = Expression.Constant(prop);
        var paramTypeParam = Expression.Constant(paramType);

        // Select(prop, paramType)
        var call = Expression.Call(null, mi, sourceParam, bagParam, propParam, paramTypeParam);

        // Create lambda expression
        var lambda = Expression.Lambda<Func<IEnumerable<T>, IEnumerable>>(call, sourceParam);
        return lambda;
    }

    /// <summary>
    /// Creates a lambda expression to invoke static method for multiple properties:
    /// <code>
    ///  SelectListOfDict{T}(IEnumerable{T} source, LambdaBag bag, PropertyInfo[] props, Type? paramType)
    ///   or
    ///  SelectListOfDict{T,TValue}(IEnumerable{T} source, LambdaBag bag, PropertyInfo[] props, Type? paramType)
    /// </code>
    /// </summary>
    public static Expression<Func<IEnumerable<T>, IEnumerable>> InvokeExpr<T>(MethodInfo mi, LambdaBag bag, PropertyInfo[] props, Type? paramType)
    {
        var sourceParam = Expression.Parameter(typeof(IEnumerable<T>), "source");
        var bagParam = Expression.Constant(bag);
        var propParam = Expression.Constant(props);
        var paramTypeParam = Expression.Constant(paramType);

        // Select(prop, paramType)
        var call = Expression.Call(null, mi, sourceParam, bagParam, propParam, paramTypeParam);

        // Create lambda expression
        var lambda = Expression.Lambda<Func<IEnumerable<T>, IEnumerable>>(call, sourceParam);
        return lambda;
    }

    /// <summary>
    /// Creates lambda expression to select property: x => x.Prop
    /// </summary>
    public static Expression<Func<T, TResult>> SelectPropertyExpr<T, TResult>(PropertyInfo prop, Type? paramType)
    {
        // Define parameter expression for the input of the lambda expression
        var paramExpr = Expression.Parameter(typeof(T), "x");

        // Access the property specified by the PropertyInfo
        var propExpr = paramType == null
            ? Expression.Property(paramExpr, prop)
            : Expression.Property(Expression.Convert(paramExpr, paramType), prop);

        // Create lambda expression representing accessing the property
        return Expression.Lambda<Func<T, TResult>>(propExpr, paramExpr);
    }

    /// <summary>
    /// Creates lambda expression: x => new Dictionary{string,TValue}(props.Length) { {Prop1 = x.Prop1}, {Prop2 = x.Prop2},... }
    /// </summary>
    public static Expression<Func<T, Dictionary<string, TValue>>> SelectDictionaryExpr<T, TValue>(PropertyInfo[] props, Type? paramType)
    {
        // Define parameter expression for the input of the lambda expression
        var paramExpr = Expression.Parameter(typeof(T), "x");
        var paramCastExpr = Expression.Convert(paramExpr, paramType ?? typeof(T));


        var dictType = typeof(Dictionary<string, TValue>);
        var addMethodInfo = dictType.GetMethod("Add")!;

        // Declare variable: Dictionary<string,TValue> dict
        var dictExpr = Expression.Variable(dictType, "dict");
        var dictConstructor = dictType.GetConstructor(new[] { typeof(int) })!;

        //var dict = new Dictionary<string, TValue>(props.Length);
        var dictAssignExpr = Expression.Assign(dictExpr, Expression.New(dictConstructor, Expression.Constant(props.Length)));
        //var dictAssignExpr = Expression.Assign(dictExpr, Expression.New(dictType));


        var list = new List<Expression>(props.Length + 2) { dictAssignExpr };

        list.AddRange(props.Select(prop =>
        {
            var key = prop.Name.ToCamelCase();
            var keyExpr = Expression.Constant(key);
            //((XBar)x).Atr
            var valueExpr = Expression.Property(paramCastExpr, prop);
            // dict.Add(key, (XBar)x).Atr)
            var call = Expression.Call(dictExpr, addMethodInfo, keyExpr, valueExpr);
            return call;
        }));

        list.Add(dictExpr);

        // Combine the add expressions into a block
        var blockExpr = Expression.Block(dictType, new[] { dictExpr }, list);
        var lambda = Expression.Lambda<Func<T, Dictionary<string, TValue>>>(blockExpr, paramExpr);
        return lambda;
    }

    /// <summary>
    /// Creates lambda expression: x => new Dictionary{string,object}(props.Length) { {Prop1 = (object)x.Prop1}, {Prop2 = (object)x.Prop2},... }
    /// </summary>
    public static Expression<Func<T, Dictionary<string, object>>> SelectDictionaryExpr<T>(PropertyInfo[] props, Type? paramType)
    {
        // Define parameter expression for the input of the lambda expression
        var paramExpr = Expression.Parameter(typeof(T), "x");
        var paramCastExpr = Expression.Convert(paramExpr, paramType ?? typeof(T));


        var dictType = typeof(Dictionary<string, object>);
        var addMethodInfo = dictType.GetMethod("Add")!;

        // Declare variable: Dictionary<string,TValue> dict
        var dictExpr = Expression.Variable(dictType, "dict");
        var dictConstructor = dictType.GetConstructor(new[] { typeof(int) })!;

        //var dict = new Dictionary<string, TValue>(props.Length);
        var dictAssignExpr = Expression.Assign(dictExpr, Expression.New(dictConstructor, Expression.Constant(props.Length)));
        //var dictAssignExpr = Expression.Assign(dictExpr, Expression.New(dictType));

        var objType = typeof(object);
        var list = new List<Expression>(props.Length + 2) { dictAssignExpr };

        list.AddRange(props.Select(prop =>
        {
            var key = prop.Name.ToCamelCase();
            var keyExpr = Expression.Constant(key);
            //((XBar)x).Atr
            var valueExpr = Expression.Property(paramCastExpr, prop);
            // dict.Add(key, (XBar)x).Atr)

            var valueToObjectCastExpr = Expression.Convert(valueExpr, objType);

            var call = Expression.Call(dictExpr, addMethodInfo, keyExpr, valueToObjectCastExpr);
            return call;
        }));

        list.Add(dictExpr);

        // Combine the add expressions into a block
        var blockExpr = Expression.Block(dictType, new[] { dictExpr }, list);
        var lambda = Expression.Lambda<Func<T, Dictionary<string, object>>>(blockExpr, paramExpr);
        return lambda;
    }
}