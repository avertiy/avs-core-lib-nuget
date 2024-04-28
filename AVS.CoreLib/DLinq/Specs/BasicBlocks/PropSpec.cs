using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.DLinq.Specs.BasicBlocks;

/// <summary>
/// Represent a property expression specification
/// <code>Prop</code>
/// </summary>
[DebuggerDisplay("PropSpec: {Name}")]
public class PropSpec : SpecBase, ILambdaSpec
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
            throw new SpecException($"Property `{Name}` not found in {type.Name} type definition (property must be public).", this);

        var outputExpr = Expression.Property(expr, prop);
        return outputExpr;
    }
    
    protected static PropertyInfo? LookupProperty(Type type, string name)
    {
        return type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
    }
    
    public override string GetCacheKey()
    {
        return Name!;
    }

    public virtual string GetKey()
    {
        return Name!;
    }

    public override string ToString()
    {
        return Name ?? string.Empty;
    }

    public virtual string Format(string expr)
    {
        return Name == null ? expr : $"{expr}.{Name}";
    }
}