using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib.DLinq.LambdaSpec;

public abstract class Spec: ILambdaSpec
{
    public string? Raw { get; set; }
    /// <summary>
    /// when x => x.Close  needs to be cast for example to object: x => (object)x.Close
    /// </summary>
    public Type? CastTo { get; set; }
    /// <summary>
    /// if T is an interface, ArgType could provide a concrete type
    /// </summary>
    public Type? ArgType { get; set; }

    public SpecMode Mode { get; set; } = SpecMode.ToList;

    public virtual Expression<Func<IEnumerable<T>, IEnumerable>> Build<T>()
    {
        try
        {
            var selectorExpr = SelectorExpr<T>(BuildValueExpr);
            return Lambda<T>(selectorExpr, selectorExpr.ReturnType);
        }
        catch (Exception ex)
        {
            throw new DLinqException($"Can't build spec - {ex.Message}", ex) { Spec = this };
        }
    }
    
    /// <summary>
    /// build lambda expr of the form: source.Select(selectorExpr).ToList()
    /// </summary>
    protected Expression<Func<IEnumerable<T>, IEnumerable>> Lambda<T>(Expression selectorExpr, Type resultType)
    {
        // call: source.Select(selector);
        var sourceParam = Expression.Parameter(typeof(IEnumerable<T>), "source");

        var selectMethod = LinqHelper.GetSelectMethodInfo(typeof(T), resultType);
        var selectExpr = Expression.Call(null, selectMethod, sourceParam, selectorExpr);
        var body = selectExpr;

        if (Mode.HasFlag(SpecMode.ToList))
        {
            var toListMethod = LinqHelper.GetToListMethodInfo(resultType);
            // add ToList() call: source.Select(selector).ToList();
            var toListExpr = Expression.Call(toListMethod, selectExpr);
            body = toListExpr;
        }

        var lambda = Expression.Lambda<Func<IEnumerable<T>, IEnumerable>>(body, sourceParam);
        return lambda;
    }

    /// <summary>
    /// build selector expression kind of x => ((?CastArg)x).Prop
    /// </summary>
    protected LambdaExpression SelectorExpr<T>(Func<Expression, Expression> getValueExpr)
    {
        // x =>
        var paramExpr = Expression.Parameter(typeof(T), "x");
        // ((?CastArg)x).Prop
        var selectorExpr = ArgType == null
            ? getValueExpr(paramExpr)
            : getValueExpr(Expression.Convert(paramExpr, ArgType));

        if (CastTo != null)
            selectorExpr = Expression.Convert(selectorExpr, CastTo);

        // selector: x => ((?CastArg)x).Prop
        var lambdaExpr = Expression.Lambda(selectorExpr, paramExpr);
        return lambdaExpr;
    }

    protected abstract Expression BuildValueExpr(Expression argExpr);

    public Expression GetValueExpr(Expression argExpr)
    {
        var expr = BuildValueExpr(argExpr);
        return Mode.HasFlag(SpecMode.Safe) ? LambdaBuilder.WrapInTryCatch(expr) : expr;
    }

    #region GetCacheKey 
    public virtual string GetCacheKey<T>()
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
            case SpecMode.ToList:
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
}