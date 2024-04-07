using System.Linq.Expressions;
using AVS.CoreLib.DLinq.Conditions;
using AVS.CoreLib.DLinq.Specifications.BasicBlocks;

namespace AVS.CoreLib.DLinq.Specifications;

public abstract class SpecBase : ISpec
{
    public string? Raw { get; set; }
    public abstract Expression BuildExpr(Expression expr, LambdaContext ctx);
    public override string ToString()
    {
        return $"{GetType().Name} {Raw}";
    }

    public string GetBody()
    {
        return $"x => {ToString("x", SpecView.Expr)}";
    }

    public abstract string ToString(string arg, SpecView view);

    public static LogicalSpec Combine(Op op, params ISpec[] specs)
    {
        return new LogicalSpec(op, specs);
    }
}