using System;

namespace AVS.CoreLib.PowerConsole.Printers
{
    [Flags]
    public enum PrintOptions2
    {
        Default = 0,
        /// <summary>
        /// don't add timestamp label
        /// </summary>
        NoTimestamp = 1,
        /// <summary>
        /// no need to process ctags 
        /// </summary>
        NoCTags = 2,
        /// <summary>
        /// no need to end line 
        /// </summary>
        Inline = 4,
        /// <summary>
        /// no need to void empty lines
        /// </summary>
        AllowEmptyLines = 8,
    }
}