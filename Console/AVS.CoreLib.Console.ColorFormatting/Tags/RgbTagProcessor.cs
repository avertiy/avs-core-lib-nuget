using System;
using System.Text;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.Console.ColorFormatting.Tags
{
    /// <summary>
    /// custom rgb tag looks as the following "<RGB color="120,100,200">text here</RGB>"
    /// the tag will be translated into corresponding rgb ansi codes
    /// </summary>
    public class RgbTagProcessor : TagProcessor
    {
        public int Counter;
        public override void Process(StringBuilder sb)
        {
            var closingRbgTagInd = sb.IndexOf("</RGB>", 10, false);
            if (closingRbgTagInd > 0)
            {
                var counter = 0;

                label1:
                if (TryParse(sb, out var rgb, out var rgbText))
                {
                    counter++;
                    sb.Replace(rgbText, AnsiCodes.Rgb(rgb));
                    sb.Replace("</RGB>", AnsiCodes.RESET);
                    //check whether there any other RGB tags
                    goto label1;
                }

                if (counter > 1)
                    sb.Append(AnsiCodes.RESET);
                Counter = counter;
            }
        }

        private static bool TryParse(StringBuilder sb, out (byte R, byte G, byte B) rgb, out string rgbText)
        {
            var ind = sb.IndexOf("<RGB color=\"");
            var parts = sb.ToString(ind + "<RGB color=\"".Length, 12).Split(new[] { ',', '"' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length > 2 && byte.TryParse(parts[0], out var r) && byte.TryParse(parts[1], out var g) && byte.TryParse(parts[2], out var b))
            {
                rgb = (r, g, b);
                var l = "<RGB color=\"".Length + 2 + parts[0].Length + parts[1].Length + parts[2].Length + 2;
                rgbText = sb.ToString(ind, l);
                return true;
            }

            rgb = (0, 0, 0);
            rgbText = string.Empty;
            return false;
        }

    }
}