using System;
using System.Collections.Generic;
using AVS.CoreLib.Math.Extensions;
using AVS.CoreLib.Utilities;

namespace AVS.CoreLib.Math.Bytes.Extensions
{
	public static class ByteExtensions
	{
		public static int GetBytesCount(this byte b)
		{
			return b == 0 ? 256 : b;
		}

		public static byte[] Repeat(this byte b, int n)
		{
			var list = new List<byte>(n);
			for (var i = 0; i < n; i++)
				list.Add(b);

			return list.ToArray();
		}

        /// <summary>
        /// split byte on 2 half bytes
        /// example: 254.Split() => (15,14) 
        /// </summary>
        public static (int, int) Split(this byte value)
        {
            var high = value >> 4;
            var low = value - high * 16;
            return (high, low);
        }

        public static string ToHex(this byte @byte)
        {
            return Convert.ToHexString(new byte[] {@byte});
        }

        
    }
}