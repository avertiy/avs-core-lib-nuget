﻿using System;
using System.Text;

namespace AVS.CoreLib.PowerConsole
{
    public static partial class PowerConsole
    {
        public static ConsoleColor Foreground
        {
            get => System.Console.ForegroundColor;
            set => System.Console.ForegroundColor = value;
        }

        public static ConsoleColor BackgroundColor
        {
            get => System.Console.BackgroundColor;
            set => System.Console.BackgroundColor = value;
        }

        public static Encoding InputEncoding
        {
            get => Console.InputEncoding;
            set => Console.InputEncoding = value;
        }
        public static Encoding OutputEncoding
        {
            get => Console.OutputEncoding;
            set => Console.OutputEncoding = value;
        }

        public static bool CapsLock => Console.CapsLock;
        public static bool NumberLock => Console.NumberLock;
        public static bool CursorVisible => Console.CursorVisible;
        public static bool KeyAvailable => Console.KeyAvailable;
        public static int BufferHeight
        {
            get => Console.BufferHeight;
            set => Console.BufferHeight = value;
        }

        public static int BufferWidth
        {
            get => Console.BufferWidth;
            set => Console.BufferWidth = value;
        }

        public static int CursorSize
        {
            get => Console.CursorSize;
            set => Console.CursorSize = value;
        }

        public static int CursorLeft
        {
            get => Console.CursorLeft;
            set => Console.CursorLeft = value;
        }

        public static int CursorTop
        {
            get => Console.CursorTop;
            set => Console.CursorTop = value;
        }
        public static int WindowHeight
        {
            get => Console.WindowHeight;
            set => Console.WindowHeight = value;
        }
        public static int WindowWidth
        {
            get => Console.WindowWidth;
            set => Console.WindowWidth = value;
        }
        public static int WindowLeft
        {
            get => Console.WindowLeft;
            set => Console.WindowLeft = value;
        }
        public static int WindowTop
        {
            get => Console.WindowTop;
            set => Console.WindowTop = value;
        }

        public static void SetCursorPosition(int left, int top)
        {
            Console.SetCursorPosition(left, top);
        }

        public static void SetWindowPosition(int left, int top)
        {
            Console.SetWindowPosition(left, top);
        }
    }
}
