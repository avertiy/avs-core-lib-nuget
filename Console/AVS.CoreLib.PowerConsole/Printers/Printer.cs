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
        public void Print(string str, bool endLine = true)
        {
            Writer.Write(str, endLine);
        }

        public void Print(string str, bool endLine, bool? containsCTags)
        {
            Writer.Write(str, endLine, containsCTags);
        }

        public void Print(string str, bool endLine, bool? containsCTags, ConsoleColor? color)
        {
            Writer.Write(str, endLine, containsCTags, color);
        }

        public void Print(string str, bool endLine, bool? containsCTags, CTag tag)
        {
            Writer.Write(str, endLine, containsCTags, tag);
        }

        public void Print(string str, bool endLine, bool? containsCTags, ColorScheme scheme)
        {
            Writer.Write(str, endLine, containsCTags, scheme);
        }

        public void Print(string str, bool endLine, bool? containsCTags, Colors colors)
        {
            Writer.Write(str, endLine, containsCTags, colors);
        } 
        #endregion

        #region Print(FormattableString) methods
        public void Print(FormattableString str, bool endLine)
        {
            var formattedStr = FormatInternal(str);
            Writer.Write(formattedStr, endLine);
        }

        public void Print(FormattableString str, bool endLine, bool? containsCTags)
        {
            var formattedStr = FormatInternal(str);
            Writer.Write(formattedStr, endLine, containsCTags);
        }

        public void Print(FormattableString str, bool endLine, bool? containsCTags, ConsoleColor? color)
        {
            var formattedStr = FormatInternal(str);
            Writer.Write(formattedStr, endLine, containsCTags, color);
        }

        public void Print(FormattableString str, bool endLine, bool? containsCTags, Colors colors)
        {
            var formattedStr = FormatInternal(str);
            Writer.Write(formattedStr, endLine, containsCTags, colors);
        }

        public void Print(FormattableString str, bool endLine, bool? containsCTags, CTag tag)
        {
            var formattedStr = FormatInternal(str);
            Writer.Write(formattedStr, endLine, containsCTags, tag);
        }

        public void Print(FormattableString str, bool endLine, bool? containsCTags, ColorScheme scheme)
        {
            var formattedStr = FormatInternal(str);
            Writer.Write(formattedStr, endLine, containsCTags, scheme);
        } 
        #endregion

        public void WriteLine(string? str = null, bool voidMultipleEmptyLines = true)
        {
            Writer.WriteLine(str, voidMultipleEmptyLines);
        }

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