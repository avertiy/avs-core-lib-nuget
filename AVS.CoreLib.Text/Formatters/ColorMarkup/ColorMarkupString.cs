using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AVS.CoreLib.Text.Formatters.ColorMarkup
{
    /// <summary>
    /// ColorMarkupString is a string wrapper allows to iterate string through markup blocks in tuples (string plainText, string color scheme, string coloredText)
    /// color markup string looks similar to format string "some plain text {text:-ForegroundColor} some other plain text {text:--BackgroundColor}"
    /// </summary>
    public class ColorMarkupString : IEnumerable<(string text, string colorScheme, string coloredText)>
    {
        /// <summary>
        /// parses text and color scheme {text:-ForegroundColor --BackgroundColor}
        /// regex is not strict 
        /// </summary>
        internal static readonly Regex regex = new Regex("{(?<text>.*?):(?<scheme>-.*?)}");
        /// <summary>
        /// match colorized string like @:Color e.g. "some text@:Red"
        /// </summary>
        internal static readonly Regex regex2 = new Regex("@:(?<color>(.{3,12}))$");
        /// <summary>
        /// initial string
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// C-tor 
        /// </summary>
        public ColorMarkupString(string str)
        {
            Value = str;
        }

        private IEnumerable<(string plainText, string colorScheme, string coloredText)> Iterate()
        {
            var match = regex.Match(Value);
            var pos = 0;

            while (match.Success)
            {
                var plainText = Value.Substring(pos, match.Index - pos);
                pos = match.Index + match.Length;

                var colorScheme = match.Groups["scheme"].Value;

                if (colorScheme.StartsWith(":"))
                    colorScheme = colorScheme.Substring(1);

                var coloredText = match.Groups["text"].Value;
                yield return (plainText, colorScheme, coloredText);
                match = match.NextMatch();
            }

            if (pos < Value.Length)
            {
                var restText = Value.Substring(pos);
                yield return (plainText: restText, null, null);
            }
        }

        /// <inheritdoc />
        public IEnumerator<(string text, string colorScheme, string coloredText)> GetEnumerator()
        {
            return Iterate().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Iterate().GetEnumerator();
        }

        /// <summary>
        /// creates ColorMarkupString from str
        /// </summary>
        public static explicit operator ColorMarkupString(string str)
        {
            return new ColorMarkupString(str);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Value;
        }

        /// <summary>
        /// match <see cref="Value"/> with color markup regex
        /// returns true if match success, false otherwise
        /// </summary>
        public bool HasMarkup
        {
            get
            {
                var match = regex.Match(Value);
                return match.Success;
            }
        }
    }
}