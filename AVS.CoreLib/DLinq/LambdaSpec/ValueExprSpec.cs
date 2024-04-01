using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AVS.CoreLib.DLinq.LambdaSpec;

/// <summary>
/// Represent lambda specification single value selector
/// e.g. x => x.close or more complex: x => x.prop[0]['key1'].value 
/// Contain one or more building blocks: <see cref="PropSpec"/>, <see cref="IndexSpec"/>, <see cref="KeySpec"/>
/// </summary>
public class ValueExprSpec : Spec, ISpecItem
{
    private List<ISpecItem> Parts { get; set; } = new();

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
            case SpecView.Key:
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
}