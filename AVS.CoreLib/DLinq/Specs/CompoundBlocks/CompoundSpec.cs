using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AVS.CoreLib.DLinq.Specs.CompoundBlocks;

public abstract class CompoundSpec<T> : SpecBase where T : ILambdaSpec
{
    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public List<T> Items { get; set; }

    public int Count => Items.Count;

    protected CompoundSpec(int capacity)
    {
        Items = new(capacity);
    }

    protected CompoundSpec(IEnumerable<T> items)
    {
        Items = new(items);
    }

    public bool SameArgType(out Type? type)
    {
        type = null;
        foreach (var argType in IterateArgTypes())
        {
            if (type == null)
            {
                type = argType;
                continue;
            }

            if (argType != type)
                return false;
        }

        return true;
    }

    protected abstract IEnumerable<Type> IterateArgTypes();
}