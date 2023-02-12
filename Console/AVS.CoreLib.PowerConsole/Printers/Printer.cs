using System;
using System.Globalization;
using System.IO;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.PowerConsole.Enums;
using AVS.CoreLib.PowerConsole.Utilities;
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
            var formattedStr = FormatInternal(str);
            Writer.Write(formattedStr, options);
        }

        public void Print(FormattableString str, MultiColorPrintOptions options)
        {
            var formattedStr = FormatInternal(str);
            throw new NotImplementedException("");
            //var arguments = str.GetArguments();
            //var str2 = new FormattableString2(str.Format, arguments);
            //var preprocessor = FormatPreprocessor;

            //if (preprocessor is ColorFormatPreprocessor colorFormatPreprocessor)
            //    colorFormatPreprocessor.Palette = palette;

            //var formattedString = str2.ToString(X.FormatProvider, printer.FormatPreprocessor, X.TextProcessor);
            //var colorMarkupStr = new ColorMarkupString(formattedString);

            //printer.Print(colorMarkupStr, endLine);
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