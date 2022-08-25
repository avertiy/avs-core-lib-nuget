using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;

namespace AVS.CoreLib.Math.Extensions
{
	public enum TextKind
	{
		None = 0,
		Bits = 1,
		//Byte = 8,
		Dec = 10,
		Hex = 16
	}

	public static class Hex
	{
		public static int ParseInt(string hex)
		{
			return int.Parse(hex, NumberStyles.HexNumber);
		}

		public static byte[] GetBytes(string str, TextKind format = TextKind.Hex)
		{
			switch (format)
			{
				case TextKind.Hex:
					{
						return GetBytesFromHex(str);
					}
				case TextKind.Bits:
					{
						return GetBytesFromBits(str);
					}
				case TextKind.Dec:
					{
						var bi = BigInteger.Parse(str, NumberStyles.Number);
						return bi.ToByteArray(true, true);
					}
				default:
					{
						return Encoding.UTF8.GetBytes(str);
					}
			}
		}
		public static byte[] GetBytesFromBits(string bitString)
		{
			byte[] result = Enumerable.Range(0, bitString.Length / 8).
				Select(pos => Convert.ToByte(
					bitString.Substring(pos * 8, 8),
					2)
				).ToArray();

			List<byte> mahByteArray = new List<byte>();
			for (int i = result.Length - 1; i >= 0; i--)
			{
				mahByteArray.Add(result[i]);
			}

			return mahByteArray.ToArray();
		}


		public static BigInteger ParseBigInteger(string hex)
		{
			return string.IsNullOrEmpty(hex) ? new BigInteger(0) : BigInteger.Parse(hex.StartsWith("0") ? hex : "0" + hex, NumberStyles.AllowHexSpecifier);
		}

		public static byte[] GetBytesFromHex(string hex)
		{
			return Convert.FromHexString(hex);
		}

        public static byte GetByteFromHex(string hex)
        {
            return Convert.FromHexString(hex)[0];
        }

        public static string ToHexString(params byte[] bytes)
		{
			return Convert.ToHexString(bytes);
		}

        public static bool Match(string hex, byte[] bytes)
		{
			return Convert.ToHexString(bytes) == hex;
		}

		public static bool Match(byte[] arr1, byte[] arr2)
		{
			return arr1.SequenceEqual(arr2);
		}

		public static bool Match(BigInteger number, string hex)
		{
			return number.ToHexString() == hex;
		}

		public static string FromText(string text)
		{
			var bytes = Encoding.UTF8.GetBytes(text);
			return ToHexString(bytes);
		}

	}
}