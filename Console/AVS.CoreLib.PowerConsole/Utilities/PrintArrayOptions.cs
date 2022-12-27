using System;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Stringify;

namespace AVS.CoreLib.PowerConsole.Utilities
{
    public class PrintArrayOptions : StringifyOptions
    {
        public Colors Colors { get; set; }
        public bool ContainsCTags { get; set; }
    }
}