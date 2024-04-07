using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AVS.CoreLib.Expressions;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib.DLinq.Specs;

public static class SpecCompiler
{
    public static Func<IEnumerable<T>, IEnumerable> BuildFn<T>(ISpec spec, LambdaContext ctx)
    {
        try
        {
            var paramExpr = Expression.Parameter(typeof(T), "x");
            //bars.Select(x => ((IXBar)x).Prop, ((IBar1)x).ATR)
            ctx.ParamExpr = paramExpr;
            ctx.ResolveTypeFn = resolveType;

            var expr = spec.BuildExpr(paramExpr, ctx);
            var lambdaExpr = Expression.Lambda(expr, paramExpr);

            var fn = CompileLambda<T>(lambdaExpr, ctx.Mode);
            return fn;

            // when result type is object we can't look up nested props, this trick to resolve runtime type
            Type? resolveType(Expression e)
            {
                var item = ctx.GetItem<T>();
                if (item == null)
                    return null;

                var getValFn = Lmbd.Compile<T, object>(e, ctx.ParamExpr!);
                var val = getValFn(item);
                return val.GetType();
            }
        }
        catch (Exception ex)
        {
            throw new DLinqException($"Can't build lambda  - {ex.Message}", ex);
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
}