using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using AVS.CoreLib.DLinq.LambdaSpec;
using AVS.CoreLib.DLinq.Specifications.BasicBlocks;
using AVS.CoreLib.Expressions;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.DLinq.Specifications;

/// <summary>
/// Represent lambda specification for select multiple values into dictionary
/// e.g. x => CreateDictionary(keys, expr1, expr2, ...)
/// Contain one or more building blocks: <see cref="Prop1Spec"/>, <see cref="Index1Spec"/>, <see cref="Key1Spec"/>, <see cref="ValueExpr1Spec"/>
/// </summary>

public class MultiPropExpr1Spec : SpecBase
{
    private Dictionary<string, ValueExpr1Spec> Items { get; set; }

    public MultiPropExpr1Spec(int capacity)
    {
        Items = new(capacity);
    }

    #region Add Key & spec
    public bool ContainsKey(string key)
    {
        return Items.ContainsKey(key);
    }

    public void AddSmart(ValueExpr1Spec item)
    {
        var key = item.GetKey().Trim('_');
        var str = ShortenKey(key);

        if (ContainsKey(str))
            str = ResolveKeyCollision(str);

        Items.Add(str, item);
    }

    private static string ShortenKey(string key)
    {
        if (key.Length < 6)
            return key;

        var parts = key.Split('_');

        if (parts.Length == 1)
            return key;

        var ind = Array.FindLastIndex(parts, x => char.IsLetter(x[0]));

        if (ind == -1)
            return key;

        if (ind > 0)
        {
            var str = parts[ind].ToLowerInvariant();
            if (str.Either("value", "item", "props"))
                ind--;
        }

        return string.Join('_', parts.Skip(ind));
    }

    private string ResolveKeyCollision(string key)
    {
        var i = 1;
        while (ContainsKey(key + "_" + i))
            i++;

        return key + "_" + i;
    }
    #endregion

    public override Expression BuildExpr(Expression expression, LambdaContext ctx)
    {
        if (Items.Count == 0)
            return expression;

        if (Items.Count == 1)
            return Items.First().Value.BuildExpr(expression, ctx);

        var keys = new string[Items.Count];
        var expressions = new List<Expression>(Items.Count);

        var sameType = true;

        foreach (var kp in Items)
        {
            var key = kp.Key;//.SanitizePropertyName();
            var valueExpr = kp.Value.BuildExpr(expression, ctx);

            keys[expressions.Count] = key;
            expressions.Add(valueExpr);

            if (sameType && expressions.Count > 1 && expressions[0].Type != valueExpr.Type)
                sameType = false;
        }

        var expr = Expr.CreateDictionaryExpr(keys, expressions.ToArray());
        return expr;
    }

    public override string ToString(string arg, SpecView view)
    {
        if (Items.Count == 0)
            return arg;

        if (Items.Count == 1)
            return Items.First().Value.ToString(arg, view);

        var sb = new StringBuilder(Items.Count * 25);
        sb.Append("new {");
        foreach (var kp in Items)
        {
            sb.Append(kp.Key);
            sb.Append(" = ");
            sb.Append(kp.Value.ToString(arg, view));
            sb.Append(", ");
        }

        sb.Length -= 2;
        sb.Append("}");

        return sb.ToString();
    }
}