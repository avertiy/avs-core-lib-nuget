using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.DLinq.Specs;
using AVS.CoreLib.DLinq.Specs.CompoundBlocks;
using AVS.CoreLib.DLinq.Specs.LambdaSpecs;
using AVS.CoreLib.Guards;

namespace AVS.CoreLib.DLinq.Conditions;

public record MultiCondition : ICondition
{
    public List<ICondition> Items { get; set; }

    public Op Op { get; set; }

    public MultiCondition(Op op, params ICondition[] items)
    {
        Guard.Array.MinLength(items, minLength: 2);
        Op = op;
        Items = new(items);
    }

    public MultiCondition(Op op, params string[] parts)
    {
        Guard.Array.MinLength(parts, minLength: 2);
        Op = op;
        Items = new(parts.Select(x => new Condition(x)));
    }

    public override string ToString()
    {
        return $"({string.Join($" {Op} ", Items)})";
    }

    public ILambdaSpec GetSpec(DLinqContext context)
    {
        var items = Items.Select(x => x.GetSpec(context)).ToArray();
        return LogicalSpec.Combine(Op, items);
    }
}