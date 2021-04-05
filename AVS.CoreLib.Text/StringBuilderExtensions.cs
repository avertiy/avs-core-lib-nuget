using System;
using System.Text;

namespace AVS.CoreLib.Text
{
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Append X.Format(str)
        /// </summary>
        public static StringBuilder XAppend(this StringBuilder sb, FormattableString str)
        {
            return sb.Append(X.Format(str));
        }
    }
}