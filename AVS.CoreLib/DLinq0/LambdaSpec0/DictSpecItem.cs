using System;
using System.Linq.Expressions;
using System.Reflection;
using AVS.CoreLib.DLinq;
using AVS.CoreLib.DLinq0;

namespace AVS.CoreLib.DLinq0.LambdaSpec0;

public class DictSpecItem : SpecItem<DictSpecItem>
{
    /// <summary>
    /// represent dictionary key i.e. dict.Add(key, value);
    /// </summary>
    public string ItemKey { get; set; }

    public DictSpecItem(string key, PropertyInfo property) : base(property)
    {
        ItemKey = key;
    }

    public static DictSpecItem Create(Lexeme lexeme, PropertyInfo propertyInfo)
    {
        var key = lexeme.GetResultKey();
        var lex = lexeme;
        var prop = propertyInfo;
        var item = new DictSpecItem(key, prop) { Key = lex.Key, Index = lex.Index };

        while (lex.Inner != null)
        {
            prop = lex.GetInnerProperty(prop);
            var inner = new DictSpecItem(key, prop) { Key = lex.Inner.Key, Index = lex.Inner.Index };
            item.Add(inner);
            lex = lex.Inner;
        }

        return item;
    }

    public Expression GetExpression(Expression instance, MethodInfo addMethod, Expression paramExpr, Type? castTo = null)
    {
        var keyExpr = Expression.Constant(ItemKey);
        //paramExpr: ((XBar)x).Atr
        // dict.Add(key, (XBar)x).Atr)
        var valueExpr = GetInnerPropertyExpr(paramExpr);

        if (castTo != null)
            valueExpr = Expression.Convert(valueExpr, castTo);

        var call = Expression.Call(instance, addMethod, keyExpr, valueExpr);

        var expr = LambdaBuilder.WrapInTryCatch(call);
        return expr;
    }
}