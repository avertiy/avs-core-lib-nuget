using System.Collections.Generic;

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
	}
}