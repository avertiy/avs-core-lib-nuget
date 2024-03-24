using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.DLinq;

internal static class InvokeExpr
{
    /// <summary>
    /// Creates a lambda expression to invoke static extension method: 
    /// <code>
    ///  source.method(bag, spec);
    /// </code>
    /// </summary>
    public static Expression<Func<IEnumerable<T>, IEnumerable>> GetExpr<T>(MethodInfo mi, LambdaBag bag, ListDictLambdaSpec<T> spec)
    {
        var sourceParam = Expression.Parameter(typeof(IEnumerable<T>), "source");
        var bagParam = Expression.Constant(bag);
        var specParam = Expression.Constant(spec);

        // Select(prop, paramType)
        var call = Expression.Call(null, mi, sourceParam, bagParam, specParam);

        // Create lambda expression
        var lambda = Expression.Lambda<Func<IEnumerable<T>, IEnumerable>>(call, sourceParam);
        return lambda;
    }
}


internal static class LambdaBuilder
{
    


    /// <summary>
    /// Creates a lambda expression to invoke static method for one property: source.method(bag, prop, paramType);
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
    /// Creates a lambda expression to invoke static method for one property: source.method(bag, prop, index, paramType);
    /// <code>
    ///  SelectByIndexAsList{T, TValue}(IEnumerable{T} source, LambdaBag bag, PropertyInfo prop, int index, Type? paramType)
    /// </code>
    /// </summary>
    public static Expression<Func<IEnumerable<T>, IEnumerable>> InvokeExpr<T>(MethodInfo mi, LambdaBag bag, PropertyInfo prop, int index, Type? paramType)
    {
        var sourceParam = Expression.Parameter(typeof(IEnumerable<T>), "source");
        var bagParam = Expression.Constant(bag);
        var propParam = Expression.Constant(prop);
        var indParam = Expression.Constant(index);
        var paramTypeParam = Expression.Constant(paramType);

        // Select(prop, paramType)
        var call = Expression.Call(null, mi, sourceParam, bagParam, propParam, indParam, paramTypeParam);

        // Create lambda expression
        var lambda = Expression.Lambda<Func<IEnumerable<T>, IEnumerable>>(call, sourceParam);
        return lambda;
    }

    /// <summary>
    /// Creates a lambda expression to invoke static method for one property: source.method(bag, prop, key, paramType);
    /// <code>
    ///  SelectByIndexAsList{T, TValue}(IEnumerable{T} source, LambdaBag bag, PropertyInfo prop, string key, Type? paramType)
    /// </code>
    /// </summary>
    public static Expression<Func<IEnumerable<T>, IEnumerable>> InvokeExpr<T>(MethodInfo mi, LambdaBag bag, PropertyInfo prop, string key, Type? paramType)
    {
        var sourceParam = Expression.Parameter(typeof(IEnumerable<T>), "source");
        var bagParam = Expression.Constant(bag);
        var propParam = Expression.Constant(prop);
        var keyParam = Expression.Constant(key);
        var paramTypeParam = Expression.Constant(paramType);

        // Select(prop, paramType)
        var call = Expression.Call(null, mi, sourceParam, bagParam, propParam, keyParam, paramTypeParam);

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
    /// Creates lambda expression to select property: x => (Type) x.Prop
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
    /// expression: x => (paramType) x.prop[index]
    /// </summary>
    public static Expression<Func<T, TResult>> SelectItemOfPropertyExpr<T, TResult>(PropertyInfo prop, int index, Type? paramType)
    {
        // Define parameter expression for the input of the lambda expression
        var paramExpr = Expression.Parameter(typeof(T), "x");

        // Access the property specified by the PropertyInfo
        var propExpr = paramType == null
            ? Expression.Property(paramExpr, prop)
            : Expression.Property(Expression.Convert(paramExpr, paramType), prop);

        var indexExpr = Expression.Constant(index);

        Expression indexerExpr;

        if (prop.PropertyType.IsArray)
        {
            indexerExpr = Expression.ArrayIndex(propExpr, indexExpr);
        }
        else
        {
            var indexerPropName = "Item";//Item is a default's name
            indexerExpr = Expression.Property(propExpr, indexerPropName, indexExpr);
        }

        // Create lambda expression representing accessing the property
        return Expression.Lambda<Func<T, TResult>>(indexerExpr, paramExpr);
    }

    /// <summary>
    /// expression: x => (paramType) x.prop[key]
    /// </summary>
    public static Expression<Func<T, TResult>> SelectItemOfPropertyExpr<T, TResult>(PropertyInfo prop, string key, Type? paramType)
    {
        // Define parameter expression for the input of the lambda expression
        var paramExpr = Expression.Parameter(typeof(T), "x");

        // Access the property specified by the PropertyInfo
        var propExpr = paramType == null
            ? Expression.Property(paramExpr, prop)
            : Expression.Property(Expression.Convert(paramExpr, paramType), prop);

        var indexExpr = Expression.Constant(key);

        var indexerPropName = "Item"; //Item is a default's name
        var indexerExpr = Expression.Property(propExpr, indexerPropName, indexExpr);

        // Create lambda expression representing accessing the property
        return Expression.Lambda<Func<T, TResult>>(indexerExpr, paramExpr);
    }

    public static Expression<Func<object, object>> CastExpr(Type targetType)
    {
        // Create parameter expressions
        var valueParam = Expression.Parameter(typeof(object), "x");

        // Create convert expression
        var convert = Expression.Convert(valueParam, targetType);

        // Compile the expression
        return Expression.Lambda<Func<object, object>>(convert, valueParam);
    }

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