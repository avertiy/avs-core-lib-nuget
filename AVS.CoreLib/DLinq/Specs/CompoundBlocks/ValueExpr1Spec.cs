using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AVS.CoreLib.DLinq.Specs.BasicBlocks;
using AVS.CoreLib.Expressions;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib.DLinq.Specs.CompoundBlocks;

/// <summary>
/// Represent lambda specification single value selector
/// e.g. x => x.close or more complex: x => x.prop[0]['key1'].value 
/// Contain one or more building blocks: <see cref="PropSpec"/>, <see cref="IndexSpec"/>, <see cref="KeySpec"/>
/// </summary>
public class ValueExpr1Spec : SpecBase
{
    public List<ISpec> Parts { get; private set; } = new();
    public required Type ArgType { get; set; }

    public bool IsEmpty => Parts.Count == 0;

    public void AddProp(string prop)
    {
        Parts.Add(new PropSpec() { Name = prop, Raw = Raw });
    }

    public void AddIndex(string input)
    {
        if (int.TryParse(input, out var index))
            Parts.Add(new IndexSpec(index) { Raw = Raw });
        else
            Parts.Add(new KeySpec(input.Trim('"', '\'')) { Raw = Raw });
    }

    public override Expression BuildExpr(Expression expression, LambdaContext ctx)
    {
        var expr = expression.Type == ArgType ? expression : Expression.Convert(expression, ArgType);

        for (var i = 0; i < Parts.Count; i++)
            expr = Parts[i].BuildExpr(expr, ctx);

        if (ctx.Mode.HasFlag(SelectMode.Safe))
            expr = Expr.WrapInTryCatch(expr);

        return expr;
    }

    public override string ToString(string arg, SpecView view)
    {
        var str = view == SpecView.Expr ? $"(({ArgType.GetReadableName()}){arg})" : arg;

        for (var i = 0; i < Parts.Count; i++)
            str = Parts[i].ToString(str, view);

        return str;
    }

    public static ValueExpr1Spec Parse(string selectExpr, Type argType)
    {
        var expr = selectExpr.TrimStart('.');
        var startInd = 0;

        if (selectExpr.StartsWith("x."))
            startInd = 2;

        var spec = new ValueExpr1Spec() { Raw = expr, ArgType = argType };
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