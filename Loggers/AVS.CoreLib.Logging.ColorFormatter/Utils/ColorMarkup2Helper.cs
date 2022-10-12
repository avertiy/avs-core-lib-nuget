using System.Text;
using System.Text.RegularExpressions;
using AVS.CoreLib.Logging.ColorFormatter.ColorMakup;

namespace AVS.CoreLib.Logging.ColorFormatter.Utils;


/// <summary>
/// text utilities for color markup on $:color$ pattern 
/// </summary>
public static class ColorMarkup2Helper
{
    #region end line color markup text$$:color
    /// <summary>
    /// match colorized string like $$:Color e.g. "some text$$:Red"
    /// </summary>
    internal static readonly Regex endLineRegex = new Regex("\\$\\$:(?<color>(.{3,12}))$");

    /// <summary>
    /// match text with color markup2 regex e.g. some text@:-Red
    /// returns true if match success, false otherwise
    /// </summary>
    public static bool HasEndLineColorMarkup(string text, out string color, out int index)
    {
        var match = endLineRegex.Match(text);
        color = match.Success ? match.Groups["color"].Value : null;
        index = match.Success ? match.Index : text.Length;
        return match.Success;
    }

    /// <summary>
    /// remove color markup2 from the text e.g. "text@:Red" => "text"
    /// </summary>
    public static string StripEndLineColorMarkup(this string text)
    {
        var match2 = endLineRegex.Match(text);

        // strip @:Color i.e. markup at the end of the string  
        if (match2.Success)
            text = text.Substring(0, match2.Index);

        return text;
    }

    #endregion

    /// <summary>
    /// match text with color markup regex e.g. text {arg:-Red --Blue}
    /// returns true if match success, false otherwise
    /// </summary>
    public static bool HasColorMarkup(string text)
    {
        var match = ColorMarkupString2.regex.Match(text);
        return match.Success;
    }

    public static ArgumentType GetArgumentType(string arg)
    {
        if (arg == null)
            throw new ArgumentNullException();

        if (arg.StartsWith("[") && arg.EndsWith("]"))
        {
            return ArgumentType.Array;
        }

        if (arg.StartsWith("{") && arg.EndsWith("}"))
            return ArgumentType.TextJson;

        if (arg.Contains("%"))
            return ArgumentType.Percentage;

        if (arg.Contains("$") || arg.Contains("USD") || arg.Contains("EUR") || arg.Contains("UAH"))
        {
            return ArgumentType.Cash;
        }

        if (double.TryParse(arg, out var d))
        {
            return d >= 0 ? ArgumentType.Numeric : ArgumentType.NumericNegative;
        }

        if (DateTime.TryParse(arg, out var date))
            return ArgumentType.Date;

        if (arg.Length < 25)
            return ArgumentType.String;

        return ArgumentType.Text;
    }


    /// <summary>
    /// colorize arguments within the formatted message
    /// </summary>
    internal static string AddColorMarkup(string formattedMessage, string format, string[] keys, Func<ArgumentType, ConsoleColors> getColors)
    {
        //format:  "some text {arg1} text {arg2}{arg3}"
        //message: "some text 1 text 0.123456789"
        //output: "some text $1:color$ text $0.123456789:-color$"
        var parts = format.Split(keys, StringSplitOptions.None);

        if (parts.Any(x => x.EndsWith('$') && x.Contains(":-")))
        {
            // most likely text already contains color formatting and due to we don't use here regex
            // it might break existing color formatting
            // so do nothing
            return formattedMessage;
        }

        //parts2: [] {"1", "0.123456789"}
        var values = formattedMessage.Split(parts.Where(x => x.Length > 0).ToArray(), StringSplitOptions.RemoveEmptyEntries);

        var sb = new StringBuilder();

        var i = 0;
        for (; i < parts.Length && i < values.Length; i++)
        {
            var isColoredArg = i + 1 < parts.Length && parts[i + 1].StartsWith(':') && parts[i + 1].EndsWith('$');

            if (isColoredArg)
            {
                sb.Append(parts[i]);
                sb.Append(values[i]);
                continue;
            }

            var type = GetArgumentType(values[i]);
            // if string do nothing
            if (type == ArgumentType.String)
            {
                sb.Append(parts[i]);
                sb.Append(values[i]);
            }
            else if (type == ArgumentType.Numeric && parts[i].EndsWith("#"))
            {
                var colors = getColors(type);
                var markup = $"{parts[i].Substring(0, parts[i].Length-1)}$#{values[i]}:{colors.ToString()}$";
                sb.Append(markup);
            }
            else
            {
                var colors = getColors(type);
                var markup = $"{parts[i]}${values[i]}:{colors.ToString()}$";
                sb.Append(markup);
            }
            
        }

        if (parts.Length > i)
            sb.Append(parts[i]);

        for (; i < values.Length; i++)
        {
            var type = GetArgumentType(values[i]);
            if (type == ArgumentType.String)
            {
                sb.Append(values[i]);
            }
            else if (type == ArgumentType.Numeric && parts[i].EndsWith("#"))
            {
                var colors = getColors(type);
                var markup = $"{parts[i].Substring(0, parts[i].Length - 1)}$#{values[i]}:{colors.ToString()}$";
                sb.Append(markup);
            }
            else
            {
                var colors = getColors(type);
                var markup = $"${values[i]}:{colors.ToString()}$";
                sb.Append(markup);
            }
        }

        var colorizedMessage = sb.ToString();
        return colorizedMessage;
    }

