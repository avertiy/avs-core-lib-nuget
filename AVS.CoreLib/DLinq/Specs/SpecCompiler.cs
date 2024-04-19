using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AVS.CoreLib.DLinq.Enums;
using AVS.CoreLib.DLinq.Specs.LambdaSpecs;
using AVS.CoreLib.Expressions;
using AVS.CoreLib.Extensions.Enums;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib.DLinq.Specs;

public static class SpecCompiler
{
    #region BuildSelectFn
    public static Func<IEnumerable<T>, IEnumerable> BuildSelectFn<T>(ILambdaSpec spec, LambdaContext ctx)
    {
        try
        {
            var paramExpr = Expression.Parameter(typeof(T), "x");
            //bars.Select(x => ((IXBar)x).Prop, ((IBar1)x).ATR)
            ctx.ParamExpr = paramExpr;
            ctx.ResolveTypeFn = e => ResolveType<T>(e, ctx);

            var expr = spec.BuildExpr(paramExpr, ctx);
            var lambdaExpr = Expression.Lambda(expr, paramExpr);

            var fn = CompileLambda<T>(lambdaExpr, ctx.Mode);
            return fn;
        }
        catch (Exception ex)
        {
            throw new DLinqException($"Can't build lambda  - {ex.Message}", ex, spec);
        }
    }

    

    /// <summary>
    /// build lambda expr of the form:
    /// <code>source => Select(source, x => selector(x)).ToList()</code>
    /// </summary>
    private static Func<IEnumerable<T>, IEnumerable> CompileLambda<T>(LambdaExpression selectorExpr, SelectMode mode)
    {
        // call: source.Select(selector);
        var sourceParam = Expression.Parameter(typeof(IEnumerable<T>), "source");

        var selectMethod = LinqHelper.GetSelectMethodInfo(typeof(T), selectorExpr.ReturnType);
        var selectExpr = Expression.Call(null, selectMethod, sourceParam, selectorExpr);
        var body = selectExpr;

        if (mode.HasFlag(SelectMode.ToList))
        {
            var toListMethod = LinqHelper.GetToListMethodInfo(selectorExpr.ReturnType);
            // add ToList() call: source.Select(selector).ToList();
            body = Expression.Call(toListMethod, body);
        }

        return Lmbd.Compile<IEnumerable<T>, IEnumerable>(body, sourceParam);
    } 
    #endregion

    public static Func<IEnumerable<T>, Sort, IEnumerable<T>> BuildOrderByFn<T>(ValueExprSpec spec, LambdaContext ctx)
    {
        var type = typeof(T);
        var paramExpr = Expression.Parameter(type, "x");

        ctx.ParamExpr = paramExpr;
        ctx.ResolveTypeFn = e => ResolveType<T>(e, ctx);

        var expr = spec.BuildExpr(paramExpr, ctx);
        var lambdaExpr = Expression.Lambda(expr, paramExpr);

        var sourceParam = Expression.Parameter(typeof(IEnumerable<T>), "source");
        var method = LinqHelper.GetOrderByMethodInfo(type, lambdaExpr.ReturnType);

        var directionParam = Expression.Parameter(typeof(Sort), "direction");

        var body = Expression.Call(null, method, sourceParam, lambdaExpr, directionParam);

        return Lmbd.Compile<IEnumerable<T>, Sort, IEnumerable<T>>(body, sourceParam, directionParam);
    }

    public static Func<IOrderedEnumerable<T>, Sort, IOrderedEnumerable<T>> BuildThenByFn<T>(ValueExprSpec spec, LambdaContext ctx)
    {
        var type = typeof(T);
        var paramExpr = Expression.Parameter(type, "x");

        ctx.ParamExpr = paramExpr;
        ctx.ResolveTypeFn = e => ResolveType<T>(e, ctx);

        var expr = spec.BuildExpr(paramExpr, ctx);
        var lambdaExpr = Expression.Lambda(expr, paramExpr);

        var sourceParam = Expression.Parameter(typeof(IOrderedEnumerable<T>), "source");
        //var orderedSourceParam = Expression.Convert(sourceParam, typeof(IOrderedEnumerable<T>));

        var method = LinqHelper.GetThenByMethodInfo(type, lambdaExpr.ReturnType);

        var directionParam = Expression.Parameter(typeof(Sort), "direction");

        var body = Expression.Call(null, method, sourceParam, lambdaExpr, directionParam);

        return Lmbd.Compile<IOrderedEnumerable<T>, Sort, IOrderedEnumerable<T>>(body, sourceParam, directionParam);
    }
    
    public static Func<T, bool> BuildPredicate<T>(ILambdaSpec spec, LambdaContext ctx)
    {
        var paramExpr = Expression.Parameter(typeof(T), "x");
        //bars.Select(x => ((IXBar)x).Prop, ((IBar1)x).ATR)
        ctx.ParamExpr = paramExpr;
        ctx.ResolveTypeFn = e => ResolveType<T>(e, ctx);

        //ctx.ResolveTypeFn = resolveType;

        var body = spec.BuildExpr(paramExpr, ctx);
        return Lmbd.Compile<T, bool>(body, paramExpr);
    }

    public static Func<T, TResult> BuildSelector<T, TResult>(ValueExprSpec spec, LambdaContext ctx)
    {
        try
        {
            var paramExpr = Expression.Parameter(typeof(T), "x");
            ctx.ParamExpr = paramExpr;
            ctx.ResolveTypeFn = e => ResolveType<T>(e, ctx);
            var expr = spec.BuildExpr(paramExpr, ctx);

            var targetType = typeof(TResult);
            if(expr.Type != targetType && !expr.Type.IsAssignableTo(targetType))
                expr = Expression.Convert(expr, targetType);

            return Lmbd.Compile<T, TResult>(expr, paramExpr);
        }
        catch (Exception ex)
        {
            throw new DLinqException($"Can't build selector - {ex.Message}", ex, spec);
        }
    }

    /// <summary>
    /// build either Func{T,decimal/double/int} selector, that supposed to be used with aggregate linq extensions (Sum,Max,Min,Avg)
    /// </summary>
    public static Delegate BuildAggregateFnSelector<T>(ValueExprSpec spec, LambdaContext ctx)
    {
        try
        {
            var paramExpr = Expression.Parameter(typeof(T), "x");
            ctx.ParamExpr = paramExpr;
            ctx.ResolveTypeFn = e => ResolveType<T>(e, ctx);
            var expr = spec.BuildExpr(paramExpr, ctx);
            
            var decType = typeof(decimal);

            if (expr.Type == decType)
            {
                return Lmbd.Compile<T, decimal>(expr, paramExpr);
            }

            if (expr.Type == typeof(int))
            {
                return Lmbd.Compile<T, int>(expr, paramExpr);
            }

            if (expr.Type == typeof(double))
            {
                return Lmbd.Compile<T, double>(expr, paramExpr);
            }

            expr = Expression.Convert(expr, decType);
            return Lmbd.Compile<T, decimal>(expr, paramExpr);
        }
        catch (Exception ex)
        {
            throw new DLinqException($"Can't build selector - {ex.Message}", ex, spec);
        }
    }

    /// <summary>
    /// when ResultType of part of the expression is object, we can't pick nested props
    /// pick first item of the source sequence and determine type from it
    /// </summary>
    private static Type? ResolveType<T>(Expression expr, LambdaContext ctx)
    {
        var item = ctx.GetItem<T>();
        if (item == null)
            return null;

        var getValFn = Lmbd.Compile<T, object>(expr, ctx.ParamExpr!);
        var val = getValFn(item);
        return val.GetType();
    }
}