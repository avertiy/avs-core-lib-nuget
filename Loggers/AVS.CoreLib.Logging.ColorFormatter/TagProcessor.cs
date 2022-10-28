using System.Text;
using AVS.CoreLib.Logging.ColorFormatter.Extensions;
using AVS.CoreLib.Logging.ColorFormatter.Utils;
using Microsoft.Extensions.Options;

namespace AVS.CoreLib.Logging.ColorFormatter;

public class TagProcessor
{
    private readonly StringBuilder sb;
    public TagProcessor(StringBuilder sb)
    {
        this.sb = sb;
    }

    public void ProcessColorTags()
    {
        if (!TagHelper.HasTags(sb))
            return;

        var colors = Enum.GetValues<ConsoleColor>();
        foreach (var color in colors)
        {
            if (sb.IndexOf($"</{color}>", 5, false) > 0)
            {
                sb.Replace($"<{color}>", AnsiCodes.Color(color));
                sb.Replace($"</{color}>", AnsiCodes.RESET);
            }

            if (sb.IndexOf($"</bg{color}>", 7, false) > 0)
            {
                sb.Replace($"<bg{color}>", AnsiCodes.BgColor(color));
                sb.Replace($"</bg{color}>", AnsiCodes.RESET);
            }
        }
    }

    public void ProcessRgbTags()
    {
        var closingRbgTagInd = sb.IndexOf("</RGB>", 10, false);
        if (closingRbgTagInd > 0)
        {
            var rgbTagsCount = 0;

            label1:
            if (RgbHelper.TryParse(sb, out var rgb, out var rgbText))
            {
                rgbTagsCount++;
                sb.Replace(rgbText, AnsiCodes.Rgb(rgb));
                sb.Replace("</RGB>", AnsiCodes.RESET);
                //check whether there any other RGB tags
                goto label1;
            }

            if (rgbTagsCount > 1)
                sb.Append(AnsiCodes.RESET);

        }
    }

    public void ProcessHeaderTags(string headerPadding = " ===== ")
    {
        //<H1>
        var tag = sb.ToString(0, 4);
        var closingTag = sb.ToString(sb.Length - 5, 5);

        if (tag == "<H1>" && closingTag == "</H1>")
        {
            sb.Remove(0, 4);
            sb.Remove(sb.Length - 5, 5);
            sb.Replace(tag, "\r\n" + headerPadding);
            sb.Replace(closingTag, headerPadding);
        }
    }

}


internal static class RgbHelper
{
    public static bool TryParse(string str, out (byte R, byte G, byte B) rgb, out string rgbText)
    {
        var ind = str.IndexOf("<RGB:", StringComparison.Ordinal);
        var parts = str.Substring(ind + "<RGB:".Length, 12).Split(new[] { ',', '>' }, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length > 2 && byte.TryParse(parts[0], out var r) && byte.TryParse(parts[1], out var g) && byte.TryParse(parts[2], out var b))
        {
            rgb = (r, g, b);
            rgbText = str.Substring(ind, 8 + parts[0].Length + parts[1].Length + parts[2].Length);
            return true;
        }

        rgb = (0, 0, 0);
        rgbText = null;
        return false;
    }

    public static bool TryParse(StringBuilder str, out (byte R, byte G, byte B) rgb, out string rgbText)
    {
        var ind = str.IndexOf("<RGB:");
        var parts = str.ToString(ind + "<RGB:".Length, 12).Split(new[] { ',', '>' }, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length > 2 && byte.TryParse(parts[0], out var r) && byte.TryParse(parts[1], out var g) && byte.TryParse(parts[2], out var b))
        {
            rgb = (r, g, b);
            rgbText = str.ToString(ind, 8 + parts[0].Length + parts[1].Length + parts[2].Length);
            return true;
        }

        rgb = (0, 0, 0);
        rgbText = null;
        return false;
    }
}

