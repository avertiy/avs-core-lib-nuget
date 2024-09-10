using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using AVS.CoreLib.DLinq.Enums;
using AVS.CoreLib.DLinq.Specs.CompoundBlocks;
using AVS.CoreLib.DLinq.Specs.Predicates.Comparison;
using AVS.CoreLib.Guards;
using AVS.CoreLib.Lambdas;

namespace AVS.CoreLib.DLinq.Specs.Predicates;

/// <summary>
/// Represent a simple predicate expression specification e.g. x => x > 1
/// </summary>
[DebuggerDisplay("PredicateSpec: {Value} {Comparison}")]
public class PredicateSpec : SpecBase, ILambdaSpec
{
    public ValueExprSpec Value { get; set; }
    public ComparisonSpec Comparison { get; set; }

    public PredicateSpec(ValueExprSpec value, ComparisonSpec comparison)
    {
        Value = value;
        Comparison = comparison;
    }

    public override Expression BuildExpr(Expression expression, LambdaContext ctx)
    {
        var expr = Value.BuildExpr(expression, ctx);

        if (ctx.Mode.HasFlag(SelectMode.Safe))
            expr = Expr.WrapInTryCatch(expr);

        expr = Comparison.BuildExpr(expr, ctx);
        return expr;
    }

    public override string GetCacheKey()
    {
        return $"{Value.GetCacheKey()} {Comparison}";
    }

    public override string ToString()
    {
        return $"{Value} {Comparison}";
    }

    public static PredicateSpec Parse(string expr, DLinqContext context)
    {
        Guard.Against.NullOrEmpty(expr);

        var parts = expr.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 3)
            throw new DLinqException($"Invalid syntax `{expr}` - condition expression might have at least 3 parts");

        var compSpec = ComparisonSpec.Parse(parts[1], parts[2]);

        var valueExpr = parts[0].Trim();

        var valueSpec = context.Items.FirstOrDefault(x => x.Name == valueExpr) ??
                        ValueExprSpec.Parse(valueExpr, context.Type);
        var spec = new PredicateSpec(valueSpec, compSpec);
        return spec;
    }


}
