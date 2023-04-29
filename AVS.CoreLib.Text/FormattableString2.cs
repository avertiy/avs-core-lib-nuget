using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using AVS.CoreLib.Abstractions.Text;

namespace AVS.CoreLib.Text
{
    /// <summary>
    /// Extends <see cref="FormattableString"/> with some additional functionality
    /// </summary>
    public class FormattableString2 : FormattableString, IEnumerable<(string format, object arg)>
    {
        private readonly string _format;
        private readonly object[] _arguments;
        private static readonly Regex _regex = new Regex("{\\d+?:(?<fmt>.*?)?}|{(?<empty>\\d)}");

        /// <summary>
        /// C-tor
        /// </summary>
        public FormattableString2(string format, object[] arguments)
        {
            _format = format;
            _arguments = arguments;
        }

        /// <summary>
        /// C-tor
        /// </summary>
        public FormattableString2(FormattableString str)
        {
            _format = str.Format;
            _arguments = str.GetArguments();
        }

        /// <summary>
        /// Indexer get/set argument by index 
        /// </summary>
        public object this[int index]
        {
            get => _arguments[index];
            set => _arguments[index] = value;
        }

        /// <summary>
        /// get format for an argument at specified index
        /// </summary>
        public string GetFormat(int index)
        {
            var matches = _regex.Matches(_format);
            var match = matches[index];
            return match.Groups["fmt"].Value;
        }

        /// <inheritdoc />
        public override string Format => _format;

        /// <inheritdoc />
        public override object[] GetArguments() { return _arguments; }

        /// <inheritdoc />
        public override int ArgumentCount => _arguments.Length;

        /// <inheritdoc />
        public override object GetArgument(int index) { return _arguments[index]; }

        /// <inheritdoc />
        public override string ToString(IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, _format, _arguments);
        }

        /// <summary>
        /// Preprocess format of arguments before formatting string, format string
        /// then process the result string with text processor 
        /// </summary>
        public string ToString(IFormatProvider formatProvider,
            IFormatPreprocessor preprocessor,
            ITextProcessor textProcessor)
        {
            var sb = new StringBuilder();
            if (preprocessor != null)
            {
                var matches = _regex.Matches(_format);
                var pos = 0;
                for (var i = 0; i < matches.Count; i++)
                {
                    var match = matches[i];
                    var fmt = match.Groups["fmt"];
                    if (string.IsNullOrEmpty(fmt.Value))
                    {
                        sb.Append(_format.Substring(pos, match.Index - pos + match.Length - 1));
                        var newFormat = preprocessor.Process(string.Empty, _arguments[i]);
                        sb.Append(':');
                        sb.Append(newFormat);
                        sb.Append('}');
                        pos = match.Index + match.Length;
                    }
                    else
                    {
                        sb.Append(_format.Substring(pos, fmt.Index - pos));
                        var newFormat = preprocessor.Process(fmt.Value, _arguments[i]);
                        sb.Append(newFormat);
                        sb.Append('}');
                        pos = fmt.Index + fmt.Length + 1;
                    }
                }

                if (pos < _format.Length)
                {
                    sb.Append(_format.Substring(pos));
                }
            }
            else
            {
                sb.Append(_format);
            }

            var format = sb.ToString();
            var text = string.Format(formatProvider, format, _arguments);
            return textProcessor != null ? textProcessor.Process(text) : text;
        }

        /// <inheritdoc />
        public IEnumerator<(string format, object arg)> GetEnumerator()
        {
            var matches = _regex.Matches(_format);
            for (var i = 0; i < _arguments.Length; i++)
            {
                var match = matches[i];
                var format = match.Groups["fmt"].Value;
                yield return (format, arg: _arguments[i]);
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return _format;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}