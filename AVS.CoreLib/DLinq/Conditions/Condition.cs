using System;
using AVS.CoreLib.DLinq.Specifications;
using AVS.CoreLib.DLinq.Specifications.Conditioning;

namespace AVS.CoreLib.DLinq.Conditions;


public interface ICondition
{
    ISpec GetSpec(Type type);
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

    public ISpec GetSpec(Type type)
    {
        return ConditionSpec.Parse(Expr, type);
    }
}

