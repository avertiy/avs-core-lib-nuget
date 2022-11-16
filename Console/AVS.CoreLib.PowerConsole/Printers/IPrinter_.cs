using System;
using System.Collections.Generic;
using AVS.CoreLib.PowerConsole.ConsoleTable;

namespace AVS.CoreLib.PowerConsole.Printers
{
    public interface IPrinter
    {
        void Print(string str, bool endLine);
        void Print(FormattableString str, bool endLine);

        void PrintArray<T>(string message,
            IEnumerable<T> enumerable,
            StringifyOptions? options,
            Func<T, string>? formatter,
            bool endLine);

        void PrintDictionary<TKey, TValue>(string message,
            IDictionary<TKey, TValue> dictionary,
            StringifyOptions? options,
            Func<TKey, TValue, string> formatter,
            bool endLine);

        void PrintJson<T>(T obj, bool indented, bool endLine);
        
        void PrintTable(Table table, bool autoWidth, bool endLine);

        void PrintHeader(string header, string template, string lineIndentation, bool endLine);
        
        void PrintTest(string message, bool test, int padRight, bool endLine);

        void PrintTimeElapsed(string message, DateTime from, bool endLine);
    }
}