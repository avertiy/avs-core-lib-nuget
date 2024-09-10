using System;

namespace AVS.CoreLib.Text.Formatters.GenericTypeFormatter
{
    /// <summary>
    /// Type formatter <see cref="GenericTypeFormatter"/>
    /// </summary>
    public class TypeFormatter<T> : ITypeFormatter
    {
        private readonly Func<string, T, string> _formatter;

        /// <summary>
        /// string format qualifies
        /// </summary>
        public string[] Qualifiers { get; }

        /// <summary>
        /// C-tor
        /// </summary>
        public TypeFormatter(string[] qualifiers, Func<string, T, string> formatter)
        {
            _formatter = formatter;
            Qualifiers = qualifiers;
        }

        /// <summary>
        /// Format argument, argument should be convertible to type T 
        /// </summary>
        public string Format(string format, object arg)
        {
            return _formatter(format, (T)arg);
        }
    }
}