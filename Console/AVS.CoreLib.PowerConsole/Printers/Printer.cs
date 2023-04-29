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
        protected IOutputWriter Writer { get; set; }
        protected Func<FormattableString, string> Format { get; set; } = str => str.ToString(CultureInfo.CurrentCulture);

        private readonly TextWriter _textWriter;
        public ColorMode ColorMode { get; private set; }

        public Printer(TextWriter textWriter, ColorMode mode)
        {
            Writer = OutputWriter.Create(textWriter, mode);
            ColorMode = mode;
            _textWriter = textWriter;
        }

        #region Print methods

        public void Print(string str, PrintOptions options)
        {
            Writer.Write(str, options);
        }

        #endregion

        #region Print(FormattableString) methods

        public void Print(FormattableString str, PrintOptions options)
        {
            string formattedStr;

            if (options.ColorPalette != null)
            {
                var str2 = str.Colorize(options.ColorPalette.Colors);
                formattedStr = FormatInternal(str2);
            }
            else
            {
                formattedStr = FormatInternal(str);
            }

            Writer.Write(formattedStr, options);
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

        public void Write(string str, PrintOptions options)
        {
            Writer.Write(str, options);
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