    /// <summary>
    /// remove color markup from the text
    /// </summary>
    public static string StripColorMarkup(this string text)
    {
        var match = ColorMarkupString2.regex.Match(text);

        if (!match.Success)
            return text;

        var sb = new StringBuilder();
        var pos = 0;

        while (match.Success)
        {
            var pos2 = match.Index - pos;
            if (pos2 > 0)
            {
                //plain text
                sb.Append(text.Substring(pos, pos2));
            }

            var coloredText = match.Groups["text"].Value;
            sb.Append(coloredText);

            match = match.NextMatch();
            pos = match.Index + match.Length;
        }

        if (pos < text.Length)
        {
            var restText = text.Substring(pos);
            sb.Append(restText);
        }

        return sb.ToString();
    }

    

    /// <summary>
    /// format plain text into color markup text e.g. FormatColor("str", Green) => {"str":-Green}; 
    /// </summary>
    public static string FormatWithColor(string str, ConsoleColor foreground)
    {
        return $"${str}:-{foreground}$";
    }

    /// <summary>
    /// Colorize plain text within a text that might contain color markup
    /// </summary>
    /// <remarks>
    /// if text does not contain a color markup, use <see cref="FormatWithColor"/>
    /// </remarks>
    /// <returns></returns>
    public static string ColorizePlainText(string text, ConsoleColor? foreground)
    {
        if (!foreground.HasValue)
            return text;

        var sb = new StringBuilder();
        var match = ColorMarkupString2.regex.Match(text);
        var pos = 0;

        while (match.Success)
        {
            var pos2 = match.Index - pos;
            if (pos2 > 0)
            {
                var plainText = text.Substring(pos, pos2);
                sb.Append(FormatWithColor(plainText, foreground.Value));
            }

            var coloredText = match.Value;
            sb.Append(coloredText);
            pos = match.Index + match.Length;
            match = match.NextMatch();
        }

        if (pos < text.Length)
        {
            var restText = text.Substring(pos);
            sb.Append(FormatWithColor(restText, foreground.Value));
        }

        return sb.ToString();
    }

    /// <summary>
    /// Parse color markup e.g. "{arg:-Reg --Black}", also short representation "{arg:-Reg}" or "{arg:--Reg}" 
    /// </summary>
    public static bool TryParse(string str, out ConsoleColor? foreground, out ConsoleColor? background)
    {
        foreground = null;
        background = null;
        if (string.IsNullOrEmpty(str))
        {
            return false;
        }

        ConsoleColor color;
        var ind = str.LastIndexOf('-');

        //no `-` symbol just Color
        if (ind < 0)
        {
            //e.g. Red
            if (!TryParseConsoleColor(str, out color))
                return false;
            foreground = color;
            return true;
        }

        //foreground color: -Color e.g. -Red
        if (ind == 0 && TryParseConsoleColor(str.Substring(1), out color))
        {
            foreground = color;
        }
        //background color: --Color e.g. --Red  
        else if (ind == 1 && TryParseConsoleColor(str.Substring(2), out color))
        {
            background = color;
        }
        else
        {
            //e.g. -Red --Gray
            var parts = str.Split(new[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
                return false;

            // most likely invalid format
            if(parts[0].Length < 3 || parts[1].Length < 3)
                return false;

            foreground = TryParseConsoleColor(parts[0], out color)
                ? color
                : (ConsoleColor?)null;
            background = TryParseConsoleColor(parts[1], out color)
                ? color
                : (ConsoleColor?)null;
        }

        return true;
    }

    private static bool TryParseConsoleColor(string value, out ConsoleColor color)
    {
        if (Enum.TryParse(value, out color))
        {
            return true;
        }
        color = ConsoleColor.Black;
        return false;
    }

    public static string FormatWithColorMarkup(this ConsoleColors scheme, string text)
    {
        return scheme.HasValue && !string.IsNullOrEmpty(text) ? $"${text}:{scheme.ToString()}$" : text;
    }
}