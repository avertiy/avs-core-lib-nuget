﻿using System;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.AbstractLogger
{
    public interface ILogWriter
    {
        void WriteLine(bool voidEmptyLines = true);
        void Write(string str, bool endLine = true);
        void Write(string logger, EventId eventId, LogLevel logLevel, string message, Exception exception = null);
        void BeginScope(object scope, bool addCurlyBraces);
        void EndScope(bool addCurlyBraces);
    }
}