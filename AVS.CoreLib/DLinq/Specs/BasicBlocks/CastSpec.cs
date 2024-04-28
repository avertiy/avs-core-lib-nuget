using System;
using System.Diagnostics;
using System.Linq.Expressions;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib.DLinq.Specs.BasicBlocks;

/// <summary>
/// Cast (convert) to ArgType
/// <code>(ArgType)x</code> 
/// </summary>
[DebuggerDisplay("CastSpec {ArgType}")]
public class CastSpec : SpecBase
{
    public CastSpec(Type argType)
    {
        ArgType = argType;
    }

    public Type ArgType { get; set; }

    public override Expression BuildExpr(Expression expr, LambdaContext ctx)
    {
        if (expr.Type == ArgType)
            return expr;

        return Expression.Convert(expr, ArgType);
    }

    public override string GetCacheKey()
    {
        return $"x => (({ArgType.GetReadableName()})x)";
    }

    public string Format(string arg)
    {
        return $"(({ArgType.GetReadableName()}){arg})";
    }
}