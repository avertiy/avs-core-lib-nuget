namespace AVS.CoreLib.PowerConsole.ConsoleTable
{
    public class TableStyle
    {
        public char Cross { get; set; } = '+';
        public char Pad { get; set; } = '-';
        public char Bar { get; set; } = '|';
        public BorderStyle Border { get; set; } = BorderStyle.Full;

        public string Spacing = " ";
    }
}