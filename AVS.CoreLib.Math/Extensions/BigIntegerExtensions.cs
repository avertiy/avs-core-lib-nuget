using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using AVS.CoreLib.Math.Bytes.Extensions;

namespace AVS.CoreLib.Math.Extensions
{
	public static class BigIntegerExtensions
	{
		public static string ToHexString(this BigInteger number)
		{
			var arr = number.ToByteArray(true, true);
			return arr.ToHexString();
		}

		public static byte[] DecomposeOnBytes(this BigInteger n, long @base = int.MaxValue)
		{
			@base += 1;
			var list = new List<int>();
			var d = n / @base;
			var rest = n % @base;
			list.Add((int)rest);

			while (d > 0)
			{
				rest = d % @base;
				d = d / @base;
				list.Add((int)rest);
			}

			return list.Select(i => (byte)i).ToArray();
		}

		public static BigInteger Factorial(this ushort n)
		{
			var result = new BigInteger(1);
			for (int i = 1; i <= n; i++)
			{
				result *= i;
			}
			return result;
		}

		public static BigInteger Factorial(this uint n)
		{
			if (n < short.MaxValue)
			{
				return Factorial((ushort)n);
			}

			var n1 = n / 2;
			var task = Task.Run(() => n1.Factorial());
			var r2 = new BigInteger(n1);
			for (uint i = n1 + 1; i <= n; i++)
			{
				r2 *= i;
			}

			task.Wait();
			return task.Result * r2;
		}

		public static BigInteger RestoreBigInteger(this byte[] bytes, int @base = 256)
		{
			var bi = new BigInteger(bytes[^1] == 0 ? @base : 0);
			for (var i = 1; i <= bytes.Length; i++)
			{
				var rest = bytes[^i];
				bi = (bi + rest);
				if (i == bytes.Length)
					break;
				bi = bi * @base;
			}

			return bi;
		}

		public static bool Equals(this BigInteger number, string hex)
		{
			return number.ToHexString() == hex;
		}

		public static bool Equals(this BigInteger number, byte[] arr)
		{
			if (arr.Length == 0)
			{
				throw new ArgumentException("Byte array must be not empty");
			}

			var bytes = number.ToByteArray();
			if (bytes.Length == arr.Length + 1)
			{
				if (bytes[0] == arr[^1] && bytes[0] != arr[0])
				{
					return bytes.Reverse().Skip(1).SequenceEqual(arr);
				}

				return bytes.Skip(1).SequenceEqual(arr);
			}
			if (bytes.Length == arr.Length)
			{
				return bytes.SequenceEqual(arr) || bytes.Reverse().SequenceEqual(arr);
			}

			return false;
		}

		public static BigInteger GetNextPrime(this BigInteger number)
		{
			if (number.Sign < 0)
			{
				throw new ArgumentOutOfRangeException($"number must be positive integer");
			}

			var i = 1;
			while (true)
			{
				var num = number + i++;
				if (num.IsPrime())
				{
					return num;
				}
			}
		}

		public static bool IsPrime(this BigInteger number)
		{
			if (number.IsEven || number <= 1)
				return false;

			var boundary = number > 999 ? 999 : number;
			for (int i = 3; i <= boundary; i += 2)
			{
				if (number % i == 0)
					return false;
			}

			return true;
		}
	}
}