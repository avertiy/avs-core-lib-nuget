using System;
using System.Diagnostics;
using AVS.CoreLib.PowerConsole.Extensions;
using AVS.CoreLib.PowerConsole.Printers;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole
{
    using Console= System.Console;
    public static partial class PowerConsole
    {
        #region WriteLine
        /// <summary>
        /// Writes the specified string value, followed by the current line terminator,
        /// to the standard output stream.
        /// </summary>
        /// <param name="message">Message to be written to console output</param>
        /// <param name="color">Color of message text</param>
        /// <param name="timeFormat">Date and time format of the time written next to message in console output</param>
        public static void WriteLine(string message, ConsoleColor color, string timeFormat = "yyyy-MM-dd hh:mm:ss.ff")
        {
            if (!string.IsNullOrEmpty(timeFormat))
                message = $"{DateTime.Now.ToString(timeFormat)} {message}";

            Printer.Print(message, color, true);
        }
        public static void WriteLine(string message, ColorScheme scheme, string timeFormat = "yyyy-MM-dd hh:mm:ss.ff")
        {
            if (!string.IsNullOrEmpty(timeFormat))
                message = $"{DateTime.Now.ToString(timeFormat)} {message}";

            Printer.Print(message, scheme, true);
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
            Printer.WriteLine(voidMultipleEmptyLines);
        } 
        #endregion

        public static void Write(string str)
        {
            Printer.Print(str, false);
        }

        public static void Write(string str, bool endLine)
        {
            Printer.Print(str, endLine);
        }

        public static void Write(string str, ConsoleColor color)
        {
            Printer.Print(str, color, false);
        }

        public static void Write(string str, ConsoleColor? color, bool endLine = false)
        {
            Printer.Print(str, color, endLine);
        }

        public static void Write(string str, ColorScheme scheme, bool endLine = false)
        {
            Printer.Print(str, scheme, endLine);
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
