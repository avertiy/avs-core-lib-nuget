using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib._archive;

internal static class LambdaBuilder
{
    /// <summary>
    /// Creates lambda expression:
    /// <code>
    ///  x => {
    ///      var dict = new Dictionary{string,TValue}(props.Length);
    ///      dict.Add(prop1.Name.ToCamelCase(), (ParamType)x).Prop1);
    ///      dict.Add(prop2.Name.ToCamelCase(), (ParamType)x).Prop2);
    ///      ....
    ///      return dict;
    /// }
    /// </code> 
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