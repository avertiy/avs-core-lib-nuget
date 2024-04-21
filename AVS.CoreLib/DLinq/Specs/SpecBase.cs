using System;

namespace AVS.CoreLib.DLinq.Specs;

public abstract class SpecBase
{
    public string? Raw { get; set; }

    public override string ToString()
    {
        return $"{GetType().Name} (raw:{Raw})";
    }
}