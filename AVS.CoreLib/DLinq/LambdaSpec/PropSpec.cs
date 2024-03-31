using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.DLinq.LambdaSpec;

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
            case SpecView.Key:
                return Name == null ? string.Empty : Name;//.Capitalize();
            default:
                return ToString();
        }
    }

    protected override Expression BuildValueExpr(Expression argExpr)
    {
        if (string.IsNullOrEmpty(Name))
            return argExpr;

        var type = argExpr.Type;

        var prop = type.GetProperty(Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

        var expr = argExpr;

        if (prop == null)
        {

        }
        else
        {
            expr = Expression.Property(argExpr, prop);
        }

        return expr;
    }

    private static PropertyInfo LookupProperty(Type type, string name)
    {
        var prop = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

        if (prop == null)
            throw new ArgumentException($"Public {name} property not found in {type.Name} type definition.");

        return prop;
    }
}