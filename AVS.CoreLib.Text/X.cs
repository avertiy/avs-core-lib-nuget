using System;
using AVS.CoreLib.Text.FormatProviders;
using AVS.CoreLib.Text.TextProcessors;

namespace AVS.CoreLib.Text
{
    /// <summary>
    /// X is just a short notation
    /// </summary>
    public static class X
    {
        private static XFormatProvider _formatProvider;
        /// <summary>
        /// Extends .NET string format modifiers like N2, d, C etc. with custom formatters
        /// (e.g. ColorFormatter, NotEmptyFormatter, PriceFormatter, EnumFormatter etc) 
        /// </summary>
        public static XFormatProvider FormatProvider
        {
            get => _formatProvider ?? (_formatProvider = new XFormatProvider());
            set => _formatProvider = value;
        }

        private static ITextProcessor _textProcessor;
        /// <summary>
        /// string started with "@..." considered as text that TextProcessor should process
        /// by default X.Format uses <seealso cref="TextExpressionProcessor"/> as TextProcessor
        /// </summary>
        public static ITextProcessor TextProcessor
        {
            get => _textProcessor ?? (_textProcessor = new TextExpressionProcessor());
            set => _textProcessor = value;
        }

        /// <summary>
        /// Replaces the format item(s) in a specified string with the string representation of the corresponding object
        /// Standard string format modifiers like N2, C etc. are extended with <see cref="XFormatProvider"/>
        /// If string starts with @ it is treated as string with expressions and processed by text processor
        /// the default text processor is <see cref="TextExpressionProcessor"/>
        /// e.g. "@any text before `expression: arg;` text after"
        /// if arg is empty the whole expression will not be included into result string: "any text before text after"
        /// Note symbol @ at the beginning of the string and expression delimiters ``(quotes) 
        /// treated as service symbols which are not included in the result string
        /// </summary>
        public static string Format(FormattableString str)
        {
            var result = str.ToString(FormatProvider);
            return TextProcessor.Process(result);
        }
    }
}