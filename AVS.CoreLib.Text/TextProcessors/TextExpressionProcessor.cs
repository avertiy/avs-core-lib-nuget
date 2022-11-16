using System.Text;
using System.Text.RegularExpressions;
using AVS.CoreLib.Abstractions.Text;

namespace AVS.CoreLib.Text.TextProcessors
{
    /// <summary>
    /// if string starts with @ symbol it is treated as string with expressions: `anything:value` or `anything:value;`
    /// where value is tested whether it is empty or not, if it is empty the whole expression will not be included in result string
    /// </summary>
    public class TextExpressionProcessor : ITextProcessor
    {
        private static readonly Regex _regex = new Regex("(`.*?:.*?`)");

        /// <inheritdoc />
        public string Process(string str)
        {
            if (!str.StartsWith("@"))
                return str;

            var parts = _regex.Split(str.Substring(1));
            var sb = new StringBuilder();

            foreach (var part in parts)
            {
                var text = part;
                if (part.StartsWith('`') && part.EndsWith('`'))
                {
                    var subParts = part.Substring(1, part.Length - 2).Split(':');
                    if (subParts.Length == 2)
                    {
                        if (string.IsNullOrWhiteSpace(subParts[1].Trim(';')))
                            continue;
                    }
                    text = part.Trim('`');
                }
                sb.Append(text);
            }

            return sb.ToString();
        }
    }
}