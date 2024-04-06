using System;
using System.Linq.Expressions;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib.DLinq.LambdaSpec.BasicBlocks;

public class IndexSpec : PropSpec
{
    public int Index { get; set; }
    public IndexSpec(int index)
    {
        Index = index;
    }

    protected override Expression BuildValueExpr(Expression argExpr, Func<Expression, Type?> resolveType)
    {
        var expr = base.BuildValueExpr(argExpr, resolveType);
        var indexExpr = Expression.Constant(Index);
        var type = expr.Type;

        if (type.IsArray)
            return Expression.ArrayIndex(expr, indexExpr);

        var methodInfo = type.GetIndexer();

        if (methodInfo == null)
            throw new ArgumentException($"Indexer this[int index] not found in {type.Name} type definition.");

        return Expression.Call(expr, methodInfo, indexExpr);
    }

    public override string ToString(SpecView view)
    {
        switch (view)
        {
            case SpecView.Expr:
                return Name == null ? $"[{Index}]" : $".{Name.Capitalize()}[{Index}]";
            case SpecView.Plain:
                return Name == null ? Index.ToString() : $"{Name.Capitalize()}_{Index}";
            default:
                return ToString();
        }
    }
}