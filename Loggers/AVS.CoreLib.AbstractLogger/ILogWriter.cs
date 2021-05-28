using System;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.AbstractLogger
{
    public interface ILogWriter
    {
        void WriteLine(bool combineEmptyLines = true);
        void Write(string str, bool endLine = true);
        void Write(string logger, EventId eventId, LogLevel logLevel, string message, Exception exception = null);
    }
}