using System;

namespace AVS.CoreLib.PowerConsole.Printers2
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
        PlainText = Inline | NoCTags | NoTimestamp
        ///// <summary>
        ///// no need to void empty lines
        ///// </summary>
        //AllowEmptyLines = 8,
    }

    public static class PrintOptionsHelper
    {
        public static PrintOptions2 InLine(this PrintOptions2 opt, bool inline)
        {
            return inline ? opt | PrintOptions2.Inline : opt;
        }

        public static PrintOptions2 InLine(this PrintOptions2 opt)
        {
            return opt | PrintOptions2.Inline;
        }

        public static PrintOptions2 Combine(this PrintOptions2 opt, PrintOptions2 other)
        {
            return opt | other;
        }
    }
}