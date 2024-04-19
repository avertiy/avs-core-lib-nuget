using System;

namespace AVS.CoreLib.DLinq.Specs;

public class SpecException : Exception
{
    public string? Raw => Spec.Raw;
    public SpecBase Spec { get; set; }

    public SpecException(string? message, SpecBase spec) : base(message)
    {
        Spec = spec;
    }

    public SpecException(string? message, Exception? innerException, SpecBase spec) : base(message, innerException)
    {
        Spec = spec;
    }
}