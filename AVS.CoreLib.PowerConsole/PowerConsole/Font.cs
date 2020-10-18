using System;
using System.Threading.Tasks;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole
{
    public static partial class PowerConsole
    {
        private static FontInfo? _defaultFont = null;

        public static void SetFont(string font, short fontSize = 12, int fontWeight = 400)
        {
            if (!_defaultFont.HasValue)
                _defaultFont = ConsoleFontHelper.GetFontInfoOrDefault();
            ConsoleFontHelper.SetFont(font, fontSize, fontWeight);
        }

        public static FontInfo GetFont()
        {
            return ConsoleFontHelper.GetFontInfoOrDefault();
        }
        public static void RestoreDefaultFont()
        {
            if (_defaultFont.HasValue)
                ConsoleFontHelper.SetFont(_defaultFont.Value);
            else
                ConsoleFontHelper.SetFont("Courier New");
        }
    }
}
