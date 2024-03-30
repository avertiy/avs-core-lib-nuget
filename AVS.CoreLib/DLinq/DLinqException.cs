using System;

namespace AVS.CoreLib.DLinq;

public class DLinqException : Exception
{
    public DLinqException(string? message) : base(message)
    {
    }

    public DLinqException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}