using System;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.PowerConsole.Enums;
using AVS.CoreLib.PowerConsole.Utilities;
using AVS.CoreLib.Text.TextProcessors;

namespace AVS.CoreLib.PowerConsole.Printers
{
    public interface IPrinter
    {
        void Print(string str, bool endLine = true);
        void Print(string str, bool endLine, bool? containsCTags);
        void Print(string str, bool endLine, bool? containsCTags, ConsoleColor? color);
        void Print(string str, bool endLine, bool? containsCTags, CTag tag);
        void Print(string str, bool endLine, bool? containsCTags, ColorScheme scheme);
        void Print(string str, bool endLine, bool? containsCTags, Colors colors);

        void Print(FormattableString str, bool endLine = true);
        void Print(FormattableString str, bool endLine, bool? containsCTags);
        void Print(FormattableString str, bool endLine,  bool? containsCTags, ConsoleColor? color);
        void Print(FormattableString str, bool endLine, bool? containsCTags, Colors colors);
        void Print(FormattableString str, bool endLine, bool? containsCTags, ColorScheme scheme);

        void WriteLine(string? str = null, bool voidMultipleEmptyLines = true);
        void SwitchMode(ColorMode mode);
    }

    /// <remarks>
    /// strings staring with @ symbol are treated as strings with expression(s) and processed by TextProcessor(s) this is part of X.Format(); 
    /// <see cref="TextExpressionProcessor"/>
    /// </remarks>
    public interface IXPrinter : IPrinter
    {
        void PrintF(FormattableString str, bool endLine = true);
        void PrintF(FormattableString str, bool endLine, bool? containsCTags);
        void PrintF(FormattableString str, bool endLine, bool? containsCTags, ConsoleColor? color);
        void PrintF(FormattableString str, bool endLine, bool? containsCTags, Colors colors);
        void PrintF(FormattableString str, bool endLine, bool? containsCTags, ColorScheme scheme);

        void SetCustomFormatter(Func<FormattableString, string> formatter, bool printF = true);
    }
}