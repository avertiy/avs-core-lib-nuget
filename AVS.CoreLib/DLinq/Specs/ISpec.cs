using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AVS.CoreLib.DLinq.Specs;

public interface ISpec
{
    /// <summary>
    /// holds a raw expression string 
    /// </summary>
    string? Raw { get; set; }
    Expression BuildExpr(Expression expr, LambdaContext ctx);
    string GetBody();

    string ToString(string arg, SpecView view);
}

public class LambdaContext
{
    public SelectMode Mode { get; set; }
    public object? Source { get; set; }
    public ParameterExpression? ParamExpr { get; set; }
    public Func<Expression, Type?>? ResolveTypeFn { get; set; }

    public T? GetItem<T>()
    {
        if (Source == null)
            return default;

        var src = Source as IEnumerable<T>;
        return src == null ? default : src.FirstOrDefault();
    }

    public bool TryResolveType(Expression expr, out Type type)
    {
        var t = ResolveTypeFn?.Invoke(expr);

        if (t != null)
        {
            type = t;
            return true;
        }

        type = expr.Type;
        return false;
    }
}