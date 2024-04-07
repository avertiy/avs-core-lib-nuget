using System.Linq.Expressions;
using AVS.CoreLib.Extensions;
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