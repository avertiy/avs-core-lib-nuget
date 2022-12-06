using System;
using System.IO;
using AVS.CoreLib.Abstractions.Text;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.PowerConsole.FormatProcessors;
using AVS.CoreLib.Text;

namespace AVS.CoreLib.PowerConsole.Printers
{
    public interface IPrinter
    {
        Func<FormattableString, string> Format { get; set; }
        Func<FormattableString, string> XFormat { get; set; }
        /// <summary>
        /// holds <see cref="ITagProcessor"/> that is used to process text containing color or other tags 
        /// </summary>
        TagProcessor TagProcessor { get; }
        /// <summary>
        /// holds <see cref="IFormatPreprocessor"/> for auto-highlight arguments feature applied to <see cref="FormattableString"/>
        /// </summary>
        IFormatPreprocessor FormatPreprocessor { get; }
        void Print(string str, bool endLine);
        void Print(string str, ConsoleColor? color, bool endLine);
        void WriteLine(bool voidMultipleEmptyLines = true);
    }
}