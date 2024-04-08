using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using AVS.CoreLib.DLinq.Conditions;

namespace AVS.CoreLib.DLinq.Specs.CompoundBlocks;

/// <summary>
/// allows to combine specs with logical operators && (and), || (or)
/// </summary>
public class LogicalSpec : SpecBase
{
    public LogicalSpec(Op op, params ISpec[] items)
    {
        Items = new List<ISpec>(items);
        Op = op;

        if(op != Op.AND && op != Op.OR)
            throw new ArgumentException($"{nameof(LogicalSpec)} does not support {Op}");
    }

    public Op Op { get; set; }

    public List<ISpec> Items { get; set; }


    public override Expression BuildExpr(Expression expression, LambdaContext ctx)
    {
        var expr = Items[0].BuildExpr(expression, ctx);

        if (Items.Count == 1)
            return expr;

        for (var i = 1; i < Items.Count; i++)
        {
            var rightExpr = Items[i].BuildExpr(expression, ctx);

            expr = Op switch
            {
                Op.AND => Expression.And(expr, rightExpr),
                Op.OR => Expression.Or(expr, rightExpr),
                _ => expr
            };
        }

        return expr;
    }

    public override string ToString(string arg, SpecView view = SpecView.Default)
    {
        if (string.IsNullOrEmpty(arg))
            return string.Join($" {Op} ", Items.Select(x => x.ToString(arg, view)));

        return arg + string.Join($" {Op} ", Items.Select(x => x.ToString(arg, view)));
    }

    public override string ToString()
    {
        var sb = new StringBuilder(Items.Count * 20);

        foreach (var item in Items)
        {
            sb.Append(item.ToString("x"));
            sb.Append(" " +Op + " ");
        }

        if(Items.Count > 0)
            sb.Length--;
        
        return sb.ToString();
    }
}