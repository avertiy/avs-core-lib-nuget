using System;
using System.Linq.Expressions;

namespace AVS.CoreLib.DLinq.Specs;

public abstract class SpecBase
{
    public string? Raw { get; set; }
    public abstract Expression BuildExpr(Expression expr, LambdaContext ctx);
    
    public override string ToString()
    {
        return $"{GetType().Name} (raw:{Raw})";
    }
}