using System;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Console.ColorFormatting.Extensions;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.PowerConsole.Enums;
using AVS.CoreLib.PowerConsole.FormatProcessors;
using AVS.CoreLib.PowerConsole.TagProcessors;
using AVS.CoreLib.PowerConsole.Utilities;
using AVS.CoreLib.PowerConsole.Writers;
using AVS.CoreLib.Text;
using AVS.CoreLib.Text.Formatters.ColorMarkup;
using AVS.CoreLib.Text.TextProcessors;

namespace AVS.CoreLib.PowerConsole.Printers
{
    public interface IBasicPrinter
    {
        void Print(string str, bool endLine);
        void Print(FormattableString str, bool endLine);
        void PrintLine(bool voidMultipleEmptyLines = true);
        void PrintLine(string str, bool voidMultipleEmptyLines);
    }

    public interface IColorPrinter
    {
        void SwitchMode(ColorMode mode);
        void Print(string str, bool endLine, bool? containsCTags);
        void Print(string str, bool endLine, ConsoleColor? color, bool? containsCTags = null);
        void Print(string str, bool endLine, Colors colors, bool? containsCTags = null);
        void Print(string str, bool endLine, ColorScheme scheme, bool? containsCTags = null);

        void Print(FormattableString str, bool endLine, bool? containsCTags = null);
        void Print(FormattableString str, bool endLine, ConsoleColor? color, bool? containsCTags = null);
        void Print(FormattableString str, bool endLine, Colors colors, bool? containsCTags = null);
        void Print(FormattableString str, bool endLine, ColorScheme scheme, bool? containsCTags = null);
    }

    /// <remarks>
    /// strings staring with @ symbol are treated as strings with expression(s) and processed by TextProcessor(s) this is part of X.Format(); 
    /// <see cref="TextExpressionProcessor"/>
    /// </remarks>
    public interface IXPrinter
    {
        void PrintF(FormattableString str, bool endLine, bool containsCTags = true);
        void PrintF(FormattableString str, bool endLine, ConsoleColor? color, bool containsCTags = true);
        void PrintF(FormattableString str, bool endLine, Colors colors, bool containsCTags = true);
        void PrintF(FormattableString str, bool endLine, ColorScheme scheme, bool containsCTags = true);
    }

    public interface IPowerConsolePrinter : IBasicPrinter, IColorPrinter, IXPrinter
    {
        void SetCustomFormatter(Func<FormattableString, string> formatter, bool printF = true);
        void Print(FormattableString str, ColorPalette palette, bool endLine);
        void Print(string str, CTag tag, bool endLine);
    }



    public interface IBasicPrinter2
    {
        void Print(string str, bool endLine);
        void PrintLine(bool voidMultipleEmptyLines = true);
        void PrintLine(string str, bool voidMultipleEmptyLines);
    }
}