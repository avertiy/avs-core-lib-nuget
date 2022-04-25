using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace AVS.CoreLib.Text
{
    /// <summary>
    /// usage:
    ///  var sb = new FormattableStringBuilder();
    ///  sb.Append($"x = {x}").Append($"y = {y}");
    ///  var fs = b.ToFormattableString();
    /// </summary>
    public class FormattableStringBuilder
    {
        private readonly List<object> _arguments = new List<object>();

        private readonly StringBuilder _buffer = new StringBuilder();

        private int _argumentsOffset;

        /// <summary>
        /// Creates <see cref="FormattableString"/>
        /// </summary>
        public FormattableString ToFormattableString()
        {
            var format = this._buffer.ToString();
            var args = this._arguments.ToArray();

            var result = FormattableStringFactory.Create(format, args);
            return result;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this._buffer.ToString();
        }

        /// <summary>
        /// append raw string
        /// </summary>
        public FormattableStringBuilder AppendRaw(string value)
        {
            this._buffer.Append(value);
            return this;
        }

        /// <summary>
        /// Append argument
        /// </summary>
        public FormattableStringBuilder Append(object value)
        {
            this._buffer.Append("{").Append(this._argumentsOffset).Append("}");
            this._arguments.Add(value);
            this._argumentsOffset++;

            return this;
        }

        /// <summary>
        /// append FormattableString
        /// </summary>
        public FormattableStringBuilder Append(FormattableString value)
        {
            this.AppendFormatHelper(value.Format, value.ArgumentCount);
            this._arguments.AddRange(value.GetArguments());

            this._argumentsOffset += value.ArgumentCount;

            return this;
        }

        // https://referencesource.microsoft.com/#mscorlib/system/text/stringbuilder.cs,2c3b4c2e7c43f5a4
        private void AppendFormatHelper(string format, int argsLength)
        {
            var pos = 0;
            var len = format.Length;
            var ch = '\x0';

            while (true)
            {
                while (pos < len)
                {
                    ch = format[pos];
                    pos++;

                    if (ch == '}')
                    {
                        if (pos < len && format[pos] == '}')
                        {
                            // Treat as escape character for }}
                            pos++;
                        }
                        else
                        {
                            FormatError(format, pos);
                        }
                    }

                    if (ch == '{')
                    {
                        if (pos < len && format[pos] == '{')
                        {
                            pos++;
                        }
                        else
                        {
                            pos--;
                            break;
                        }
                    }

                    this._buffer.Append(ch);
                }

                if (pos == len)
                {
                    break;
                }

                pos++;
                if (pos == len || (ch = format[pos]) < '0' || ch > '9')
                {
                    FormatError(format, pos);
                }

                var parameterIndex = 0;
                do
                {
                    parameterIndex = parameterIndex * 10 + ch - '0';
                    pos++;
                    if (pos == len)
                    {
                        FormatError(format, pos);
                    }

                    ch = format[pos];
                }
                while (ch >= '0' && ch <= '9' && parameterIndex < 1000000);

                if (parameterIndex >= argsLength)
                {
                    throw new FormatException(
                        $"Index must be greater than or equal to zero and less than the size of the argument list. "
                        + $"Index: {parameterIndex}, format: '{format}', argument list size: {argsLength}.");
                }

                this._buffer.Append("{").Append(this._argumentsOffset + parameterIndex);

                // skip spaces
                while (pos < len && (ch = format[pos]) == ' ')
                {
                    pos++;
                }

                if (ch == ',')
                {
                    this._buffer.Append(',');

                    // format {1,5}
                    var width = 0;
                    pos++;

                    // skip spaces
                    while (pos < len && format[pos] == ' ')
                    {
                        pos++;
                    }

                    if (pos == len)
                    {
                        FormatError(format, pos);
                    }

                    ch = format[pos];

                    if (ch == '-')
                    {
                        this._buffer.Append('-');
                        pos++;

                        if (pos == len)
                        {
                            FormatError(format, pos);
                        }

                        ch = format[pos];
                    }

                    if (ch < '0' || ch > '9')
                    {
                        FormatError(format, pos);
                    }

                    do
                    {
                        width = width * 10 + ch - '0';
                        pos++;
                        if (pos == len)
                        {
                            FormatError(format, pos);
                        }

                        ch = format[pos];
                    }
                    while (ch >= '0' && ch <= '9' && width < 1000000);

                    this._buffer.Append(width);
                }

                // skip spaces
                while (pos < len && (ch = format[pos]) == ' ')
                {
                    pos++;
                }

                if (ch == ':')
                {
                    // {0:D}
                    pos++;

                    while (true)
                    {
                        if (pos == len)
                        {
                            FormatError(format, pos);
                        }

                        ch = format[pos];
                        pos++;

                        if (ch == '{')
                        {
                            if (pos < len && format[pos] == '{')
                            {
                                // Treat as escape character for {{
                                pos++;
                            }
                            else
                            {
                                FormatError(format, pos);
                            }
                        }
                        else if (ch == '}')
                        {
                            if (pos < len && format[pos] == '}')
                            {
                                // Treat as escape character for }}
                                pos++;
                            }
                            else
                            {
                                pos--;
                                break;
                            }
                        }

                        this._buffer.Append(ch);
                    }
                }

                if (ch != '}')
                {
                    FormatError(format, pos);
                }

                pos++;

                this._buffer.Append('}');
            }
        }

        private static void FormatError(string format, int position)
        {
            throw new FormatException($"Input string was not in a correct format. Format: '{format}', position: {position}.");
        }
    }
}
