using System;
using System.Collections.Generic;
using System.Linq;

namespace AVS.CoreLib.Text.Formatters
{
    /// <summary>
    /// Generic formatter allows to register a type formatter on the fly
    /// without the need to implement CustomFormatter 
    /// </summary>
    public class CompositeFormatter : CustomFormatter
    {
        private readonly Dictionary<string, ITypeFormatter> _formatters = new Dictionary<string, ITypeFormatter>();

        /// <summary>
        /// Register type formatter 
        /// </summary>
        public void AddTypeFormatter<T>(string[] qualifiers, Func<string, T, string> func)
        {
            var formatter = new TypeFormatter<T>(qualifiers, func);
            _formatters.Add(typeof(T).Name, formatter);
        }

        /// <summary>
        /// Remove type formatter
        /// </summary>
        public void RemoveTypeFormatter<T>()
        {
            _formatters.Remove(typeof(T).Name);
        }

        /// <inheritdoc />
        protected override string CustomFormat(string format, object arg, IFormatProvider formatProvider)
        {
            var key = arg.GetType().Name;
            if (_formatters.ContainsKey(key))
            {
                return _formatters[key].Format(format, arg);
            }

            return arg?.ToString();
        }

        /// <inheritdoc />
        protected override string NoFormat(object arg)
        {
            var key = arg.GetType().Name;
            if (_formatters.ContainsKey(key))
                return _formatters[key].Format(string.Empty, arg);

            return base.NoFormat(arg);
        }

        /// <inheritdoc />
        protected override bool Match(string format)
        {
            return _formatters.Any(x => x.Value.Qualifiers.Any(q => q == format));
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{nameof(CompositeFormatter)} [{string.Join(", ", _formatters.Keys)}]";
        }
    }
}