using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;

namespace AVS.CoreLib.DLinq;

public class DictSpecItem
{
    public string Key { get; set; }
    public PropertyInfo Property { get; set; }
    public int ValueIndex { get; set; } = -1;
    public string? ValueKey { get; set; }
    public DictSpecItem? Inner { get; set; }

    public DictSpecItem(string key, PropertyInfo property)
    {
        Key = key;
        Property = property;
    }

    public Expression GetExpression(Expression instance, MethodInfo methodInfo, Expression paramExpr, Type? castTo = null)
    {
        var keyExpr = Expression.Constant(Key);
        //paramExpr: ((XBar)x).Atr
        // dict.Add(key, (XBar)x).Atr)
        var valueExpr = GetInnerPropertyExpr(paramExpr);

        if (castTo != null)
            valueExpr = Expression.Convert(valueExpr, castTo);

        var call = Expression.Call(instance, methodInfo, keyExpr, valueExpr);
        return call;
    }

    private Expression GetInnerPropertyExpr(Expression paramExpr)
    {
        var valueExpr = (Expression)Expression.Property(paramExpr, Property);

        if (ValueIndex > -1)
        {
            var indexExpr = Expression.Constant(ValueIndex);
            valueExpr = Property.PropertyType.IsArray
                ? Expression.ArrayIndex(valueExpr, indexExpr)
                : Expression.Property(valueExpr, "Item", indexExpr);
        }
        else if (ValueKey != null)
        {
            var indexExpr = Expression.Constant(ValueKey);
            valueExpr = Expression.Property(valueExpr, "Item", indexExpr);
        }

        if (Inner == null)
            return valueExpr;

        return Inner.GetInnerPropertyExpr(valueExpr);
    }

    public void Add(DictSpecItem inner)
    {
        if (Inner == null)
        {
            Inner = inner;
            return;
        }

        Inner.Add(inner);
    }
}