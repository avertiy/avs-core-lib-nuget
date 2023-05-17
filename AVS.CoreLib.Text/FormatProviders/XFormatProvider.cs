using System;
using AVS.CoreLib.Text.Formatters;
using AVS.CoreLib.Text.Formatters.ColorMarkup;
using AVS.CoreLib.Text.Formatters.GenericFormatter;
using AVS.CoreLib.Text.Formatters.GenericTypeFormatter;

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
        protected Formatters.CustomFormatter Formatter { get; set; }

        private GenericTypeFormatter GenericTypeFormatter { get; set; }

        /// <summary>
        /// C-tor
        /// </summary>
        public XFormatProvider()
        {
            GenericTypeFormatter = new GenericTypeFormatter();
            Formatter = new NotEmptyFormatter() { Next = new ColorMarkupFormatter() { Next = GenericTypeFormatter } };
        }


        /// <summary>
        /// Append custom formatter
        /// </summary>
        public void AppendFormatter(Formatters.CustomFormatter formatter)
        {
            Formatter.AppendFormatter(formatter);
        }

        /// <summary>
        /// Configure composite formatter
        /// </summary>
        public void ConfigureCompositeFormatter(Action<GenericTypeFormatter> configure)
        {
            configure(GenericTypeFormatter);
        }

        /// <inheritdoc />
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(System.ICustomFormatter))
                return Formatter;
            return null;
        }
    }
}