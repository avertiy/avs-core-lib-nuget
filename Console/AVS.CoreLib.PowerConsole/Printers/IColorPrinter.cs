using System;
using System.IO;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.PowerConsole.Enums;
using AVS.CoreLib.PowerConsole.Extensions;
using AVS.CoreLib.PowerConsole.TagProcessors;
using AVS.CoreLib.PowerConsole.Utilities;
using AVS.CoreLib.PowerConsole.Writers;

namespace AVS.CoreLib.PowerConsole.Printers
{
    public class ColorPrinter: BasicPrinter<IOutputWriter>, IColorPrinter
    {
        private readonly TextWriter _textWriter;
        public ColorMode ColorMode { get; private set; }

        public ColorPrinter(TextWriter textWriter, ColorMode mode) : base(OutputWriter.Create(textWriter, mode))
        {
            _textWriter = textWriter;
        }

        public void SwitchMode(ColorMode mode)
        {
            Writer = OutputWriter.Create(_textWriter, mode);
            ColorMode = mode;
        }

        #region Print methods

        public void Print(string str, bool endLine, bool? containsCTags)
        {
            Writer.Write(str, endLine, containsCTags);
        }

        public virtual void Print(string str, bool endLine, ConsoleColor? color, bool? containsCTags = null)
        {
            if (color.HasValue)
                Writer.Write(str, endLine, containsCTags, color.Value);
            else
                Writer.Write(str, endLine, containsCTags);
        }

        public virtual void Print(string str, bool endLine, Colors colors, bool? containsCTags = null)
        {
            Writer.Write(str, endLine, containsCTags, colors);
        }

        public virtual void Print(string str, bool endLine, ColorScheme scheme, bool? containsCTags = null)
        {
            Writer.Write(str, endLine, containsCTags, scheme);
        }

        #endregion

        #region Print FormattableString methods

        public void Print(FormattableString str, bool endLine, bool? containsCTags = null)
        {
            var text = FormatInternal(str);
            Writer.Write(text, endLine, containsCTags);
        }

        public virtual void Print(FormattableString str, bool endLine, ConsoleColor? color, bool? containsCTags = null)
        {
            var text = FormatInternal(str);
            if (color.HasValue)
                Writer.Write(text, endLine, containsCTags, color.Value);
            else             
                Writer.Write(text, endLine, containsCTags);
        }

        public void Print(FormattableString str, bool endLine, Colors colors, bool? containsCTags = null)
        {
            var text = FormatInternal(str);
            Writer.Write(text, endLine, containsCTags, colors);
        }

        public void Print(FormattableString str, bool endLine, ColorScheme scheme, bool? containsCTags = null)
        {
            var text = FormatInternal(str);
            Writer.Write(text, endLine, containsCTags, scheme);
        }
        #endregion
    }
}