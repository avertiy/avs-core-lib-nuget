using System.Collections;
using System.Text.RegularExpressions;

namespace AVS.CoreLib.Logging.ColorFormatter.ColorMakup
{
    /// <summary>
    /// ColorMarkupString2 is a string wrapper allows to iterate string through color markup blocks $arg:color scheme$
    /// w.g. $plain text $red text:-Red$ $blue background:--Blue$"
    /// 
    /// </summary>
    /// <remarks>ColorMarkupString2 uses $ signs instead of curly brackets used by ColorMarkupString, this allows to avoid conflicts with string format arguments</remarks>
    public class ColorMarkupString2 : IEnumerable<(string text, string colorScheme, string coloredText)>
    {
        /// <summary>
        /// parses text and color scheme $text:-ForegroundColor --BackgroundColor$
        /// regex is not strict 
        /// </summary>
        internal static readonly Regex regex = new Regex("\\$(?<text>.*?):(?<scheme>-.*?)\\$", RegexOptions.Multiline);
        //internal static readonly Regex regex = new Regex("\\$(?<text>[^$]*?):(?<scheme>-.*?)\\$");
        
        /// <summary>
        /// initial string
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// C-tor 
        /// </summary>
        public ColorMarkupString2(string str)
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
                
                if(colorScheme.StartsWith(":"))
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
        public static explicit operator ColorMarkupString2(string str)
        {
            return new ColorMarkupString2(str);
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