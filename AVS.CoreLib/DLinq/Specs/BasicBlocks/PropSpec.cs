using System;
using System.Linq.Expressions;
using System.Reflection;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.DLinq.Specs.BasicBlocks;

/// <summary>
/// Builds property expression
/// <code>x.Name</code>
/// </summary>
public class PropSpec : SpecBase
{
    public string? Name { get; set; }

    public override Expression BuildExpr(Expression expression, LambdaContext ctx)
    {
        if (string.IsNullOrEmpty(Name))
            return expression;

        var expr = expression;
        var type = expr.Type;
        var prop = LookupProperty(type, Name);

        if (prop == null && ctx.TryResolveType(expr, out type))
        {
            prop = LookupProperty(type, Name);
            expr = Expression.Convert(expr, type);
        }

        if (prop == null)
            throw new SpecException($"Public {Name} property not found in {type.Name} type definition.", this);

        var outputExpr = Expression.Property(expr, prop);
        return outputExpr;
    }
    
    protected static PropertyInfo? LookupProperty(Type type, string name)
    {
        return type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
    }

    public override string ToString(string arg, SpecView view = SpecView.Default)
    {
        return view switch
        {
            SpecView.Expr => Name == null ? arg : $"{arg}.{Name.Capitalize()}",
            SpecView.Plain => Name == null ? arg : arg.Length > 0 ? $"{arg}_{Name}" : Name,
            _ => Name == null ? arg : $"{arg}.{Name}"
        };
    }

    public override string ToString()
    {
        return $"{nameof(PropSpec)} {Name}";
    }
}