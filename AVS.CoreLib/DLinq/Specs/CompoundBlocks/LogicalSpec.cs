using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using AVS.CoreLib.DLinq.Conditions;
using AVS.CoreLib.DLinq.Specs.Conditioning;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib.DLinq.Specs.CompoundBlocks;

/// <summary>
/// allows to combine specs with logical operators && (and), || (or)
/// </summary>
[DebuggerDisplay("LogicalSpec: {ToString()} (raw: {Raw})")]
public class LogicalSpec : SpecBase, ILambdaSpec
{
    public Op Op { get; set; }
    /// <summary>
    /// contains either <see cref="ConditionSpec"/> or another <see cref="LogicalSpec"/>
    /// </summary>
    public List<ILambdaSpec> Items { get; set; }

    public LogicalSpec(Op op, params ILambdaSpec[] items)
    {
        Items = new List<ILambdaSpec>(items);
        Op = op;

        if(op != Op.AND && op != Op.OR)
            throw new ArgumentException($"{nameof(LogicalSpec)} does not support {Op}");
    }

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

    //public override string ToString0(string arg, SpecView view = SpecView.Default)
    //{
    //    if (string.IsNullOrEmpty(arg))
    //        return string.Join($" {Op} ", Items.Select(x => x.ToString0(arg, view)));

    //    return arg + string.Join($" {Op} ", Items.Select(x => x.ToString0(arg, view)));
    //}

    public string GetCacheKey()
    {
        if (HasSameArgType(out var type))
        {
            var argType = type!.GetReadableName();
            return argType + " x => " + string.Join($" {Op} ", Items.Select(x => x.ToString()));
        }

        return string.Join($" {Op} ", Items.Select(x => x.GetCacheKey()));
    }

    public bool HasSameArgType(out Type? type)
    {
        type = null;
        foreach (var spec in IterateConditionSpecs())
        {
            if (type == null)
            {
                type = spec.Value.ArgType;
                continue;
            }

            if(spec.Value.ArgType != type)
                return false;
        }

        return true;
    }

    private IEnumerable<ConditionSpec> IterateConditionSpecs()
    {
        foreach (var item in Items)
        {
            if (item is ConditionSpec cond)
            {
                yield return cond;
                continue;
            }

            var logical = (LogicalSpec)item;

            foreach (var spec in logical.IterateConditionSpecs())
            {
                yield return spec;
            }
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder(Items.Count * 20);

        foreach (var spec in Items)
        {
            sb.Append(spec.ToString()+ " " + Op + " ");
        }

        if(Items.Count > 0)
            sb.Length--;
        
        return sb.ToString();
    }

    public static LogicalSpec Combine(Op op, params ILambdaSpec[] specs)
    {
        return new LogicalSpec(op, specs);
    }
}