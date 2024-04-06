using System;
using System.Linq.Expressions;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib.DLinq.LambdaSpec.BasicBlocks;

public class KeySpec : PropSpec
{
    public string Key { get; set; }

    public KeySpec(string key)
    {
        Key = key;
    }

    protected override Expression BuildValueExpr(Expression argExpr, Func<Expression, Type?> resolveType)
    {
        var expr = base.BuildValueExpr(argExpr, resolveType);
        var type = expr.Type;
        var methodInfo = type.GetKeyIndexer();

        if (methodInfo == null)
            throw new ArgumentException($"Indexer this[string key] not found in {type.Name} type definition.");

        expr = Expression.Call(expr, methodInfo, Expression.Constant(Key));
        return expr;
    }

    public override string ToString(SpecView view)
    {
        switch (view)
        {
            case SpecView.Expr:
                return Name == null ? $"[\"{Key}\"]" : $".{Name.Capitalize()}[\"{Key}\"]";
            case SpecView.Plain:
                return Name == null ? Key : $"{Name.Capitalize()}_{Key}";
            default:
                return ToString();
        }
    }
}