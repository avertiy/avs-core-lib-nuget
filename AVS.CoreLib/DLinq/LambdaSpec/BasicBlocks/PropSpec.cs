using System;
using System.Linq.Expressions;
using System.Reflection;
using AVS.CoreLib.DLinq.Specifications;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.DLinq.LambdaSpec.BasicBlocks;

/// <summary>
/// Represent single value case
/// e.g src.Select(x => x.Close)
/// </summary>
public class PropSpec : Spec, ISpecItem
{
    public string? Name { get; set; }

    public override string ToString(SpecView view)
    {
        switch (view)
        {
            case SpecView.Expr:
                return Name == null ? string.Empty : "." + Name.Capitalize();
            case SpecView.Plain:
                return Name == null ? string.Empty : Name;//.Capitalize();
            default:
                return ToString();
        }
    }

    protected override Expression BuildValueExpr(Expression argExpr, Func<Expression, Type?> resolveType)
    {
        if (string.IsNullOrEmpty(Name))
            return argExpr;

        var expr = argExpr;
        var type = ArgType ?? expr.Type;

        var prop = type.GetProperty(Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

        if (prop == null && ArgType == null)
        {
            type = resolveType(expr);
            // if resolveType returns null => source collection is empty we can simply return
            if (type == null)
                return expr;

            prop = type.GetProperty(Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            expr = Expression.Convert(expr, type);
        }

        if (prop == null)
            throw new ArgumentException($"Public {Name} property not found in {type.Name} type definition.");

        expr = Expression.Property(expr, prop);

        return expr;
    }
}