using System;
using System.Linq.Expressions;
using System.Reflection;
using AVS.CoreLib.DLinq.LambdaSpec;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib.DLinq.Specifications.BasicBlocks;

/// <summary>
/// Builds property expression
/// <code>x.Name</code>
/// </summary>
public class Prop1Spec : SpecBase
{
    public string? Name { get; set; }

    public override Expression BuildExpr(Expression expr, LambdaContext ctx)
    {
        if (string.IsNullOrEmpty(Name))
            return expr;
            //throw new LambdaSpecException("Property Name is empty", this);

        var type = expr.Type;

        var prop = LookupProperty(type, Name);

        if (prop == null && ctx.TryResolveType(expr, out type))
            prop = LookupProperty(type, Name);

        if (prop == null)
            throw new LambdaSpecException($"Public {Name} property not found in {type.Name} type definition.", this);

        var outputExpr = Expression.Property(expr, prop);
        return outputExpr;
    }
    
    protected static PropertyInfo? LookupProperty(Type type, string name)
    {
        return type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
    }

    public override string ToString(string arg, SpecView view)
    {
        return view switch
        {
            SpecView.Expr => Name == null ? arg : $"{arg}.{Name.Capitalize()}",
            SpecView.Plain => Name == null ? arg : arg.Length > 0 ? $"{arg}_{Name}" : Name,
            _ => ToString()
        };
    }
}

/// <summary>
/// Builds indexer expressions 
/// <code>
/// 1. x[Key]
/// 2. x.Prop[Key]
/// </code>
/// </summary>
public class Key1Spec : Prop1Spec
{
    public string Key { get; set; }
    public Key1Spec(string key)
    {
        Key = key;
    }

    public override Expression BuildExpr(Expression expression, LambdaContext ctx)
    {
        var expr = base.BuildExpr(expression, ctx);
        var type = expr.Type;
        var methodInfo = type.GetKeyIndexer();

        if (methodInfo == null && ctx.TryResolveType(expr, out type))
        {
            methodInfo = type.GetKeyIndexer();
        }

        if (methodInfo == null)
            throw new LambdaSpecException($"Indexer this[string key] not found in {type.Name} type definition.", this);

        expr = Expression.Call(expr, methodInfo, Expression.Constant(Key));
        return expr;
    }

    public override string ToString(string arg, SpecView view)
    {
        return view switch
        {
            SpecView.Expr => Name == null ? $"{arg}[\"{Key}\"]" : $"{arg}.{Name.Capitalize()}[\"{Key}\"]",
            SpecView.Plain => Name == null ? string.Join('_', arg, Key) : string.Join('_', arg, Name, Key),
            _ => ToString()
        };
    }
}

/// <summary>
/// Builds indexer expressions 
/// <code>
/// 1. x[Index]
/// 2. x.Prop[Index]
/// </code>
/// </summary>
public class Index1Spec : Prop1Spec
{
    public int Index { get; set; }
    public Index1Spec(int index)
    {
        Index = index;
    }

    public override Expression BuildExpr(Expression expression, LambdaContext ctx)
    {
        var expr = base.BuildExpr(expression, ctx);
        var type = expr.Type;

        if (type.IsArray)
            return Expression.ArrayIndex(expr, Expression.Constant(Index));

        var methodInfo = type.GetIndexer();

        if (methodInfo == null && ctx.TryResolveType(expr, out type))
        {
            methodInfo = type.GetKeyIndexer();
        }

        if (methodInfo == null)
            throw new LambdaSpecException($"Indexer this[int index] not found in {type.Name} type definition.", this);

        expr = Expression.Call(expr, methodInfo, Expression.Constant(Index));
        return expr;
    }

    public override string ToString(string arg, SpecView view)
    {
        return view switch
        {
            SpecView.Expr => Name == null ? $"{arg}[{Index}]" : $"{arg}.{Name.Capitalize()}[{Index}]",
            SpecView.Plain => Name == null ? string.Join('_', arg, Index.ToString()) : string.Join('_', arg, Name, Index.ToString()),
            _ => ToString()
        };
    }
}