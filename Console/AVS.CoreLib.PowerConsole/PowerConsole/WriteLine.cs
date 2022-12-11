using System;
using System.Diagnostics;
using AVS.CoreLib.PowerConsole.Enums;
using AVS.CoreLib.PowerConsole.Extensions;
using AVS.CoreLib.PowerConsole.Printers;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole
{
    using Console = System.Console;

    /// <remarks>
    /// Write/WriteLine methods provide pure behavior i.e. no c-tags, ansi-codes etc.
    /// The message argument is written to output stream <see cref="Out"/>
    /// </remarks>
    public static partial class PowerConsole
    {
        #region WriteLine

        public static void WriteLine(string message, bool voidEmptyLines = false)
        {
            Printer.PrintLine(message, voidEmptyLines);
        }

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator,
        /// to the standard output stream.
        /// </summary>
        /// <param name="message">Message to be written to console output</param>
        /// <param name="color">Color of message text</param>
        /// <param name="timeFormat">Date and time format of the time written next to message in console output</param>
        public static void WriteLine(string message, ConsoleColor? color, string timeFormat = "yyyy-MM-dd hh:mm:ss.ff")
        {
            if (!string.IsNullOrEmpty(timeFormat))
                message = $"{DateTime.Now.ToString(timeFormat)} {message}";

            Printer.Print(message, true, color, false);
        }
        public static void WriteLine(string message, ColorScheme scheme, string timeFormat = "yyyy-MM-dd hh:mm:ss.ff")
        {
            if (!string.IsNullOrEmpty(timeFormat))
                message = $"{DateTime.Now.ToString(timeFormat)} {message}";

            Printer.Print(message,true, scheme, false);
        }

        public static void WriteLine(int posX, int posY, params string[] arr)
        {
            ClearRegion(posX, posY, arr.Length);
            foreach (var text in arr)
            {
                Console.WriteLine(text);
            }
        }

        public static void WriteLine(bool voidMultipleEmptyLines = true)
        {
            Printer.PrintLine(voidMultipleEmptyLines);
        } 

        #endregion

        public static void Write(string str)
        {
            Printer.Print(str, false);
        }
     
        public static void Write(string str, ConsoleColor? color, bool endLine = false)
        {
            Printer.Print(str, endLine, color, containsCTags: false);
        }

        public static void Write(string str, ColorScheme scheme, bool endLine = false)
        {
            Printer.Print(str, endLine, scheme, containsCTags:false);
        }

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator,
        /// to the standard output stream.
        /// </summary>
        /// <param name="message">Message to be written to console output</param>
        /// <param name="status">status of the message to be written to console output</param>
        /// <param name="timeFormat">Date and time format of the time written next to message in console output</param>
        public static void WriteLine(string message, MessageStatus status,
            string? timeFormat = "yyyy-MM-dd hh:mm:ss.ff")
        {
            Printer.Print(message, status, timeFormat);
            if (status == BeepOnMessageStatus)
                Console.Beep();
        }

        [Conditional("DEBUG")]
        public static void WriteDebug(string str, string? timeFormat = "yyyy-MM-dd hh:mm:ss.ff")
        {
            WriteLine(str, MessageStatus.Debug, timeFormat);
        }

        public static void WriteError(Exception ex, bool printStackTrace = true, string? timeFormat = "yyyy-MM-dd hh:mm:ss.ff")
        {
            Printer.PrintError(ex, printStackTrace, timeFormat);
            if (BeepOnMessageStatus is MessageStatus.Error)
                Console.Beep();
        }
    }
}
