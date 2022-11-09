using System;

namespace AVS.CoreLib.PowerConsole.Printers
{
    public interface IPrinter
    {
        void Print(FormattableString str, ColorPalette palette, bool endLine = true);
    }
}