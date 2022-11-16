using System;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.PowerConsole.Printers
{
    public class StringifyOptions
    {
        public int MaxLength { get; set; }
        public string Separator { get; set; } = ",";
        public string KeyValueSeparator { get; set; } = ":";
        public StringifyFormat Format { get; set; } = StringifyFormat.Default;
        public Colors Colors { get; set; }
        public bool ContainsCTags { get; set; }
        //public bool EndLine { get; set; }
    }
}