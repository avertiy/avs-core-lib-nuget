using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AVS.CoreLib.Diagnostic;

public static class LogBuffer
{
    private static LogBufferImpl? _instance;
    private static LogBufferImpl Instance => _instance ??= new LogBufferImpl();

    public static void WriteLine(string message, string category = "info")
    {
        Instance.WriteLine(message, category);
    }

    public static IList<LogItem> Logs => Instance.Logs;

    public static IList<LogItem> GetLogs(string category)
    {
        return Instance.Logs.Where(x => x.Category == category).ToList();
    }

    public static new string ToString()
    {
        return Instance.ToString();
    }

    public static void Flush()
    {
        Instance.Flush();
    }

    public static void PrintToConsole()
    {
        Instance.PrintToConsole();
    }

    public static void Clear()
    {
        Instance.Clear();
    }
}

internal class LogBufferImpl
{
    public List<LogItem> Logs { get; private set; } = new();

    public int Count => Logs.Count;

    public void WriteLine(string message, string category = "info")
    {
        Logs.Add(new LogItem() { Message = message, Timestamp = DateTime.Now, Category = category });
    }

    public virtual void Flush()
    {
        var str = ToString();
        Debug.WriteLine(str);
        Clear();
    }

    public virtual void PrintToConsole()
    {
        var str = ToString();
        Console.WriteLine(str);
        Clear();
    }

    public void Clear(string? category = null)
    {
        if (category == null)
        {
            Logs.Clear();
            return;
        }

        Logs.RemoveAll(x => x.Category == category);
    }

    public override string ToString()
    {
        var sb = new System.Text.StringBuilder(Logs.Count * 100);

        foreach (var group in Logs.GroupBy(x => x.Category))
        {
            sb.AppendLine($" =============== {group.Key} (START) =================");

            foreach (var log in group)
                sb.AppendLine($"{log.Timestamp:G} {log.Message}");

            sb.AppendLine($" =============== {group.Key} (END) =================");
        }

        return sb.ToString();
    }
}

public class LogItem
{
    public DateTime Timestamp { get; set; }
    public required string Message { get; set; }
    public required string Category { get; set; }
}