using System;
using System.Collections.Generic;
using AVS.CoreLib.DLinq.Specs;
using AVS.CoreLib.DLinq.Specs.CompoundBlocks;
using AVS.CoreLib.DLinq.Specs.Conditioning;

namespace AVS.CoreLib.DLinq.Conditions;


public interface ICondition
{
    ISpec GetSpec(Type type, Dictionary<string, ValueExprSpec> specs);
}

public partial class Condition : ICondition
{
    public string Expr { get; set; }

    public static Condition Empty { get; } = new(string.Empty);

    public Condition(string expr)
    {
        Expr = expr.Trim();
    }

    public override string ToString()
    {
        return Expr;
    }

    public ISpec GetSpec(Type type, Dictionary<string, ValueExprSpec> specs)
    {
        return ConditionSpec.Parse(Expr, type, specs);
    }
}

