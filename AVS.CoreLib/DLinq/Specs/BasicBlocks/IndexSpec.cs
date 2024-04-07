using System.Linq.Expressions;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib.DLinq.Specs.BasicBlocks;

/// <summary>
/// Builds indexer expressions 
/// <code>
/// 1. x[Index]
/// 2. x.Prop[Index]
/// </code>
/// </summary>
public class IndexSpec : PropSpec
{
    public int Index { get; set; }
    public IndexSpec(int index)
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