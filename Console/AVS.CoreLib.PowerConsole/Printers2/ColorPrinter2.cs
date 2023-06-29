using System;
using System.IO;
using System.Runtime.CompilerServices;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.PowerConsole.Enums;
using AVS.CoreLib.PowerConsole.Extensions;

namespace AVS.CoreLib.PowerConsole.Printers2
{
    public class ColorPrinter2 : Printer2, IColorPrinter2
    {
        private readonly TextWriter _textWriter;
        private IColorOutputWriter Writer { get; set; }
        public ColorMode Mode { get; private set; } = ColorMode.Default;

        public ColorPrinter2(TextWriter textWriter, PrinterOptions? options = null) : base(textWriter, options)
        {
            _textWriter = textWriter;
            Writer = CreateWriter(textWriter, Mode);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(string message, Colors colors)
        {
            Writer.Write(message, colors);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteLine(string message, Colors? colors = null)
        {
            Writer.WriteLine(message, colors);
        }

        public override void WriteLine(MessageLevel level, string message)
        {
            var text = level.GetText();
            if(!string.IsNullOrEmpty(text))
                Writer.Write(level.GetText()+": ", level.GetColors());

            Writer.WriteLine(message);
        }

        public override void Print(string message, PrintOptions2 options = PrintOptions2.Default)
        {
            this.Print(message, options, null);
        }

        public void Print(string message, PrintOptions2 options, Colors? colors)
        {
            if (!options.HasFlag(PrintOptions2.NoTimestamp))
            {
                WriteTimestamp();
            }
            var endLine = !options.HasFlag(PrintOptions2.Inline);

            if (options.HasFlag(PrintOptions2.NoCTags))
            {
                Writer.Write(message, endLine, colors);
            }
            else
            {
                Writer.WriteWithCTags(message, endLine, colors);
            }
        }

        public void Print(MessageLevel level, string message, PrintOptions2 options = PrintOptions2.Default, Colors? colors = null)
        {
            if (!options.HasFlag(PrintOptions2.NoTimestamp))
            {
                WriteTimestamp();
            }

            var text = level.GetText();
            if (!string.IsNullOrEmpty(text))
                Writer.Write(level.GetText() + ": ", level.GetColors());

            var endLine = !options.HasFlag(PrintOptions2.Inline);

            if (options.HasFlag(PrintOptions2.NoCTags))
            {
                Writer.Write(message, endLine, colors);
            }
            else
            {
                Writer.WriteWithCTags(message, endLine, colors);
            }
        }

        public override void Print(FormattableString str, PrintOptions2 options = PrintOptions2.Default)
        {
            this.Print(str, options, null);
        }

        public void Print(FormattableString str, PrintOptions2 options, Colors? colors)
        {
            var message = FormatInternal(str);
            this.Print(message, options, colors);
        }

        public void SwitchMode(ColorMode mode)
        {
            Writer = CreateWriter(_textWriter, mode);
            Mode = mode;
        }

        protected void WriteTimestamp()
        {
            if (Options.TimeFormat == null)
                return;

            var colors = new Colors(ConsoleColor.DarkYellow, null);
            Writer.Write($"{GetTime().ToString(Options.TimeFormat)} ", colors);
        }

        private static IColorOutputWriter CreateWriter(TextWriter textWriter, ColorMode mode)
        {
            switch (mode)
            {
                case ColorMode.Default:
                case ColorMode.AnsiCodes:
                {
                    return new ColorOutputWriter(textWriter);
                }
                default:
                    throw new NotImplementedException("other modes not supported at the moment");
            }
        }
    }

    public class PowerConsolePrinter2 : ColorPrinter2, IPowerConsolePrinter2
    {
        public PowerConsolePrinter2(TextWriter textWriter, PrinterOptions? options = null) : base(textWriter, options)
        {
        }
    }
}