using System;
using System.Collections.Generic;
using AVS.CoreLib.PowerConsole.ConsoleTable;
using AVS.CoreLib.PowerConsole.Extensions;
using AVS.CoreLib.PowerConsole.Structs;
using AVS.CoreLib.PowerConsole.Utilities;
using AVS.CoreLib.Text.Extensions;

namespace AVS.CoreLib.PowerConsole
{

    /// <summary>
    /// PowerConsole represents simple extensions over standard .NET Console functionality
    /// If you need more rich & extensive console frameworks check out links below  
    /// </summary>
    /// <seealso>https://github.com/Athari/CsConsoleFormat - advanced formatting of console output for .NET</seealso>
    /// <seealso>https://github.com/migueldeicaza/gui.cs - Terminal GUI toolkit for .NET</seealso>
    /// <seealso>http://elw00d.github.io/consoleframework/- cross-platform toolkit that allows to develop TUI applications using C# and based on WPF-like concepts</seealso>
    public static partial class PowerConsole
    {
        public static void Print(string str, bool endLine = true)
        {
            Write(str);
            WriteEndLine(endLine);
        }

        public static void Print(string str, ConsoleColor color, bool endLine = true)
        {
            Write(str, color);
            WriteEndLine(endLine);
        }

        public static void Print(string str, ColorScheme scheme, bool endLine = true)
        {
            ApplyColorScheme(scheme);
            Write(str);
            WriteEndLine(endLine);
            ColorSchemeReset();
        }

        public static void Print<T>(string message, IEnumerable<T> arr, ConsoleColor color = ConsoleColor.Gray)
        {
            Print($"{message} {arr.ToArrayString()}", color);
        }

        public static void PrintAllColors()
        {
            var values = Enum.GetNames(typeof(ConsoleColor));
            foreach (var colorName in values)
            {
                var color = Enum.Parse<ConsoleColor>(colorName);
                Print(colorName, color);
            }
        }

        public static void PrintArray<T>(IEnumerable<T> enumerable, Func<T, string> formatter = null, bool endLine = true)
        {
            Write(enumerable.ToArrayString(formatter));
            if (endLine && NewLineFlag == false)
            {
                Console.WriteLine();
                NewLineFlag = true;
            }
        }

        public static void PrintKeyValue<TKey,TValue>(IDictionary<TKey, TValue> dictionary, ConsoleColor color = ConsoleColor.White, string keyValueSeparator = " => ", string separator = "\r\n", bool endLine = true)
        {
            Write(dictionary.ToKeyValueString(keyValueSeparator, separator), color);
            WriteEndLine(endLine);
        }

        public static void PrintJson<T>(T obj, bool indented = false, ConsoleColor color = ConsoleColor.White, bool endLine = true)
        {
            Write(obj.ToJsonString(indented), color);
            WriteEndLine(endLine);
        }

        public static void PrintHeader(string header, ConsoleColor color = ConsoleColor.Cyan, string template = "============", string lineIndentation = "\r\n\r\n")
        {
            WriteLine();
            var str = $"{template} {header} {template}{lineIndentation}";
            Print(str, color, false);
        }

        public static void PrintTable<T>(IEnumerable<T> data, ConsoleColor color = ConsoleColor.White, bool endLine = true)
        {
            Print(Table.Create(data).ToString(), color, endLine);
        }

        public static void PrintTable<T>(IEnumerable<T> data, Action<Table> configure = null, bool endLine = true)
        {
            var table = Table.Create(data);
            var colorFormattedString = table.ToColorFormattedString();
            PrintF(colorFormattedString, endLine);
        }

        public static void PrintTable(Table table, bool endLine = true)
        {
            var colorFormattedString = table.ToColorFormattedString();
            PrintF(colorFormattedString, endLine);
        }

        public static void PrintTest(string message, bool test, int padRight, bool endLine = true)
        {
            Write(message.PadRight(padRight));
            if (test)
            {
                Print("OK", ConsoleColor.Green, endLine);
            }
            else
            {
                Print("Fail", ConsoleColor.DarkRed, endLine);
            }
        }

        public static void PrintTimeElapsed(DateTime @from, string comment)
        {
            var ms = (DateTime.Now - @from).TotalMilliseconds;
            if (ms < 0.5)
                return;

            Print($"{comment} => elapsed:{ms:N3} ms", ConsoleColor.Green);
        }

        public static void Print(params ColorString[] messages)
        {
            foreach (var coloredText in messages)
                Write(coloredText.Text, coloredText.Color);
        }

        public static void Print(IEnumerable<ColorString> messages)
        {
            foreach (var coloredText in messages)
                Write(coloredText.Text, coloredText.Color);
        }

        //public static void PrintProgress()
        //{
        //    for (int i = 0; i <= 100; i++)
        //    {
        //        Console.Write($"\rProgress: {i}%   ");
        //        Thread.Sleep(25);
        //    }
        //}

        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //public static extern IntPtr GetDC(IntPtr hWnd);

        //static void DrawInConsoleFromGraphics()
        //{
        //    Process process = Process.GetCurrentProcess();
        //    Graphics g = Graphics.FromHdc(GetDC(process.MainWindowHandle));

        //    // Create pen.
        //    Pen bluePen = new Pen(Color.Blue, 3);

        //    // Create rectangle.
        //    Rectangle rect = new Rectangle(0, 0, 200, 200);

        //    // Draw rectangle to screen.
        //    g.DrawRectangle(bluePen, rect);
        //    g.DrawLine(new Pen(Color.Red, 2), new Point(100, 100), new Point(0, 0));
        //    g.DrawString("Welcome to Dotnetvisio.blogspot.com", new Font("Arial", 14),
        //        new SolidBrush(Color.Orange), new Point(10, 300));
        //    g.Save();

        //}
    }
}
