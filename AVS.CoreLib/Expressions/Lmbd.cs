using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using AVS.CoreLib.DLinq;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib.Expressions;

public static class Lmbd
{
    [DebuggerStepThrough]
    public static Expression<Func<IEnumerable<T>, IEnumerable>> Create<T>(Expression body, ParameterExpression param)
    {
        return Expression.Lambda<Func<IEnumerable<T>, IEnumerable>>(body, param);
    }

    /// <summary>
    /// Creates lambda: x => ((Type)x)
    /// </summary>
    public static Expression<Func<T, object>> Cast<T>(Type targetType)
    {
        // Create parameter expressions
        var paramExpr = Expression.Parameter(typeof(T), "x");

        // Create convert expression
        var convert = Expression.Convert(paramExpr, targetType);

        // Compile the expression
        return Expression.Lambda<Func<T, object>>(convert, paramExpr);
    }

    /// <summary>
    /// Creates lambda: x => ((TypeArg)x).Prop;
    /// </summary>
    public static LambdaExpression SelectProp<TSource>(PropertyInfo prop, Type? typeArg)
    {
        var paramExpr = Expression.Parameter(typeof(TSource), "x");

        var propExpr = Expression.Property(paramExpr.Cast(typeArg), prop);

        var lambdaExpr = Expression.Lambda(propExpr, paramExpr);
        return lambdaExpr;
    }

    public static Expression<Func<TSource, TResult>> SelectProp<TSource, TResult>(PropertyInfo prop, Type? typeArg)
    {
        var paramExpr = Expression.Parameter(typeof(TSource), "x");

        var propExpr = Expression.Property(paramExpr.Cast(typeArg), prop);

        var lambdaExpr = Expression.Lambda<Func<TSource, TResult>>(propExpr, paramExpr);
        return lambdaExpr;
    }

    /// <summary>
    /// Creates lambda: x => XActivator.CreateDictionary(keys, new object[] {expr1, expr2, ...});
    /// </summary>
    public static LambdaExpression SelectDict<TSource>(PropertyInfo[] props, Type? typeArg)
    {
        var paramExpr = Expression.Parameter(typeof(TSource), "x");
        var selectorExpr = Expr.CreateDictionaryExpr<TSource>(paramExpr.Cast(typeArg), props);
        var lambdaExpr = Expression.Lambda(selectorExpr, paramExpr);
        return lambdaExpr;
    }

    /// <summary>
    /// Creates lambda: source.Select(selector);
    /// </summary>
    public static Expression<Func<IEnumerable<T>, IEnumerable>> Select<T>(LambdaExpression selectorExpr, SelectMode mode)
    {
        var sourceParam = Expression.Parameter(typeof(IEnumerable<T>), "source");
        var selectMethod = LinqHelper.GetSelectMethodInfo(typeof(T), selectorExpr.ReturnType);
        Expression body = Expression.Call(null, selectMethod, sourceParam, selectorExpr);
        
        if (mode.HasFlag(SelectMode.ToList))
        {
            var toListMethod = LinqHelper.GetToListMethodInfo(selectorExpr.ReturnType);
            // add ToList() call: source.Select(selector).ToList();
            body = Expression.Call(toListMethod, body);
        }

        if (mode.HasFlag(SelectMode.Safe))
        {
            body = Expr.WrapInTryCatch(body);
        }

        var lambda = Lmbd.Create<T>(body, sourceParam);
        return lambda;
    }

   
}