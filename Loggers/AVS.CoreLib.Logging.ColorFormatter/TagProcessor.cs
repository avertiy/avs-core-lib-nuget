using System.Text;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Logging.ColorFormatter.Enums;
using AVS.CoreLib.Logging.ColorFormatter.Extensions;
using AVS.CoreLib.Logging.ColorFormatter.Utils;
using Microsoft.Extensions.Options;

namespace AVS.CoreLib.Logging.ColorFormatter;

public interface ITagProcessor
{
    string Process(string input);
}

public class TagProcessor
{
    private readonly StringBuilder _sb;
    private readonly string _message;
    public TagProcessor(StringBuilder sb, string message)
    {
        this._sb = sb;
        _message = message;
    }

    public void ProcessColorTags()
    {
        if (!TagHelper.HasTags(_sb))
            return;

        for (var i = 0; i < _sb.Length - 3; i++)
        {
            if (!_sb.MatchTag(i, out var tagName, out var closingTagIndex) || !Enum.TryParse(tagName, out CTag tag))
                continue;
            
            var length = tagName.Length + 2;
            var end = closingTagIndex + length + 1;

            if(tagName.StartsWith("bg"))
                _sb.FixBgColorLineEnding(end);

            // fix bg color at the end of line 
            if (end == _sb.Length - 1 || end == _sb.Length)
                _sb.Append(' ');

            _sb.Replace($"</{tagName}>", AnsiCodes.RESET, closingTagIndex, length + 1);
            var ansiCode = tag.ToAnsiCode();
            _sb.Replace($"<{tagName}>", ansiCode, i, length);

            i += ansiCode.Length;
            //return back cursor position
            if(i > length)
                i -= (length - ansiCode.Length+1);
        }
    }

    public void ProcessRgbTags()
    {
        var closingRbgTagInd = _sb.IndexOf("</RGB>", 10, false);
        if (closingRbgTagInd > 0)
        {
            var rgbTagsCount = 0;

            label1:
            if (RgbTagHelper.TryParse(_sb, out var rgb, out var rgbText))
            {
                rgbTagsCount++;
                _sb.Replace(rgbText, AnsiCodes.Rgb(rgb));
                _sb.Replace("</RGB>", AnsiCodes.RESET);
                //check whether there any other RGB tags
                goto label1;
            }

            if (rgbTagsCount > 1)
                _sb.Append(AnsiCodes.RESET);

        }
    }

    public void ProcessHeaderTags(string headerPadding)
    {
        //<H1>
        var tag = _message.Substring(0, 4);
        var closingTag = _message.Substring(_message.Length - 5, 5);

        if (tag == "<H1>" && closingTag == "</H1>")
        {
            if (!_sb.StartsWith(Environment.NewLine))
                _sb.Insert(0, Environment.NewLine);

            _sb.Replace("<H1>", headerPadding ?? "<B> ======= ");
            _sb.Replace("</H1>", headerPadding ?? " ======= </B>");
            return;
        }

        if (tag == "<H2>" && closingTag == "</H2>")
        {
            if (!_sb.StartsWith(Environment.NewLine))
                _sb.Insert(0, Environment.NewLine);
            _sb.Replace("<H2>", "<B> >>>> ");
            _sb.Replace("</H2>", "</B>");
        }
    }

}


internal static class RgbTagHelper
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

