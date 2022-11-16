using System;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole.ConsoleWriters
{
    public interface IConsoleWriter
    {
        void Write(string str, bool endLine);
        void Write(string str, ConsoleColor color, bool endLine);
        void Write(string message, Colors colors, bool endLine);
        void Write(string message, ColorScheme scheme, bool endLine);
        void WriteLine(bool voidMultipleEmptyLines = true);
    }
}