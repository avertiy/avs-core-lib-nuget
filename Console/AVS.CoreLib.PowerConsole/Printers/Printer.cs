using System;
using System.Globalization;
using System.IO;
using AVS.CoreLib.PowerConsole.Enums;
using AVS.CoreLib.PowerConsole.Extensions;
using AVS.CoreLib.PowerConsole.Writers;

namespace AVS.CoreLib.PowerConsole.Printers
{
    public class Printer : IPrinter
    {
        public ColorMode ColorMode { get; private set; }
        public virtual DateTime SystemTime => DateTime.Now;
        public string TimeFormat { get; set; } = "HH:mm:ss";

        protected IOutputWriter Writer { get; set; }
        protected Func<FormattableString, string> Format { get; set; } = str => str.ToString(CultureInfo.CurrentCulture);

        private readonly TextWriter _textWriter;
        
        public Printer(TextWriter textWriter, ColorMode mode)
        {
            Writer = OutputWriter.Create(textWriter, mode);
            ColorMode = mode;
            _textWriter = textWriter;
        }

        #region Print methods

        public void Print(string message, PrintOptions options)
        {
            var text = options.TimeFormat == null ? message : this.AddTimestamp(message, SystemTime, options.TimeFormat);
            Writer.Write(text, options);
        }

        public void Print(string message, PrintOptions2 options)
        {
            var text = options.HasFlag(PrintOptions2.NoTimestamp) ? message : this.AddTimestamp(message, SystemTime, TimeFormat);
            var endLine = !options.HasFlag(PrintOptions2.Inline);
            var colorTags = !options.HasFlag(PrintOptions2.NoCTags);
            Writer.Write(text, endLine, colorTags);
        }

        #endregion

        #region Print(FormattableString) methods

        public void Print(FormattableString str, PrintOptions options)
        {
            string message;

            if (options.ColorPalette != null)
            {
                var str2 = str.Colorize(options.ColorPalette.Colors);
                message = FormatInternal(str2);
            }
            else
            {
                message = FormatInternal(str);
            }

            var text = options.TimeFormat == null ? message : this.AddTimestamp(message, SystemTime, options.TimeFormat);
            Writer.Write(text, options);
        }

        #endregion

        #region Write & WriteLine methods
        public void Write(string str, bool endLine = false)
        {
            Writer.Write(str, endLine);
        }

        public void Write(string str, ConsoleColor color, bool endLine = false)
        {
            Writer.Write(str, new PrintOptions() { EndLine = endLine, Color = color });
        }

        public void Write(string message, PrintOptions options)
        {
            var text = options.TimeFormat == null ? message : this.AddTimestamp(message, SystemTime, options.TimeFormat);
            Writer.Write(text, options);
        }

        public void WriteLine(bool voidMultipleEmptyLines = true)
        {
            Writer.WriteLine(voidMultipleEmptyLines);
        }

        public void WriteLine(string? str, PrintOptions options)
        {
            Writer.WriteLine(str, options);
        } 
        #endregion

        public void SwitchMode(ColorMode mode)
        {
            Writer = OutputWriter.Create(_textWriter, mode);
            ColorMode = mode;
        }

        protected string FormatInternal(FormattableString str)
        {
            return Format?.Invoke(str) ?? str.ToString(CultureInfo.CurrentCulture);
        }
    }
}