using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;

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
        Parts.Add(new PropSpec() {Name = prop});
    }

    public void AddIndex(int index)
    {
        Parts.Add(new IndexSpec(index));
    }

    public void AddKey(string key)
    {
        Parts.Add(new KeySpec(key));
    }

    public void AddIndex(string input)
    {
        if (int.TryParse(input, out var index))
        {
            Parts.Add(new IndexSpec(index));
        }
        else
        {
            Parts.Add(new KeySpec(input.Trim('"', '\'')));
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

    protected override Expression BuildValueExpr(Expression argExpr)
    {
        var expr = argExpr;

        foreach (var spec in Parts)
        {
            expr = spec.GetValueExpr(expr);
        }

        return expr;
    }
}