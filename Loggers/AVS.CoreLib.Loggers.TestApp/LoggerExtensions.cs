#nullable enable
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.Loggers.TestApp;

public static class LoggerExtensions
{
    public static IDisposable? Context(this ILogger logger, params (string Key, object Value)[] context)
    {
        var state = new Dictionary<string, object>(context.Length);

        foreach (var x in context)
        {
            state.Add(x.Key, x.Value);
        }

        return logger.BeginScope(state);
    }
}