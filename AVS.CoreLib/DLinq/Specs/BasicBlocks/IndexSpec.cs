using System;
using System.Diagnostics;
using System.Linq.Expressions;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib.DLinq.Specs.BasicBlocks;

/// <summary>
/// Represent an int indexer expression specification
/// <code>
/// 1. [index]
/// 2. prop[index]
/// </code>
/// </summary>
[DebuggerDisplay("IndexSpec: {ToString()}")]
public class IndexSpec : PropSpec
{
    public int Index { get; set; }
    public IndexSpec(int index)
    {
        Index = index;
    }

    public override Expression BuildExpr(Expression expression, LambdaContext ctx)
    {
        var expr = base.BuildExpr(expression, ctx);
        var type = expr.Type;

        if (type.IsArray)
            return Expression.ArrayIndex(expr, Expression.Constant(Index));

        var methodInfo = type.GetIndexer();

        if (methodInfo == null && ctx.TryResolveType(expr, out type))
        {
            expr = Expression.Convert(expr, type);
            methodInfo = type.GetKeyIndexer();
        }

        if (methodInfo == null)
            throw new SpecException($"Indexer this[int index] not found in {type.Name} type definition.", this);

        expr = Expression.Call(expr, methodInfo, Expression.Constant(Index));
        return expr;
    }
    
    public override string GetKey()
    {
        return Name == null ? Index.ToString(): $"{Name}_{Index}";
    }

    public override string GetCacheKey()
    {
        return ToString();
    }

    public override string ToString()
    {
        return $"{Name}[{Index}]";
    }

    /// <summary>
    /// Converts spec to its string representation
    /// <code>
    /// p/plain -> prop_0 or 0 
    /// _ -> ToString() -> prop[0]
    /// </code> 
    /// </summary>
    public string ToString(string format)
    {
        switch (format)
        {
            case "p":
            case "plain":
                return Name == null ? Index.ToString() : $"{Name}_{Index}";
            default:
                return ToString();
        }
    }

    public override string Format(string expr)
    {
        return $"{base.Format(expr)}[{Index}]";
    }
}