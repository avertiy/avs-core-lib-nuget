using System.Linq.Expressions;
using AVS.CoreLib.DLinq.LambdaSpec;

namespace AVS.CoreLib.DLinq.Specifications;

public abstract class SpecBase : ISpec
{
    public string? Raw { get; set; }
    public abstract Expression BuildExpr(Expression expr, LambdaContext ctx);
    //public abstract string ToString(SpecView view);
    public override string ToString()
    {
        return $"{GetType().Name} {Raw}";
    }

    public string GetBody()
    {
        return $"x => {ToString("x", SpecView.Expr)}";
    }

    public string GetKey()
    {
        return ToString(string.Empty, SpecView.Plain);
    }

    public abstract string ToString(string arg, SpecView view);
}