using System;
using System.Linq.Expressions;
using System.Reflection;
using AVS.CoreLib.DLinq;
using AVS.CoreLib.DLinq0;

namespace AVS.CoreLib.DLinq0.LambdaSpec0;

public abstract class SpecItem<T> where T : SpecItem<T>
{
    public PropertyInfo Property { get; set; }
    public int Index { get; set; } = -1;
    public string? Key { get; set; }

    public T? Inner { get; set; }
    protected SpecItem(PropertyInfo property)
    {
        Property = property;
    }

    public void Add(T inner)
    {
        if (Inner == null)
        {
            Inner = inner;
            return;
        }

        Inner.Add(inner);
    }

    protected virtual Expression GetInnerPropertyExpr(Expression paramExpr)
    {
        var valueExpr = (Expression)Expression.Property(paramExpr, Property);

        if (Index > -1)
        {
            var indexExpr = Expression.Constant(Index);
            valueExpr = Property.PropertyType.IsArray
                ? Expression.ArrayIndex(valueExpr, indexExpr)
                : Expression.Property(valueExpr, "Item", indexExpr);
        }
        else if (Key != null)
        {
            var indexExpr = Expression.Constant(Key);
            valueExpr = Expression.Property(valueExpr, "Item", indexExpr);
        }

        if (Inner == null)
            return valueExpr;

        return Inner.GetInnerPropertyExpr(valueExpr);
    }
}


public class SpecItem : SpecItem<SpecItem>
{
    public SpecItem(PropertyInfo property) : base(property)
    {
    }

    public static SpecItem Create(Lexeme lexeme, PropertyInfo propertyInfo)
    {
        var lex = lexeme;
        var prop = propertyInfo;
        var item = new SpecItem(prop) { Key = lex.Key, Index = lex.Index };

        while (lex.Inner != null)
        {
            prop = lex.GetInnerProperty(prop);
            var inner = new SpecItem(prop) { Key = lex.Inner.Key, Index = lex.Inner.Index };
            item.Add(inner);
            lex = lex.Inner;
        }

        return item;
    }

    /// <summary>
    /// expression: x => (castTo) x.prop or x.prop[index] or x.prop[key]
    /// </summary>
    public Expression GetExpression(Expression paramExpr, Type? castTo = null)
    {
        //paramExpr: ((XBar)x).prop

        var valueExpr = GetInnerPropertyExpr(paramExpr);

        if (castTo != null)
            valueExpr = Expression.Convert(valueExpr, castTo);

        var expr = LambdaBuilder.WrapInTryCatch(valueExpr);
        return expr;
    }
}