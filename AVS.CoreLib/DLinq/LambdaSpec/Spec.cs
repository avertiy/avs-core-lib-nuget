using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AVS.CoreLib.DLinq.Conditions;
using AVS.CoreLib.Expressions;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib.DLinq.LambdaSpec;

public abstract class Spec : ISpecItem
{
    public string? Raw { get; set; }
    /// <summary>
    /// Converts value to ResultType e.g. (object)x.Close
    /// </summary>
    public Type? CastTo { get; set; }
    /// <summary>
    /// converts to ArgType e.g. (ArgType)x
    /// </summary>
    public Type? ArgType { get; set; }

    public SelectMode Mode { get; set; } = SelectMode.ToList;

    public virtual IEnumerable Process<T>(IEnumerable<T> source)
    {
        var key = GetCacheKey<T>();
        var bag = LambdaBag.Lambdas;

        if (!bag.TryGetFunc(key, out Func<IEnumerable<T>, IEnumerable>? fn))
        {
            try
            {
                // x =>
                var paramExpr = Expression.Parameter(typeof(T), "x");

                Expression argExpr = ArgType == null ? paramExpr : Expression.Convert(paramExpr, ArgType);

                var selectorExpr = BuildValueExpr(argExpr, expr =>
                {
                    var item = source.FirstOrDefault();
                    if (item == null)
                        return null;

                    var lambda = Expression.Lambda<Func<T,object>>(expr, paramExpr);
                    var getValFn = lambda.Compile();
                    var val = getValFn(item);
                    return val.GetType();
                });

                if (CastTo != null)
                    selectorExpr = Expression.Convert(selectorExpr, CastTo);

                // selector: x => ((?CastArg)x).Prop
                var lambdaExpr = Expression.Lambda(selectorExpr, paramExpr);

                // source => Select(source, x => selector(x)).ToList()
                var lambda = Lambda<T>(lambdaExpr);
                fn = lambda.Compile();

                bag[key] = fn;
            }
            catch (Exception ex)
            {
                throw new DLinqException($"Can't build lambda  - {ex.Message}", ex);
            }
        }

        try
        {
            return fn!.Invoke(source);
        }
        catch (Exception ex)
        {
            throw new DLinqException($"Invoke lambda failed - {ex.Message}", ex);
        }
    }

    protected abstract Expression BuildValueExpr(Expression argExpr, Func<Expression, Type?> resolveType);

    /// <summary>
    /// build lambda expr of the form: source.Select(selectorExpr).ToList()
    /// </summary>
    private Expression<Func<IEnumerable<T>, IEnumerable>> Lambda<T>(LambdaExpression selectorExpr)
    {
        // call: source.Select(selector);
        var sourceParam = Expression.Parameter(typeof(IEnumerable<T>), "source");

        var selectMethod = LinqHelper.GetSelectMethodInfo(typeof(T), selectorExpr.ReturnType);
        var selectExpr = Expression.Call(null, selectMethod, sourceParam, selectorExpr);
        var body = selectExpr;

        if (Mode.HasFlag(SelectMode.ToList))
        {
            var toListMethod = LinqHelper.GetToListMethodInfo(selectorExpr.ReturnType);
            // add ToList() call: source.Select(selector).ToList();
            body = Expression.Call(toListMethod, body);
        }

        var lambda = Expression.Lambda<Func<IEnumerable<T>, IEnumerable>>(body, sourceParam);
        return lambda;
    }
    
    public Expression GetValueExpr(Expression argExpr, Func<Expression, Type?> resolveType)
    {
        var expr = BuildValueExpr(ArgType == null ? argExpr : Expression.Convert(argExpr, ArgType), resolveType);
        return Mode.HasFlag(SelectMode.Safe) ? Expr.WrapInTryCatch(expr) : expr;
    }

    #region GetCacheKey 
    protected string GetCacheKey<T>()
    {
        var body = GetBodyExpr();
        return FormatMode($"Select({typeof(T).GetReadableName()} {body}) [mode:{Mode}]");
    }

    protected virtual string GetBodyExpr()
    {
        var expr = ToString(SpecView.Expr);
        var body = ArgType == null ? $"x => x{expr}" : $"x => (({ArgType.GetReadableName()})x){expr}";
        return body;
    }

    public abstract string ToString(SpecView view);

    private string FormatMode(string key)
    {
        switch (Mode)
        {
            case SelectMode.ToList:
                return $"{key}.ToList()";
            default:
                return key;
        }
    } 
    #endregion

    public override string ToString()
    {
        return $"{GetType().Name}: {Mode} {Raw ?? GetBodyExpr()}";
    }

    public static LogicalSpec Combine(Op op, params Spec[] specs)
    {
        return new LogicalSpec(op, specs);
    }
}