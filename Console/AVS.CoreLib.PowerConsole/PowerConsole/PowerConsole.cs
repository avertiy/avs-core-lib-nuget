﻿using System;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole
{
    public static partial class PowerConsole
    {
        static PowerConsole()
        {
            ColorScheme.Default = new ColorScheme(Console.BackgroundColor, Console.ForegroundColor);
            DefaultSchemeBackup = ColorScheme.Default;
        }

        private static ColorScheme DefaultSchemeBackup { get; set; }
        public static ColorScheme CurrentColorScheme => ColorScheme.GetCurrentScheme();
        public static ColorScheme PreviousScheme = new ColorScheme(Console.BackgroundColor, Console.ForegroundColor);


        public static void ApplyColor(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }

        public static void ApplyColorScheme(ColorScheme scheme)
        {
            Console.ForegroundColor = scheme.Foreground;
            Console.BackgroundColor = scheme.Background;
        }

        /// <summary>
        /// Applies default color scheme
        /// </summary>
        public static void ColorSchemeReset()
        {
            ApplyColorScheme(ColorScheme.Default);
        }

        public static void SetDefaultColorScheme(ColorScheme scheme)
        {
            ColorScheme.Default = scheme;
        }

        /// <summary>
        /// Restores ColorScheme.Default to ColorScheme.Classic
        /// </summary>
        public static void RestoreDefaultColorScheme()
        {
            ColorScheme.Default = DefaultSchemeBackup;
        }

        /// <summary>
        /// Indicates whether new line (\r\n) has been just written 
        /// </summary>
        public static bool NewLineFlag = true;

        /// <summary>
        /// Status of the message will produce beep sound when written to console
        /// <see cref="WriteLine(string,MessageStatus,string)"/>
        /// </summary>
        public static MessageStatus? BeepOnMessageStatus { get; set; } = null;

        public static void ClearLine(int left = 0)
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(left, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth - left));
            Console.SetCursorPosition(left, currentLineCursor);
        }

        public static void ClearRegion(int left, int top, int rows)
        {
            var clearLine = new string(' ', Console.WindowWidth - left);
            for (var i = 0; i < rows; i++)
            {
                Console.SetCursorPosition(left, top + i);
                Console.Write(clearLine);
            }
            Console.SetCursorPosition(left, top);
        }

        public static void PressEnterToExit()
        {
            Console.Write("Press enter to quit.");
            Console.ReadLine();
        }
    }
}