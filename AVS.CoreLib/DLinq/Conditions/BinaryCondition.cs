using System;
using System.Collections.Generic;
using AVS.CoreLib.DLinq.Specs;
using AVS.CoreLib.DLinq.Specs.CompoundBlocks;

namespace AVS.CoreLib.DLinq.Conditions;

public record BinaryCondition : ICondition
{
    public ICondition Left { get; set; } = null!;
    public Op Op { get; set; }
    public ICondition Right { get; set; } = null!;

    public BinaryCondition(string left, Op @operator, string right)
    {
        Left = new Condition(left);
        Op = @operator;
        Right = new Condition(right);
    }

    public BinaryCondition(ICondition left, Op @operator, ICondition right)
    {
        Left = left;
        Op = @operator;
        Right = right;
    }

    public override string ToString()
    {
        return $"({Left} {Op} {Right})";
    }

    public ISpec GetSpec(Type type, Dictionary<string, ValueExprSpec> specs)
    {
        var leftSpec = Left.GetSpec(type, specs);
        var rightSpec = Right.GetSpec(type, specs);
        return SpecBase.Combine(Op, leftSpec, rightSpec);
    }

    public static ICondition OR(ICondition left, ICondition right)
    {
        return new BinaryCondition(left, Op.OR, right);
    }

    public static ICondition AND(ICondition left, ICondition right)
    {
        return new BinaryCondition(left, Op.AND, right);
    }
}