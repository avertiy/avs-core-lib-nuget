using System;
using System.Linq.Expressions;
using AVS.CoreLib.DLinq.Specs.CompoundBlocks;
using AVS.CoreLib.Expressions;
using AVS.CoreLib.Guards;

namespace AVS.CoreLib.DLinq.Specs.Conditioning;

/// <summary>
/// Represent a simple condition: A > 1
/// </summary>
public class ConditionSpec : SpecBase
{
    public ConditionSpec(ValueExprSpec value, ComparisonSpec comparison)
    {
        Value = value;
        Comparison = comparison;
    }

    public ValueExprSpec Value { get; set; }

    public ComparisonSpec Comparison { get; set; }

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

    public override string ToString(string arg, SpecView view = SpecView.Default)
    {
        return $"{Value.ToString(arg, view)} {Comparison.ToString(arg, view)}";
    }

    public static ConditionSpec Parse(string expr, Type argType)
    {
        Guard.Against.NullOrEmpty(expr);

        var parts = expr.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 3)
            throw new DLinqException($"Invalid syntax `{expr}` - condition expression might have at least 3 parts");

        var compSpec = ComparisonSpec.Parse(parts[1], parts[2]);
        var valueSpec = ValueExprSpec.Parse(parts[0], argType);
        var spec = new ConditionSpec(valueSpec, compSpec);
        return spec;
    }

    public override string ToString()
    {
        return $"{nameof(ConditionSpec)} {Value.ToString("x", SpecView.Default)} {Comparison.ToString("x", SpecView.Default)}";
    }
}
