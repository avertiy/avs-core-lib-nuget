using System;
using AVS.CoreLib.Text.FormatPreprocessors;
using AVS.CoreLib.Text.FormatProviders;
using AVS.CoreLib.Text.Formatters;
using AVS.CoreLib.Text.Formatters.ColorMarkup;
using AVS.CoreLib.Text.TextProcessors;

namespace AVS.CoreLib.Text
{
    /// <summary>
    /// X is just a short notation
    /// </summary>
    public static class X
    {
        /// <summary>
        /// Extends .NET string format modifiers like N2, d, C etc. with custom formatters
        /// (e.g. ColorFormatter, NotEmptyFormatter, PriceFormatter, EnumFormatter etc) 
        /// </summary>
        public static XFormatProvider FormatProvider { get; set; } = new XFormatProvider();

        /// <summary>
        /// string started with "@..." considered as text expression for TextProcessor
        /// by default X.Format uses <seealso cref="TextExpressionProcessor"/> as TextProcessor
        /// </summary>
        public static ITextProcessor TextProcessor { get; set; } = new TextExpressionProcessor();

        /// <summary>
        /// allows to preprocess/modify argument's format 
        /// </summary>
        public static FormatPreprocessor FormatPreprocessor { get; set; } = new FormatPreprocessor();

        /// <summary>
        /// Format string using <see cref="XFormatProvider"/>
        /// 
        /// <see cref="NotEmptyFormatter"/> format symbol: !
        /// in case argument is null, empty (0 for numeric types, MinValue for datetime etc.) such arg is replaced with string.Empty $"{0:!}" => "";
        /// 
        /// <see cref="ColorMarkupFormatter"/> format symbol: -Color (foreground color) --Color (background color)
        /// put color markup $"{arg:-Red}" => "$$arg:-Red$"
        /// <remarks>color markup is used by PowerConsole</remarks>
        ///  
        /// <see cref="CompositeFormatter"/> by default does not include any formatters
        /// </summary>
        public static string Format(FormattableString str)
        {
            var result = str.ToString(FormatProvider);
            return result;
        }

        /// <summary>
        /// applies format modifier before string format and there after process the result with <see cref="TextExpressionProcessor"/>
        /// </summary>
        public static string Format(FormattableString str, IFormatPreprocessor preprocessor)
        {
            if (!(str is FormattableString2 str2))
            {
                str2 = new FormattableString2(str);
            }
            var result = str2.ToString(FormatProvider, preprocessor, TextProcessor);
            return result;
        }

        /// <summary>
        /// Format <see cref="FormattableString"/> string to string
        /// </summary>
        /// <param name="str">string</param>
        /// <param name="preprocessor">if not null each argument format is preprocessed by preprocessor</param>
        public static string Format(FormattableString2 str, IFormatPreprocessor preprocessor)
        {
            var result = str.ToString(FormatProvider, preprocessor, TextProcessor);
            return TextProcessor.Process(result);
        }

        /// <summary>
        /// Format <see cref="FormattableString"/> string to string
        /// </summary>
        /// <param name="str">string</param>
        /// <param name="preprocessor">if not null each argument format is preprocessed by preprocessor</param>
        /// <param name="textProcessor"><see cref="TextProcessor"/></param>
        public static string Format(FormattableString str, IFormatPreprocessor preprocessor, ITextProcessor textProcessor)
        {
            if (!(str is FormattableString2 str2))
            {
                str2 = new FormattableString2(str);
            }

            var result = str2.ToString(FormatProvider, preprocessor, textProcessor);
            return TextProcessor.Process(result);
        }
    }
}