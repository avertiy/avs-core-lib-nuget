using System.Diagnostics;
using System.Linq.Expressions;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib.DLinq.Specs.BasicBlocks;

/// <summary>
/// Represent indexer by string key expression specification: 
/// <code>
/// 1. ["key"]
/// 2. prop["key"]
/// </code>
/// </summary>
[DebuggerDisplay("KeySpec: {ToString()}")]
public class KeySpec : PropSpec
{
    public string Key { get; set; }
    public KeySpec(string key)
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
            expr = Expression.Convert(expr, type);
            methodInfo = type.GetKeyIndexer();
        }

        if (methodInfo == null)
            throw new SpecException($"Indexer this[string key] not found in {type.Name} type definition.", this);

        expr = Expression.Call(expr, methodInfo, Expression.Constant(Key));
        return expr;
    }

    public override string GetKey()
    {
        return Name == null ? Key : $"{Name}_{Key}";
    }

    public override string ToString()
    {
        return $"{Name}[\"{Key}\"]";
    }

    ///// <summary>
    ///// Converts spec to its string representation
    ///// <code>
    ///// p/plain -> prop_key or key 
    ///// </code> 
    ///// </summary>
    //public string ToString(string format)
    //{
    //    switch (format)
    //    {
    //        case "p":
    //        case "plain":
    //            return Name == null ? Key : $"{Name}_{Key}";
    //        default:
    //            return ToString();
    //    }
    //}

    public override string Format(string expr)
    {
        return Name == null ? $"{expr}[\"{Key}\"]" : $"{expr}.{Name}[\"{Key}\"]";
    }
}