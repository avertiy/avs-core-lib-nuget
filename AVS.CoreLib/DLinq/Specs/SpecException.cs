using System;

namespace AVS.CoreLib.DLinq.Specs;

public class SpecException : Exception
{
    public string? Raw => Spec.Raw;
    public ISpec Spec { get; set; }

    public SpecException(string? message, ISpec spec) : base(message)
    {
        Spec = spec;
    }

    public SpecException(string? message, Exception? innerException, ISpec spec) : base(message, innerException)
    {
        Spec = spec;
    }
}