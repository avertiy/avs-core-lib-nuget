using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AVS.CoreLib.PowerConsole.Utilities
{
    /// <summary>
    /// Color formatted string is a string that contains patterns $$text:-Color$
    /// e.g. "$$text in red font:-Red$ any plain text $$text with blue background:--Blue$"
    /// ColorFormattedString enumerates tuples (string plainText, ColorScheme scheme, string coloredText)
    /// 
    /// </summary>
    public class ColorFormattedString : IEnumerable<(string text, ColorScheme scheme, string coloredText)>
    {
        /// <summary>
        /// parses $$some text:-Color$
        /// </summary>
        private static readonly Regex Regex = new Regex(@"\$\$(?<text>.*?):(?<scheme>-.*?)\$");
        public string Value { get; }

        public ColorFormattedString(string str)
        {
            Value = str;
        }

        private IEnumerable<(string text, ColorScheme scheme, string coloredText)> Iterate()
        {
            var match = Regex.Match(Value);
            var pos = 0;

            while (match.Success)
            {
                var plainText = Value.Substring(pos, match.Index - pos);
                pos = match.Index + match.Length;

                var schemeStr = match.Groups["scheme"].Value;
                var text = match.Groups["text"].Value;
                if (ColorHelper.TryParse(schemeStr, out var scheme))
                {
                    yield return (text:plainText, scheme, coloredText:text);
                }
                else
                {
                    yield return (text: plainText, null, coloredText: text);
                }
                match = match.NextMatch();
            }

            if (pos < Value.Length)
            {
                var restText = Value.Substring(pos);
                yield return (text: restText, ColorScheme.Current, null);
            }
        }

        public IEnumerator<(string text, ColorScheme scheme, string coloredText)> GetEnumerator()
        {
            return Iterate().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Iterate().GetEnumerator();
        }

        public static explicit operator ColorFormattedString(string str)
        {
            return new ColorFormattedString(str);
        }

        public static implicit operator string(ColorFormattedString str)
        {
            return str?.Value;
        }
    }
}