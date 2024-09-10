using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using AVS.CoreLib.PowerConsole.Enums;
using AVS.CoreLib.PowerConsole.Extensions;

namespace AVS.CoreLib.PowerConsole.Printers2
{
    public class Printer2 : IPrinter2
    {
        protected PrinterOptions Options { get; set; }
        private IOutputWriter2 Writer { get; set; }

        public Printer2(TextWriter textWriter, PrinterOptions? options = null)
        {
            Writer = new OutputWriter2(textWriter);
            Options = options ?? PrinterOptions.Default;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(string message)
        {
            Writer.Write(message);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteLine(bool voidMultipleNewLines = true)
        {
            Writer.WriteLine(voidMultipleNewLines);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteLine(string message)
        {
            Writer.WriteLine(message);
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void WriteLine(MessageLevel level, string message)
        {
            Writer.WriteLine($"{level.GetText()}: {message}");
        }

        public virtual void Print(string message, PrintOptions2 options = PrintOptions2.Default)
        {
            var inline = options.HasFlag(PrintOptions2.Inline);
            var text = !inline || options.HasFlag(PrintOptions2.NoTimestamp) ? message : message.AddTimestamp(GetTime(), Options.TimeFormat);

            //as this is a printer without coloring options we assume message does not contain any color tags
            //thus no need to process tags

            if (inline)
            {
                Writer.Write(text);
            }
            else
            {
                Writer.WriteLine(text);
            }
        }

        public virtual void Print(FormattableString str, PrintOptions2 options = PrintOptions2.Default)
        {
            var message = FormatInternal(str);
            var text = options.HasFlag(PrintOptions2.NoTimestamp) ? message : message.AddTimestamp(GetTime(), Options.TimeFormat);
            var inline = options.HasFlag(PrintOptions2.Inline);

            //as this is a printer without coloring options we assume message does not contain any color tags
            //thus no need to process tags

            if (inline)
            {
                Writer.Write(text);
            }
            else
            {
                Writer.WriteLine(text);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual string FormatInternal(FormattableString str)
        {
            return Options.Format.Invoke(str);
        }

        protected virtual DateTime GetTime()
        {
            return DateTime.Now;
        }
    }

    internal static class TextExtensions
    {
        internal static string AddTimestamp(this string message, DateTime timestamp, string? timeFormat)
        {
            return string.IsNullOrEmpty(timeFormat) ? message : $"{timestamp.ToString(timeFormat)} {message}";
        }
    }
}