using System;
using System.Collections.Generic;
using System.Diagnostics;
using AVS.CoreLib.PowerConsole.Extensions;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole
{
    public static partial class PowerConsole
    {
        public static void Print(string str, ConsoleColor color, bool endLine = true)
        {
            Write(str, color);
            if (endLine && NewLineFlag == false)
            {
                Console.WriteLine();
                NewLineFlag = true;
            }
        }

        public static void Print(string str, ColorScheme scheme, bool endLine = true)
        {
            scheme.Apply();
            Write(str);
            if (endLine && NewLineFlag == false)
            {
                Console.WriteLine();
                NewLineFlag = true;
            }
            scheme.Restore();
        }

        public static void Print(string message, Array arr, ConsoleColor color = ConsoleColor.Gray)
        {
            Print($"{message} {arr.ToArrayString()}", color);
        }

        

        public static void PrintAllColors()
        {
            var values = Enum.GetNames(typeof(ConsoleColor));

            foreach (string colorName in values)
            {
                var color = Enum.Parse<ConsoleColor>(colorName);
                Print(colorName, color);
            }
        }

        public static void PrintHeader(string header, ConsoleColor color = ConsoleColor.Cyan, string template = "==========", string lineIdentation = "\r\n\r\n")
        {
            var str = $"{lineIdentation}{template} {header} {template}{lineIdentation}";
            Print(str, color, false);
        }

        public static void PrintTimeElapsed(DateTime @from, string comment)
        {
            var ms = (DateTime.Now - @from).TotalMilliseconds;
            if (ms < 0.5)
                return;

            Print($"{comment} => elapsed:{ms:N3} ms", ConsoleColor.Green);
        }

        public static void PrintTable<T>(IEnumerable<T> data, ConsoleColor color = ConsoleColor.White, bool endLine = true)
        {
            Print(Table.Create(data).ToString(), color, endLine);
        }
    }
}
