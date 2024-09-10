using System.Linq.Expressions;

namespace AVS.CoreLib.DLinq.Specs;

public abstract class SpecBase : ILambdaSpec
{
    public string? Raw { get; set; }
    public abstract Expression BuildExpr(Expression expr, LambdaContext ctx);
    public abstract string GetCacheKey();
    public override string ToString()
    {
        return $"{GetType().Name} (raw:{Raw})";
    }
}