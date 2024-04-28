using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Text.Json.Serialization;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.Extensions
{
    public static class SystemExtensions
    {
        /// <summary>
        /// get stack trace in human-friendly format
        /// </summary>
        /// <param name="ex">exception</param>
        /// <param name="format">when true takes first few stack calls and ones that point to line numbers in the code</param>
        public static string GetStackTrace(this Exception ex, ReductionFormat format = ReductionFormat.Truncated)
        {
            if (ex.StackTrace == null)
                return string.Empty;

            var lines = ex.GetStackTraceLines(format);
            return string.Join(Environment.NewLine, lines);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] GetStackTraceLines(this Exception ex, ReductionFormat format = ReductionFormat.Truncated, int extraLines = 0)
        {
            if (ex.StackTrace == null)
                return Array.Empty<string>();

            var lines = ex.StackTrace.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            lines = StackTraceHelper.Reduce(lines, format, extraLines);

            var list = new List<string>(lines);
            list = StackTraceHelper.CutPaths(list);
            list = StackTraceHelper.CutNamespaces(list);
            return list.ToArray();

            //if (ex.InnerException?.StackTrace == null)
            //    return StackTraceHelper.Shorten(lines, format, extraLines);

            //var lines2 = ex.InnerException.StackTrace.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            //lines = StackTraceHelper.Reduce(lines, format, extraLines);
            //lines2 = StackTraceHelper.Reduce(lines2, format, extraLines);

            //var list = new List<string>(lines.Length + 2 + lines2.Length);
            //list.AddRange(lines);
            //list.Add("---------------------------------------------");
            //list.Add("InnerException: " + ex.InnerException.Message);
            //list.AddRange(lines2);
            //return list.ToArray();
        }

        public static ErrorDetails ToPlainObject(this Exception ex, bool addStackTrace = false)
        {
            var view = new ErrorDetails()
            {
                Message = ex.Message,
                Type = ex.GetType().Name,
                Source = ex.Source,
            };

            if(ex.Data is { Count: > 0 })
                view.Data = ex.Data;

            if (addStackTrace)
                view.StackTrace = ex.GetStackTraceLines();

            if (ex.InnerException != null)
            {
                view.InnerException = ToPlainObject(ex.InnerException, addStackTrace);
            }

            return view;
        }

        public static string ToString(this Exception ex, ReductionFormat format = ReductionFormat.Truncated)
        {
            var str = ex.ToString();
            
            if (format == ReductionFormat.None)
                return str;
            var n = ex.InnerException == null ? 1 : 2;

            var lines = str.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            lines = lines.Shorten(format, extraLines: n);
            return string.Join(Environment.NewLine, lines);
        }
    }

    public class ErrorDetails
    {
        public required string Message { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Type { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Source { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Data { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? StackTrace { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ErrorDetails? InnerException { get; set; }
    }

    internal static class StackTraceHelper
    {
        internal static string[] Reduce(this string[] lines, ReductionFormat format, int extraLines = 0)
        {
            if (format == ReductionFormat.None || lines.Length <= 8)
                return lines;

            var n = format == ReductionFormat.Truncated ? 4 : 7;
            var take = n + extraLines;

            if (lines.Length < take + 2)
                return lines;

            var list = new List<string>(lines.Length);
            // take first few lines and lines that point to source code, omit stack in between of those 
            list.AddRange(lines.Take(take));

            list.Add("...");
            list.AddRange(lines.Skip(take).Where(x => x.Contains(":line")));

            return list.ToArray();
        }

        internal static string[] Shorten(this string[] lines, ReductionFormat format, int extraLines = 0)
        {
            var list = new List<string>(Reduce(lines, format, extraLines));

            list = CutPaths(list);
            list = CutNamespaces(list);
            return list.ToArray();
        }

        internal static List<string> CutPaths(List<string> lines)
        {
            var refs = new List<string>();
            for (var k = 0; k < lines.Count; k++)
            {
                var line = lines[k];//.Trim();
                var ind = line.IndexOf(" in ", StringComparison.Ordinal);
                if (ind == -1)
                    continue;

                var path = line.Substring(ind + 4);
                var count = path.Count(x => x == '\\');

                if (count < 3)
                    continue;

                ind = path.LastIndexOf('\\');
                var ind2 = path.LastIndexOf('\\', ind - 1);

                path = path.Substring(0, ind2);

                if (!refs.Contains(path))
                    refs.Add(path);

                var refNumber = refs.IndexOf(path) + 1;

                var placeholder = $"[PATH_{refNumber}]";
                lines[k] = line.Replace(path, placeholder);
            }

            for (var i = 0; i < refs.Count; i++)
                lines.Add($"[PATH_{i + 1}] = {@refs[i]}");

            return lines;
        }

        internal static List<string> CutNamespaces(List<string> lines)
        {
            var refs = new List<string>();
            for (var k = 0; k < lines.Count; k++)
            {
                // replace common keywords
                lines[k] = lines[k].Replace("cancellationToken", "ct").Replace("context", "ctx");
                var line = lines[k];

                var ind = line.IndexOf("(", StringComparison.Ordinal);
                var atIndex = line.IndexOf("at ", StringComparison.Ordinal);
                if (ind == -1 || atIndex == -1 || atIndex+ind > line.Length)
                    continue;
                
                if(line.Length <= 100)
                    continue;

                var ns = line.Substring(atIndex+3, ind - 3);
                var count = ns.Count(x => x == '.');

                if (count < 3)
                    continue;

                ind = ns.LastIndexOf('.');
                var ind2 = ns.LastIndexOf('.', ind - 1);

                ns = ns.Substring(0, ind2);

                if (!refs.Contains(ns))
                    refs.Add(ns);

                var refNumber = refs.IndexOf(ns);

                var placeholder = $"[{refNumber}]";
                lines[k] = line.Replace(ns, placeholder);
            }

            for (var i = 0; i < refs.Count; i++)
            {
                lines.Add($"[{i}] = {@refs[i]}");
            }

            return lines;
        }
    }
}
