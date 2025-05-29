using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xunit.Sdk;

namespace AVS.CoreLib.UnitTesting.XUnit.Exceptions
{
    /// <summary>
    /// Exception thrown when two values are unexpectedly not equal.
    /// </summary>
    public class StringEqualException : XunitException
    {
        private static readonly Dictionary<char, string> Encodings = new Dictionary<char, string>()
        {
            {
                '\r',
                "\\r"
            },
            {
                '\n',
                "\\n"
            },
            {
                '\t',
                "\\t"
            },
            {
                char.MinValue,
                "\\0"
            }
        };
        private string? _message = null;
        public  string Expected { get; set; }
        public  string Actual { get; set; }
        public  string UserMessage { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="T:Xunit.Sdk.EqualException" /> class for string comparisons.
        /// </summary>
        /// <param name="expected">The expected string value</param>
        /// <param name="actual">The actual string value</param>
        /// <param name="userMessage">user message</param>
        /// <param name="expectedIndex">The first index in the expected string where the strings differ</param>
        /// <param name="actualIndex">The first index in the actual string where the strings differ</param>
        public StringEqualException(string expected, string actual, string userMessage, int expectedIndex = -1, int actualIndex = -1)
            : base($"Assert.Equal() Failure: `{expected}` != `{actual}` \r\n{userMessage}")
        {
            Actual = actual;
            Expected = expected;
            UserMessage = userMessage;
            this.ActualIndex = actualIndex;
            this.ExpectedIndex = expectedIndex;
        }

        /// <summary>
        /// Gets the index into the actual value where the values first differed.
        /// Returns -1 if the difference index points were not provided.
        /// </summary>
        public int ActualIndex { get; private set; }

        /// <summary>
        /// Gets the index into the expected value where the values first differed.
        /// Returns -1 if the difference index points were not provided.
        /// </summary>
        public int ExpectedIndex { get; private set; }

        /// <inheritdoc />
        public override string Message
        {
            get
            {
                if (this._message == null)
                    this._message = this.CreateMessage();
                return this._message;
            }
        }

        private string CreateMessage()
        {
            if (this.ExpectedIndex == -1)
                return base.Message;
            Tuple<string, string> tuple1 = StringEqualException.ShortenAndEncode(this.Expected, this.ExpectedIndex, '↓');
            Tuple<string, string> tuple2 = StringEqualException.ShortenAndEncode(this.Actual, this.ActualIndex, '↑');
            return string.Format((IFormatProvider)CultureInfo.CurrentCulture, "{1}{0}          {2}{0}Expected: {3}{0}Actual:   {4}{0}          {5}", (object)Environment.NewLine,
                (object)this.UserMessage, (object)tuple1.Item2, (object)(tuple1.Item1 ?? "(null)"), (object)(tuple2.Item1 ?? "(null)"), (object)tuple2.Item2);
        }

        private static Tuple<string, string> ShortenAndEncode(
            string value,
            int position,
            char pointer)
        {
            int num1 = Math.Max(position - 20, 0);
            int num2 = Math.Min(position + 41, value.Length);
            StringBuilder stringBuilder1 = new StringBuilder(100);
            StringBuilder stringBuilder2 = new StringBuilder(100);
            if (num1 > 0)
            {
                stringBuilder1.Append("···");
                stringBuilder2.Append("   ");
            }
            for (int index = num1; index < num2; ++index)
            {
                char key = value[index];
                int repeatCount = 1;
                string str;
                if (StringEqualException.Encodings.TryGetValue(key, out str))
                {
                    stringBuilder1.Append(str);
                    repeatCount = str.Length;
                }
                else
                    stringBuilder1.Append(key);
                if (index < position)
                    stringBuilder2.Append(' ', repeatCount);
                else if (index == position)
                    stringBuilder2.AppendFormat("{0} (pos {1})", new object[2]
                    {
                        (object) pointer,
                        (object) position
                    });
            }
            if (value.Length == position)
                stringBuilder2.AppendFormat("{0} (pos {1})", new object[2]
                {
                    (object) pointer,
                    (object) position
                });
            if (num2 < value.Length)
                stringBuilder1.Append("···");
            return new Tuple<string, string>(stringBuilder1.ToString(), stringBuilder2.ToString());
        }
    }
}