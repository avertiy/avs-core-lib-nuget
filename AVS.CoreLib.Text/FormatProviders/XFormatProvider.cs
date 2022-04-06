using System;
using AVS.CoreLib.Text.Formatters;
using AVS.CoreLib.Text.Formatters.ColorMarkup;

namespace AVS.CoreLib.Text.FormatProviders
{
    /// <summary>
    /// Extend standard string format modifiers like N2, C etc. with custom formatters and modifiers
    /// <see cref="ColorMarkupFormatter"/>
    /// <see cref="NotEmptyFormatter"/>
    /// </summary>
    public class XFormatProvider : IFormatProvider
    {
        /// <summary>
        /// Formatter returned by <seealso cref="GetFormat"/>
        /// </summary>
        protected CustomFormatter Formatter { get; set; }

        private CompositeFormatter CompositeFormatter { get; set; }

        /// <summary>
        /// C-tor
        /// </summary>
        public XFormatProvider()
        {
            CompositeFormatter = new CompositeFormatter();
            Formatter = new NotEmptyFormatter() { Next = new ColorMarkupFormatter() { Next = CompositeFormatter } };
        }


        /// <summary>
        /// Append custom formatter
        /// </summary>
        public void AppendFormatter(CustomFormatter formatter)
        {
            Formatter.AppendFormatter(formatter);
        }

        /// <summary>
        /// Configure composite formatter
        /// </summary>
        public void ConfigureCompositeFormatter(Action<CompositeFormatter> configure)
        {
            configure(CompositeFormatter);
        }

        /// <inheritdoc />
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
                return Formatter;
            return null;
        }
    }
}