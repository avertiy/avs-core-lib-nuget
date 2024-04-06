using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AVS.CoreLib.DLinq.Conditions;

namespace AVS.CoreLib.DLinq.LambdaSpec;

public class LogicalSpec : Spec
{
    public LogicalSpec(Op op, params Spec[] items)
    {
        Items = new List<Spec>(items);
        Op = op;
    }

    public Op Op { get; set; }

    public List<Spec> Items { get; set; }

    public Type? CastTo { get; set; }
    public Type? ArgType { get; set; }

    public override string ToString(SpecView view)
    {
        return string.Join($" {Op} ", Items.Select(x => x.ToString(view)));
    }

    protected override Expression BuildValueExpr(Expression argExpr, Func<Expression, Type?> resolveType)
    {
        throw new NotImplementedException();
    }

    public Expression GetValueExpr(Expression argExpr, Func<Expression, Type?> resolveType)
    {
        throw new NotImplementedException();
    }
}