using System;
using System.Diagnostics;

namespace AVS.CoreLib.Text.Formatters
{
    public abstract class CustomFormatter : IFormatProvider, ICustomFormatter
    {
        public CustomFormatter Next { get; set; }

        public void AppendFormatter(CustomFormatter formatter)
        {
            if (Next == null)
                Next = formatter;
            else
                Next.AppendFormatter(formatter);
        }

        public virtual string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (Equals(null, arg))
                return string.Empty;

            if (string.IsNullOrEmpty(format))
                return arg.ToString();

            return Match(format) ? CustomFormat(format, arg, formatProvider) : DefaultFormat(format, arg, formatProvider);
        }

        protected virtual string CustomFormat(string format, object arg, IFormatProvider formatProvider)
        {
            return DefaultFormat(format, arg, formatProvider);
        }

        /// <summary>
        /// Matches the format qualifier
        /// when True - CustomFormat(format, arg) is called
        /// when False - DefaultFormat(format, arg) is called
        /// </summary>
        protected virtual bool Match(string format) => true;

        protected string DefaultFormat(string format, object arg, IFormatProvider formatProvider)
        {
            if (Next == null)
                return string.Format("{0:" + format + "}", arg);

            return Next.Format(format, arg, formatProvider);
        }

        [DebuggerStepThrough]
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
                return this;
            return null;
        }
    }
}