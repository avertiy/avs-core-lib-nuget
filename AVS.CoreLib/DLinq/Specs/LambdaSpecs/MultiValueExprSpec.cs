using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using AVS.CoreLib.DLinq.Enums;
using AVS.CoreLib.Expressions;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.DLinq.Extensions;
namespace AVS.CoreLib.DLinq.Specs.LambdaSpecs;

/// <summary>
/// Represent lambda specification with multiple value expressions <see cref="ValueExprSpec"/>
/// Selector 
/// Contain one or more <see cref="ValueExprSpec"/>
/// </summary>
public class MultiValueExprSpec : SpecBase, ILambdaSpec
{
    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public Dictionary<string, ValueExprSpec> Items { get; private set; }

    public int Count => Items.Count;

    public MultiValueExprSpec(int capacity)
    {
        Items = new(capacity);
    }

    public MultiValueExprSpec(IDictionary<string, ValueExprSpec> dict)
    {
        Items = new(dict.Count);

        foreach (var kp in dict)
        {
            var key = ShortenKey(kp.Key);

            if (ContainsKey(key))
                key = ResolveKeyCollision(key);

            Items.Add(key, kp.Value);
        }

    }

    #region Add Key & spec
    public bool ContainsKey(string key)
    {
        return Items.ContainsKey(key);
    }

    public void AddSmart(ValueExprSpec item)
    {
        var parts = item.Parts.Select(x => x.GetKey()).ToArray();

        var key = parts[0];

        if (parts.Length > 1)
        {
            var ind = Array.FindLastIndex(parts, x => char.IsLetter(x[0]));
            if (ind > 0 && parts[ind].ToLowerInvariant().Either("value", "item", "props"))
                ind--;
            key = string.Join('_', parts.Skip(ind));
        }

        if (ContainsKey(key))
            key = ResolveKeyCollision(key);

        Items.Add(key, item);
    }

    private static string ShortenKey(string key)
    {
        if (key.Length < 6)
            return key;

        var str = key;
        string[]? parts = null;

        var ind = str.LastIndexOf(']');

        if (ind > -1)
            if (ind < str.Length - 2)
            {
                //prop[0].prop1.prop2
                str = str.Substring(ind + 1);
                parts = str.Split('.', StringSplitOptions.RemoveEmptyEntries);
            }
            else
                parts = str.Split('[', ']').Select(x => x.Trim('"')).Where(x => x.Length > 0).ToArray();
        else// if (str.Contains('.'))
            parts = str.Split('.', StringSplitOptions.RemoveEmptyEntries);
        //else
        //{
        //    parts = str.Split('_', StringSplitOptions.RemoveEmptyEntries);
        //}

        if (parts.Length == 1)
            return parts[0];

        ind = Array.FindLastIndex(parts, x => char.IsLetter(x[0]));

        if (ind == -1)
            return str;

        if (ind > 0)
        {
            str = parts[ind].ToLowerInvariant();
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

    /// <summary>
    /// Builds an expression that pick multiple values putting them into a dictionary
    /// A result expression looks like: x => XActivator.CreateDictionary(keys, expr1, expr2, ...)
    /// </summary>
    public override Expression BuildExpr(Expression expression, LambdaContext ctx)
    {
        if (Items.Count == 0)
            return expression;

        if (Items.Count == 1)
            return Items.First().Value.BuildExpr(expression, ctx);

        var keys = new List<string>(Items.Count);
        var expressions = new List<Expression>(Items.Count);

        ctx.Expressions = new Dictionary<string, Expression>(Items.Count);

        var sameType = true;

        foreach (var kp in Items)
        {
            var key = kp.Key;
            var valueExpr = kp.Value.BuildExpr(expression, ctx);

            ctx.Expressions.Add(key, valueExpr);

            if (kp.Value.Shortcut)
                continue;

            keys.Add(key);
            expressions.Add(valueExpr);

            if (sameType && expressions.Count > 1 && expressions[0].Type != valueExpr.Type)
                sameType = false;
        }

        var expr = Expr.CreateDictionaryExpr(keys.ToArray(), expressions.ToArray());
        return expr;
    }

    //public override string ToString0(string arg, SpecView view = SpecView.Default)
    //{
    //    if (Items.Count == 0)
    //        return arg;

    //    if (Items.Count == 1)
    //        return Items.First().Value.ToString0(arg, view);

    //    var sb = new StringBuilder(Items.Count * 25);
    //    sb.Append("new {");
    //    foreach (var kp in Items)
    //    {
    //        sb.Append(kp.Key);
    //        sb.Append(" = ");
    //        sb.Append(kp.Value.ToString0(arg, view));
    //        sb.Append(", ");
    //    }

    //    sb.Length -= 2;
    //    sb.Append("}");

    //    return sb.ToString();
    //}

    public string GetCacheKey()
    {
        var sb = new StringBuilder(Items.Count * 25);
        sb.Append('{');
        foreach (var kp in Items)
        {
            sb.Append('{');
            sb.Append(kp.Key);
            sb.Append(":");
            sb.Append(kp.Value.GetCacheKey());
            sb.Append("}, ");
        }

        if (sb.Length > 2)
            sb.Length -= 2;

        sb.Append('}');
        return sb.ToString();
    }
}

internal static class MultiValueExprSpecExtensions
{
    public static IEnumerable Execute<T>(this MultiValueExprSpec spec, IEnumerable<T> source, SelectMode mode = SelectMode.Default)
    {
        var items = spec.Items;

        if (items.Count == 0 || items.All(x => x.Value.Shortcut))
            return source;

        if (items.Any(x => x.Value.Fn > 0))
            return spec.ExecuteAggregation(source, mode);

        return source.Select(spec, mode);
    }

    private static IEnumerable ExecuteAggregation<T>(this MultiValueExprSpec spec, IEnumerable<T> source, SelectMode mode)
    {
        var dict = new Dictionary<string, ValueExprSpec>(spec.Items.Count);
        var alias = false;
        var shortcuts = new Dictionary<string, ValueExprSpec>();

        foreach (var kp in spec.Items)
        {
            if (kp.Value.Shortcut)
            {
                shortcuts.Add(kp.Key, kp.Value);
                continue;
            }

            if (kp.Value.Fn == 0)
                continue;

            var valueExpr = kp.Value;

            var name = valueExpr.Parts[0].Name;
            if (name != null && shortcuts.ContainsKey(name))
            {
                var shortcutSpec = shortcuts[name];
                valueExpr.ApplyShortcut(shortcutSpec);
            }

            if (!alias && kp.Value.Alias != null)
                alias = true;

            dict.Add(kp.Key, valueExpr);
        }

        if (dict.Count == 0)
            return source;

        if (dict.Count == 1)
            return dict.Select(x => source.Aggregate(x.Value, mode)).ToArray();

        // multi aggregates case
        var arr = source.ToArray();

        IEnumerable result = alias
            ? dict.ToDictionary(x => x.Key, x => arr.Aggregate(x.Value, mode))
            : dict.Select(x => arr.Aggregate(x.Value, mode)).ToList();

        return result;
    }
}

