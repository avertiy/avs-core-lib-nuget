using System.Linq.Expressions;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib.DLinq.Specs.BasicBlocks;

/// <summary>
/// Builds indexer expressions 
/// <code>
/// 1. x[Key]
/// 2. x.Prop[Key]
/// </code>
/// </summary>
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

    public override string ToString(string arg, SpecView view = SpecView.Default)
    {
        return view switch
        {
            SpecView.Expr => $"{base.ToString(arg, view)}[\"{Key}\"]",
            SpecView.Plain => $"{base.ToString(arg, view)}_{Key}",
            _ => $"{base.ToString(arg, view)}[{Key}]"
        };
    }

    public override string ToString()
    {
        return $"{nameof(KeySpec)} [{Key}]";
    }
}