using System;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole.ConsoleWriters
{
    public class ConsoleWriter : ConsoleWriterBase, IConsoleWriter
    {
        public void Write(string message, ConsoleColor color, bool endLine)
        {
            var str = $"{AnsiCodes.Color(color)}{message}{AnsiCodes.RESET}";
            base.Write(str, endLine);
        }

        public void Write(string message, Colors colors, bool endLine)
        {
            var str = colors.Colorize(message);
            base.Write(str, endLine);
        }

        public void Write(string message, ColorScheme scheme, bool endLine)
        {
            var str = scheme.Colorize(message);
            base.Write(str, endLine);
        }
    }
}