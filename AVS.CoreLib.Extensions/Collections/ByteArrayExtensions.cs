using System;
using System.Globalization;
using System.Text;

namespace AVS.CoreLib.Extensions.Collections
{
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// Since .NET 5.0 available Convert.ToHexString(hash) which should be faster 
        /// </summary>
        /// <remarks>the same as BitConverter.ToString(signatureHash).Replace("-", "").ToLowerInvariant()</remarks>
        public static string ToHexString(this byte[] bytes)
        {
	        //BitConverter.ToString(bytes).Replace("-", "");
            var sb = new StringBuilder(bytes.Length*2);
            for (var i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public static string ToBase64String(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }
    }
}