using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using AVS.CoreLib.Math.Bytes.Extensions;

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


        /// <summary>
        /// Decode hex string to bytes
        /// </summary>
        /// <param name="hex">Hex string</param>
        /// <returns>Bytes</returns>
        public static byte[] FromHexString(ReadOnlySpan<char> hex)
        {
            if (hex.Length % 2 != 0)
                throw new ArgumentException("Odd hex string length.", nameof(hex));

            var result = new byte[hex.Length / 2];
            for (int i = 0; i < result.Length; i++)
                result[i] = Convert.ToByte(hex.Slice(i * 2, 2).ToString(), 16);

            return result;
        }

        public static byte[] GetBytesFromHex(string hex)
		{
            return FromHexString(hex.AsSpan());
            //return Convert.FromHexString(hex);
		}

        public static byte GetFirstByteFromHex(string hex)
        {
            if (string.IsNullOrEmpty(hex) || hex.Length < 2)
                return 0xb;

            var @byte = Convert.ToByte(hex.Substring(0, 2), 16);
            return @byte;
        }

        public static string ToHexString(params byte[] bytes)
		{
			return bytes.ToHexString();
		}

        public static bool Match(string hex, byte[] bytes)
		{
			return bytes.ToHexString() == hex;
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