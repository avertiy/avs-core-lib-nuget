using System;
using System.Collections.Generic;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.PowerConsole.ConsoleWriters;
using AVS.CoreLib.PowerConsole.Extensions;

namespace AVS.CoreLib.PowerConsole.Printers
{
    public class StringifyPrinter : ColorPrinter
    {
        public StringifyPrinter(IConsoleWriter writer) : base(writer)
        {
        }

        public void PrintArray<T>(IEnumerable<T> enumerable,
            string? message,
            StringifyOptions? options,
            Func<T, string>? formatter,
            bool endLine)
        {
            string str;
            var tags = false;
            if (options == null)
            {
                str = enumerable.Stringify(StringifyFormat.Default, ",", formatter);
            }
            else
            {
                str = enumerable.Stringify(options.Format, options.Separator, formatter);
                str = options.Colors.Colorize(str);
                tags = options.ContainsCTags;
            }
            
            var text = message == null ? str : $"{message}{str}";
            base.Print(text, endLine, tags);
        }

        public void PrintDictionary<TKey, TValue>(
            string? message, IDictionary<TKey, TValue> dictionary,
            StringifyOptions? options,
            Func<TKey, TValue, string>? formatter,
            bool endLine)
        {
            string str;
            var tags = false;
            if (options == null)
            {
                str = dictionary.Stringify(StringifyFormat.Default, ",", ":", formatter);
            }
            else
            {
                str = dictionary.Stringify(options.Format, options.Separator, options.KeyValueSeparator, formatter, options.MaxLength);
                str = options.Colors.Colorize(str);
                tags = options.ContainsCTags;
            }

            var text = message == null ? str : $"{message}{str}";
            base.Print(text, endLine, tags);
        }

        
    }
}