using AVS.CoreLib.PowerConsole.Structs;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole.ConsoleTable
{
    public class Column
    {
        public string Title { get; set; }
        public int Width { get; set; }
        public ColorScheme? ColorScheme { get; set; }
        public override string ToString()
        {
            var pad = (Width - Title.Length) / 2 + Title.Length;
            var str = Title.PadLeft(pad, ' ').PadRight(Width, ' ');
            return str;
        }

        public ColorString ToColorString()
        {
            var pad = (Width - Title.Length) / 2 + Title.Length;
            var str = Title.PadLeft(pad, ' ').PadRight(Width, ' ');
            if (ColorScheme.HasValue)
            {
                return new ColorString(str, ColorScheme.Value); //$"$${str}:{ColorScheme.Value}$";
            }
            else
            {
                return new ColorString(str);
            }
        }
    }
}