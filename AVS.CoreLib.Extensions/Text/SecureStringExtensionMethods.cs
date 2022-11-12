using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;

namespace AVS.CoreLib.Extensions
{
    public static class SecureStringExtensionMethods
    {
        public static string GetString(this SecureString source)
        {
            lock (source)
            {
                int length = source.Length;
                var num = System.IntPtr.Zero;
                char[] destination = new char[length];
                string str;
                try
                {
                    num = Marshal.SecureStringToBSTR(source);
                    Marshal.Copy(num, destination, 0, length);
                    str = string.Join<char>("", (IEnumerable<char>)destination);
                }
                finally
                {
                    if (num != System.IntPtr.Zero)
                        Marshal.ZeroFreeBSTR(num);
                }
                return str;
            }
        }

        public static SecureString ToSecureString(this string source)
        {
            var secureString = new SecureString();
            foreach (var c in source)
                secureString.AppendChar(c);
            secureString.MakeReadOnly();
            return secureString;
        }
    }
}