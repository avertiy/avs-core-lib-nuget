using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

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
            var lines = ex.StackTrace?.Split(Environment.NewLine);
            if (lines == null)
                return string.Empty;

            lines = ex.GetStackTraceLines(format);
            return string.Join(Environment.NewLine, lines);
        }

        public static string ToString(this Exception ex, ReductionFormat format = ReductionFormat.Truncated)
        {
            var str = ex.ToString();
            var n = ex.InnerException == null ? 1 : 2;
            var lines = str.SplitAndShorten(format, extraLines: n);
            return string.Join(Environment.NewLine, lines);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string[] GetStackTraceLines(this Exception ex, ReductionFormat format = ReductionFormat.Truncated)
        {
            return ex.StackTrace.SplitAndShorten(format);
        }
    }


    internal static class StackTraceHelper
    {
        internal static string[] SplitAndShorten(this string? text, ReductionFormat format, int extraLines = 0)
        {
            var lines = text?.Split(Environment.NewLine);
            if (lines == null || lines.Length == 0)
                return Array.Empty<string>();

            return Shorten(lines, format, extraLines);
        }

        internal static string[] Shorten(this string[] lines, ReductionFormat format, int extraLines = 0)
        {
            if (lines.Length < 1)
                return lines;

            var list = new List<string>(lines.Length);
            // take first few lines and lines that point to source code, omit stack in between of those 
            if (lines.Length > 6 && format != ReductionFormat.None)
            {
                var n = format == ReductionFormat.Truncated ? 3 : 5;
                list.AddRange(lines.Take(n + extraLines));
                list.Add("...");
                list.AddRange(lines.Where(x => x.Contains(":line")));
            }
            else
            {
                list.AddRange(lines);
            }

            list = StackTraceHelper.CutPaths(list);
            list = StackTraceHelper.CutNamespaces(list);
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
            {
                lines.Add($"[PATH_{i + 1}] = {@refs[i]}");
            }

            return lines;
        }

        internal static List<string> CutNamespaces(List<string> lines)
        {
            var refs = new List<string>();
            for (var k = 0; k < lines.Count; k++)
            {
                var line = lines[k];//.Trim();
                var ind = line.IndexOf("(", StringComparison.Ordinal);
                var atIndex = line.IndexOf("at ", StringComparison.Ordinal);
                if (ind == -1 || atIndex == -1)
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
