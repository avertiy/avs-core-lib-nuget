using System;
using System.Runtime.InteropServices;
using System.Security;

namespace AVS.CoreLib.REST.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Get the string the secure string is representing
        /// </summary>
        /// <param name="source">The source secure string</param>
        public static string GetString(this SecureString source)
        {
            lock (source)
            {
                int length = source.Length;
                IntPtr intPtr = IntPtr.Zero;
                char[] array = new char[length];
                try
                {
                    intPtr = Marshal.SecureStringToBSTR(source);
                    Marshal.Copy(intPtr, array, 0, length);
                    return string.Join("", array);
                }
                finally
                {
                    if (intPtr != IntPtr.Zero)
                    {
                        Marshal.ZeroFreeBSTR(intPtr);
                    }
                }
            }
        }
    }
}