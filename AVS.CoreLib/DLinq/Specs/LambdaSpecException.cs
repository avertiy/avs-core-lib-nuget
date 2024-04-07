using System;

namespace AVS.CoreLib.DLinq.Specs;

public class LambdaSpecException : Exception
{
    public string? Raw => Spec.Raw;
    public ISpec Spec { get; set; }

    public LambdaSpecException(string? message, ISpec spec) : base(message)
    {
        Spec = spec;
    }

    public LambdaSpecException(string? message, Exception? innerException, ISpec spec) : base(message, innerException)
    {
        Spec = spec;
    }
}