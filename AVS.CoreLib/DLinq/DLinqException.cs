using System;
using AVS.CoreLib.DLinq.LambdaSpec;
using AVS.CoreLib.DLinq.Specifications;

namespace AVS.CoreLib.DLinq;

public class DLinqException : Exception
{
    public string? Raw { get; }
    public ISpec? Spec { get; }
    public DLinqException(string? message) : base(message)
    {
    }

    public DLinqException(string? message, string raw) : base(message)
    {
        Raw = raw;
    }

    public DLinqException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public DLinqException(string? message, Exception? innerException, ISpec spec) : base(message, innerException)
    {
        Spec = spec;
        Raw = spec?.Raw;
    }
}

public class InvalidExpression : DLinqException
{
    public InvalidExpression(string? message) : base(message)
    {
    }

    public InvalidExpression(string? message, string raw) : base(message, raw)
    {
    }

    public InvalidExpression(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
