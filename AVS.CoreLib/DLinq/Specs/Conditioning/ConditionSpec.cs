using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using AVS.CoreLib.DLinq.Enums;
using AVS.CoreLib.DLinq.Specs.LambdaSpecs;
using AVS.CoreLib.Expressions;
using AVS.CoreLib.Guards;

namespace AVS.CoreLib.DLinq.Specs.Conditioning;

/// <summary>
/// Represent a simple condition: A > 1
/// </summary>
[DebuggerDisplay("ConditionSpec: {Value} {Comparison}")]
public class ConditionSpec : SpecBase, ILambdaSpec
{
    public ValueExprSpec Value { get; set; }
    public ComparisonSpec Comparison { get; set; }

    public ConditionSpec(ValueExprSpec value, ComparisonSpec comparison)
    {
        Value = value;
        Comparison = comparison;
    }

    public override Expression BuildExpr(Expression expression, LambdaContext ctx)
    {
        var expr = Value.BuildExpr(expression, ctx);

        if (ctx.Mode.HasFlag(SelectMode.Safe))
        {
            expr = Expr.WrapInTryCatch(expr);
        }

        expr = Comparison.BuildExpr(expr, ctx);
        return expr;
    }
    
    public string GetCacheKey()
    {
        return $"{Value.GetCacheKey()} {Comparison}";
    }

    public static ConditionSpec Parse(string expr, Type argType, Dictionary<string, ValueExprSpec> specs)
    {
        Guard.Against.NullOrEmpty(expr);

        var parts = expr.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 3)
            throw new DLinqException($"Invalid syntax `{expr}` - condition expression might have at least 3 parts");

        var compSpec = ComparisonSpec.Parse(parts[1], parts[2]);

        var valueExpr = parts[0].Trim();
        var valueSpec = specs.ContainsKey(valueExpr) ? specs[valueExpr] : ValueExprSpec.Parse(valueExpr, argType);
        var spec = new ConditionSpec(valueSpec, compSpec);
        return spec;
    }

    public override string ToString()
    {
        return $"{Value} {Comparison}";
    }
}
