using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole.Structs
{
    public struct ColorString
    {
        public string Text { get; set; }
        public ColorScheme Color { get; set; }

        public ColorString(string text)
        {
            Text = text;
            Color = ColorScheme.GetCurrentScheme();
        }

        public ColorString(string text, ColorScheme color)
        {
            Text = text;
            Color = color;
        }

        public override string ToString()
        {
            return $"$${Text}:-{Color.Foreground} --{Color.Background}$";
        }
    }
}