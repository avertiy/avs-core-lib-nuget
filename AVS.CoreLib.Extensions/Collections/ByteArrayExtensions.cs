using System;
using System.Globalization;

namespace AVS.CoreLib.Extensions.Collections
{
    public static class ByteArrayExtensions
    {
        public static string ToHexString(this byte[] value)
        {
            var output = string.Empty;
            for (var i = 0; i < value.Length; i++)
            {
                output += value[i].ToString("x2", CultureInfo.InvariantCulture);
            }
            return (output);
        }

        public static string ToBase64String(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }
    }
}