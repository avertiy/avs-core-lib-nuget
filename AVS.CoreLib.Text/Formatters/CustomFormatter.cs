using System;
using System.Diagnostics;

namespace AVS.CoreLib.Text.Formatters
{
    /// <summary>
    /// base abstract class for custom format providers
    /// </summary>
    public abstract class CustomFormatter : IFormatProvider, System.ICustomFormatter
    {
        /// <summary>
        /// points to the next formatter when few formatters are combined into one 
        /// </summary>
        public CustomFormatter Next { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formatter"></param>
        public CustomFormatter AppendFormatter(CustomFormatter formatter)
        {
            if (Next == null)
            {
                Next = formatter;
            }
            else
            {
                Next.AppendFormatter(formatter);
            }

            return formatter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public virtual string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (Equals(null, arg))
                return string.Empty;

            if (string.IsNullOrEmpty(format))
                return NoFormat(arg);

            if (Match(format))
                return CustomFormat(format, arg, formatProvider);

            return DefaultFormat(format, arg, formatProvider);
        }

        /// <summary>
        /// convert arg to string when format symbol is missing i.e. format is null or empty
        /// </summary>
        protected virtual string NoFormat(object arg)
        {
            return Next == null ? arg.ToString() : Next.NoFormat(arg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        protected virtual string CustomFormat(string format, object arg, IFormatProvider formatProvider)
        {
            return DefaultFormat(format, arg, formatProvider);
        }

        /// <summary>
        /// Matches the format qualifier
        /// when True - CustomFormat(format, arg) is called
        /// when False - DefaultFormat(format, arg) is called
        /// </summary>
        protected virtual bool Match(string format)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        protected string DefaultFormat(string format, object arg, IFormatProvider formatProvider)
        {
            if (Next == null)
                return string.Format("{0:" + format + "}", arg);

            if (string.IsNullOrEmpty(format))
                return NoFormat(arg);

            return Next.Format(format, arg, formatProvider);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formatType"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(System.ICustomFormatter))
                return this;
            return null;
        }
    }
}