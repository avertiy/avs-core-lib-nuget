using System;
using AVS.CoreLib.DLinq.LambdaSpec;

namespace AVS.CoreLib.DLinq;

public class DLinqException : Exception
{
    public string? Raw => Spec?.Raw;
    public Spec? Spec { get; set; }
    public DLinqException(string? message) : base(message)
    {
    }

    public DLinqException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}