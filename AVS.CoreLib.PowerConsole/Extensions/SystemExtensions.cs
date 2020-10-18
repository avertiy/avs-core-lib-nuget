using System;
using System.Text;

namespace AVS.CoreLib.PowerConsole.Extensions
{
    public static class SystemExtensions
    {
        public static string Truncate(this string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            return str.Substring(0, Math.Min(str.Length, maxLength));
        }

        public static string ToArrayString(this Array arr, bool addLength = true)
        {
            var sb = new StringBuilder("[");

            var enumerator = arr.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var str = enumerator.Current.ToString();
                if (str.Length > 10)
                {
                    sb.AppendLine();
                    sb.Append(str);
                }
                else
                    sb.Append(", ");
            }

            sb.Length -= 2;
            sb.Append("]");

            if (addLength && arr.Length > 5)
                sb.Append($"(#{arr.Length})");

            return sb.ToString();
        }
    }
}

