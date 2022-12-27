using System;
using System.Collections.Generic;
using System.Globalization;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Stringify;
using AVS.CoreLib.PowerConsole.ConsoleTable;
using AVS.CoreLib.PowerConsole.Extensions;
using AVS.CoreLib.PowerConsole.Printers;
using AVS.CoreLib.PowerConsole.Structs;
using AVS.CoreLib.PowerConsole.Utilities;
using AVS.CoreLib.Text.Extensions;
using AVS.CoreLib.Text.Formatters.ColorMarkup;

namespace AVS.CoreLib.PowerConsole
{
    using Console = System.Console;
    public static partial class PowerConsole
    {
        public static void PrintKeyValue<TKey, TValue>(IDictionary<TKey, TValue> dictionary,
            ConsoleColor color = ConsoleColor.White,
            string keyValueSeparator = " => ",
            string separator = "\r\n",
            bool endLine = true)
        {
            Printer.Print(dictionary.ToKeyValueString(keyValueSeparator, separator), endLine, false, color);
        }

    }
}
