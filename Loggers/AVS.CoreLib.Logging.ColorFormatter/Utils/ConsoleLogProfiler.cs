﻿using System.Text;

namespace AVS.CoreLib.Logging.ColorFormatter.Utils;

/// <summary>
/// utility class for profiling console logging i.e. keep printed on console logs in buffer
/// useful for debugging or saving printed logs as text
/// </summary>
public static class ConsoleLogProfiler
{
    public static bool Enabled { get; set; }
    private static readonly StringBuilder _sb = new StringBuilder();

    public static void Write(string message)
    {
        if (Enabled)
        {
            _sb.AppendLine(message);
            if (_sb.Length > 5000)
                _sb.Remove(0, 1000);
        }
    }

    public static string Flush(bool clear = true)
    {
        var log = _sb.ToString();
        if (clear)
            _sb.Length = 0;
        return log;
    }

}