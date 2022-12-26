using System;
using System.Text;
using AVS.CoreLib.Console.ColorFormatting.Extensions;

namespace AVS.CoreLib.Console.ColorFormatting.Tags
{
    /// <summary>
    /// replace color tags <see cref="CTag"/> with ansi codes
    /// </summary>
    public class CTagProcessor : TagProcessor
    {
        public static bool FixBgColorLineEnding { get; set; } = true;

        public override void Process(StringBuilder sb)
        {
            Process(sb, (tagName, startIndex, endIndex) =>
            {
                if (!Enum.TryParse(tagName, out CTag tag))
                    return -1;

                if (FixBgColorLineEnding && tagName.StartsWith("bg"))
                    sb.FixBgColorLineEnding(endIndex);

                // fix bg color at the end of line 
                if (endIndex == sb.Length - 1 || endIndex == sb.Length)
                    sb.Append(' ');

                var length = tagName.Length;
                var ansiCode = tag.ToAnsiCode();

                sb.Replace($"</{tagName}>", AnsiCodes.RESET, endIndex - length-3, length + 3);
                sb.Replace($"<{tagName}>", ansiCode, startIndex, length+2);

                return ansiCode.Length;
            });
        }
    }
}