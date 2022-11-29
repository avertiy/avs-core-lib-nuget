using System;
using AVS.CoreLib.Console.ColorFormatting;

namespace AVS.CoreLib.PowerConsole.Writers
{
    public class SimpleConsoleWriter : IWriter
    {
        public virtual void Write(string str, bool endLine)
        {
            System.Console.Write(str);
            if (endLine)
                System.Console.WriteLine();
        }

        public virtual void Write(string str, ConsoleColor color, bool endLine)
        {
            var coloredStr = $"{AnsiCodes.Color(color)}{str}{AnsiCodes.RESET}";
            System.Console.Write(coloredStr);
            if (endLine)
                System.Console.WriteLine();
        }

        public virtual void WriteLine(bool voidMultipleEmptyLines = true)
        {
            System.Console.WriteLine();
        }
    }
}