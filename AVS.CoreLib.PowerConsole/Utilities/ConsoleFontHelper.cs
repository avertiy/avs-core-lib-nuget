using System;
using System.Runtime.InteropServices;

namespace AVS.CoreLib.PowerConsole.Utilities
{
    internal static class ConsoleFontHelper
    {
        private const int FixedWidthTrueType = 54;
        private const int StandardOutputHandle = -11;

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetStdHandle(int nStdHandle);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool SetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool MaximumWindow, ref FontInfo ConsoleCurrentFontEx);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool GetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool MaximumWindow, ref FontInfo ConsoleCurrentFontEx);
        
        private static readonly IntPtr ConsoleOutputHandle = GetStdHandle(StandardOutputHandle);

        public static bool TryGetCurrentFont(out FontInfo fontInfo)
        {
            fontInfo = new FontInfo
            {
                cbSize = Marshal.SizeOf<FontInfo>()
            };
            return GetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref fontInfo);
        }

        public static void SetFont(FontInfo fontInfo)
        {
            // Get some settings from current font.
            if (!SetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref fontInfo))
            {
                var er = Marshal.GetLastWin32Error();
                var ex = new System.ComponentModel.Win32Exception(er,
                    $"Unable to set font {fontInfo}");
                PowerConsole.WriteError(ex, false);
            }
        }

        public static void SetFont(string font, short fontSize = 12, int fontWeight = 400)
        {
            var fontInfo = new FontInfo
            {
                cbSize = Marshal.SizeOf<FontInfo>(),
                FontIndex = 0,
                FontFamily = FixedWidthTrueType,
                FontName = font,
                FontWeight = 400,
                FontSize = fontSize
            };
            SetFont(fontInfo);
        }

        public static FontInfo GetFontInfoOrDefault(string font = "Courier New", short size =12, int weight = 400)
        {
            var fontInfo = new FontInfo
            {
                cbSize = Marshal.SizeOf<FontInfo>()
            };
            if(GetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref fontInfo))
                return fontInfo;
            return new FontInfo
            {
                FontName = font,
                FontSize = size,
                FontWeight = weight
            };
        }
    }
}