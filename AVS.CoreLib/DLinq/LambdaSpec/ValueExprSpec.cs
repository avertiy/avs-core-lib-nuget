using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AVS.CoreLib.DLinq.LambdaSpec.BasicBlocks;
using AVS.CoreLib.DLinq.Specifications;

namespace AVS.CoreLib.DLinq.LambdaSpec;

/// <summary>
/// Represent lambda specification single value selector
/// e.g. x => x.close or more complex: x => x.prop[0]['key1'].value 
/// Contain one or more building blocks: <see cref="PropSpec"/>, <see cref="IndexSpec"/>, <see cref="KeySpec"/>
/// </summary>
public class ValueExprSpec : Spec, ISpecItem
{
    private List<ISpecItem> Parts { get; set; } = new();

    public bool IsEmpty => Parts.Count == 0;

    public void AddProp(string prop)
    {
        Parts.Add(new PropSpec() {Name = prop, Raw = Raw });
    }

    public void AddIndex(int index)
    {
        Parts.Add(new IndexSpec(index));
    }

    public void AddKey(string key)
    {
        Parts.Add(new KeySpec(key){ Raw = Raw });
    }

    public void AddIndex(string input)
    {
        if (int.TryParse(input, out var index))
        {
            Parts.Add(new IndexSpec(index) { Raw = Raw });
        }
        else
        {
            Parts.Add(new KeySpec(input.Trim('"', '\'')) { Raw = Raw });
        }
    }

    public override string ToString(SpecView view)
    {
        switch (view)
        {
            case SpecView.Expr:
                return string.Join(null, Parts.Select(x => x.ToString(view)));
            case SpecView.Plain:
                return string.Join('_', Parts.Select(x => x.ToString(view)));
            default:
                return ToString();
                
        }
    }

    protected override Expression BuildValueExpr(Expression argExpr, Func<Expression, Type?> resolveType)
    {
        var expr = argExpr;

        for (var i = 0; i < Parts.Count; i++)
        {
            var spec = Parts[i];
            expr = spec.GetValueExpr(expr, resolveType);
        }

        return expr;
    }

    public static ValueExprSpec Parse(string selectExpr, SelectMode mode = SelectMode.Default)
    {
        var expr = selectExpr.TrimStart('.');
        var startInd = 0;

        if (selectExpr.StartsWith("x."))
            startInd = 2;

        var spec = new ValueExprSpec() { Mode = mode, Raw = expr };
        var ind = -1;

        for (var i = startInd; i < expr.Length; i++)
            switch (expr[i])
            {
                case '.':
                {
                    //prop.inner.value or prop[0].inner
                    if (startInd < i && ind == -1)
                        spec.AddProp(expr.Substring(startInd, i - startInd));
                    startInd = i + 1;
                    break;
                }

                case '[' when ind < 0:
                {
                    if (startInd < i)
                        spec.AddProp(expr.Substring(startInd, i - startInd));
                    startInd = i + 1;
                    ind = i;
                    break;
                }
                case ']' when ind > -1:
                {
                    var key = expr.Substring(ind + 1, i - ind - 1);
                    spec.AddIndex(key);
                    ind = -1;
                    startInd = i + 1;
                    break;
                }
            }

        if (startInd < expr.Length)
            spec.AddProp(expr.Substring(startInd, expr.Length - startInd));

        return spec;
    }
}