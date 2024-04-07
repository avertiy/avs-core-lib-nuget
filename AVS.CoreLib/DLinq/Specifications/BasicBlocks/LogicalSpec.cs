using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AVS.CoreLib.DLinq.Conditions;

namespace AVS.CoreLib.DLinq.Specifications.BasicBlocks;

/// <summary>
/// allows to combine specs with logical operators && (and), || (or)
/// </summary>
public class LogicalSpec : SpecBase
{
    public LogicalSpec(Op op, params ISpec[] items)
    {
        Items = new List<ISpec>(items);
        Op = op;
    }

    public Op Op { get; set; }

    public List<ISpec> Items { get; set; }


    public override Expression BuildExpr(Expression expr, LambdaContext ctx)
    {
        return expr;
    }

    public override string ToString(string arg, SpecView view)
    {
        if(string.IsNullOrEmpty(arg))
            return string.Join($" {Op} ", Items.Select(x => x.ToString(arg, view)));

        return arg + string.Join($" {Op} ", Items.Select(x => x.ToString(arg, view)));
    }
}