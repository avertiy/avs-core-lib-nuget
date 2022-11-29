using System;
using AVS.CoreLib.Console.ColorFormatting;

namespace AVS.CoreLib.PowerConsole.Writers
{
    public class DefaultConsoleWriter : IWriter
    {
        /// <summary>
        /// Indicates whether new line (\r\n) has been just written 
        /// </summary>
        public bool NewLineFlag = true;

        public virtual void Write(string str, bool endLine)
        {
            System.Console.Write(str);
            if (endLine && !NewLineFlag)
            {
                System.Console.WriteLine();
                NewLineFlag = true;
            }
            else
                NewLineFlag = str.EndsWith(Environment.NewLine);
        }

        public virtual void WriteLine(bool voidMultipleEmptyLines = true)
        {
            if (voidMultipleEmptyLines && NewLineFlag)
                return;

            System.Console.WriteLine();
            NewLineFlag = true;
        }

        public virtual void Write(string str, ConsoleColor color, bool endLine)
        {
            var coloredStr = $"{AnsiCodes.Color(color)}{str}{AnsiCodes.RESET}";
            System.Console.Write(coloredStr);
            if (endLine && !NewLineFlag)
            {
                System.Console.WriteLine();
                NewLineFlag = true;
            }
            else
                NewLineFlag = str.EndsWith(Environment.NewLine);
        }
    }
}