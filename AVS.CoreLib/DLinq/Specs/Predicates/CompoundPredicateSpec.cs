using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using AVS.CoreLib.DLinq;
using AVS.CoreLib.DLinq.Enums;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib.DLinq.Specs.Predicates;

/// <summary>
/// allows to combine specs with logical operators && (and), || (or)
/// contains either <see cref="PredicateSpec"/> or another <see cref="CompoundPredicateSpec"/>
/// </summary>
[DebuggerDisplay("LogicalSpec: {ToString()} (raw: {Raw})")]
public class CompoundPredicateSpec : CompoundSpec<ILambdaSpec>, ILambdaSpec
{
    public Op Op { get; set; }

    public CompoundPredicateSpec(Op op, params ILambdaSpec[] items) : base(items)
    {
        Items = new List<ILambdaSpec>(items);
        Op = op;

        if (op != Op.AND && op != Op.OR)
            throw new ArgumentException($"{nameof(CompoundPredicateSpec)} does not support {Op}");
    }

    public virtual Expression BuildExpr(Expression expression, LambdaContext ctx)
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

    public string GetCacheKey()
    {
        if (SameArgType(out var type))
        {
            var argType = type!.GetReadableName();
            return argType + " x => " + string.Join($" {Op} ", Items.Select(x => x.ToString()));
        }

        return string.Join($" {Op} ", Items.Select(x => x.GetCacheKey()));
    }

    protected override IEnumerable<Type> IterateArgTypes()
    {
        return IterateConditionSpecs().Select(x => x.Value.ArgType);
    }

    private IEnumerable<PredicateSpec> IterateConditionSpecs()
    {
        foreach (var item in Items)
        {
            if (item is PredicateSpec cond)
            {
                yield return cond;
                continue;
            }

            var logical = (CompoundPredicateSpec)item;

            foreach (var spec in logical.IterateConditionSpecs())
                yield return spec;
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder(Items.Count * 20);

        foreach (var spec in Items)
            sb.Append(spec.ToString() + " " + Op + " ");

        if (Items.Count > 0)
            sb.Length--;

        return sb.ToString();
    }

    public static CompoundPredicateSpec Combine(Op op, params ILambdaSpec[] specs)
    {
        return new CompoundPredicateSpec(op, specs);
    }
}