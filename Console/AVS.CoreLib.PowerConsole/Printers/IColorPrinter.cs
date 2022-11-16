using System;
using System.Collections.Generic;
using AVS.CoreLib.PowerConsole.ConsoleTable;
using AVS.CoreLib.PowerConsole.Structs;
using AVS.CoreLib.PowerConsole.Utilities;
using AVS.CoreLib.Text.Formatters.ColorMarkup;

namespace AVS.CoreLib.PowerConsole.Printers
{
    public interface IColorPrinter : IPrinter
    {
        void Print(string str, bool endLine, ConsoleColor color);
        void Print(FormattableString str, bool endLine, ConsoleColor color);

        void PrintArray<T>(string message,
            IEnumerable<T> enumerable,
            StringifyOptions? options,
            Func<T, string>? formatter,
            bool endLine,
            ConsoleColor color);

        void PrintDictionary<TKey, TValue>(string message,
            IDictionary<TKey, TValue> dictionary,
            StringifyOptions? options,
            Func<TKey, TValue, string> formatter,
            bool endLine,
            ConsoleColor color);

        void PrintJson<T>(T obj, bool indented, bool endLine, ConsoleColor color);

        void PrintTable(Table table, bool autoWidth, bool endLine, ConsoleColor color);

        void PrintHeader(string header, string template, string lineIndentation, bool endLine, ConsoleColor color);

        void PrintTest(string message, bool test, int padRight, bool endLine, ConsoleColor color);

        void PrintTimeElapsed(string message, DateTime from, bool endLine, ConsoleColor color);


        //void Print(string str, bool endLine, ColorScheme color);
        //void Print(FormattableString str, bool endLine, ColorScheme color);
        //void Print(string str, ConsoleColor? color, bool endLine = true);
        //void Print(string str, ColorScheme scheme, bool endLine = true);
        //void Print(FormattableString str, ConsoleColor color, bool endLine = true);
        //void Print(FormattableString str, ColorPalette palette, bool endLine = true);
        //void Print(ColorMarkupString str, bool endLine = true);
        //void PrintConsoleColors();
        //void PrintArray<T>(string message, IEnumerable<T> enumerable, Func<T, string> formatter, ConsoleColor? color, bool endLine);
        //void PrintJson<T>(T obj, bool indented, ConsoleColor color, bool endLine);
        //void PrintTable(Table table, bool autoWidth, ConsoleColor color, bool endLine);
        //void Print(IEnumerable<ColorString> messages);
        //void PrintTimeElapsed(DateTime from, string message, ConsoleColor color, bool endLine);
    }


}