using System.Runtime.InteropServices;

namespace AVS.CoreLib.PowerConsole.Utilities
{
    /// <summary>
    /// Console font info
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct FontInfo
    {
        internal int cbSize;
        internal int FontIndex;
        internal short FontWidth;
        public short FontSize;
        public int FontFamily;
        public int FontWeight;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string FontName;

        public override string ToString()
        {
            return $"{FontName} [{FontSize};{FontWeight}]";
        }
    }
}