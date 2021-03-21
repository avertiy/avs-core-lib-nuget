using System;
using AVS.CoreLib.Text.Formatters;

namespace AVS.CoreLib.Text.FormatProviders
{
    /// <summary>
    /// Extends standard string format modifiers like N2, C etc. with custom formatters
    /// (e.g. ColorFormatter, NotEmptyFormatter, PriceFormatter, EnumFormatter etc) 
    /// </summary>
    public class XFormatProvider : IFormatProvider
    {
        protected CustomFormatter Formatter { get; }

        public XFormatProvider()
        {
            Formatter = new NotEmptyFormatter() { Next = new ColorFormatter() };
        }

        protected XFormatProvider(CustomFormatter formatter)
        {
            Formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
        }

        /// <summary>
        /// You can extend base functionality by adding custom formatters
        /// e.g. 
        /// X.FormatProvider.AppendFormatter(new PriceFormatter());
        /// X.FormatProvider.AppendFormatter(new PairStringFormatter());
        /// X.FormatProvider.AppendFormatter(new TradingEnumsFormatter());
        /// usually custom formatters are added within RegisterServices
        /// </summary>
        public void AppendFormatter(CustomFormatter formatter)
        {
            Formatter.AppendFormatter(formatter);
        }

        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
                return Formatter;
            return null;
        }
    }
}