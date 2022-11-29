using System;
using System.IO;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole.Printers
{
    public interface IPrinter
    {
        Func<FormattableString, string> Format { get; set; }
        Func<FormattableString, string> XFormat { get; set; }
        TagProcessor TagProcessor { get; }
        IColorFormatProcessor FormatProcessor { get; }
        void Print(string str, bool endLine);
        void Print(string str, ConsoleColor? color, bool endLine);
        void WriteLine(bool voidMultipleEmptyLines = true);
    }
}