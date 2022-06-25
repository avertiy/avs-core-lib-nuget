using System;
using System.Collections.Generic;
using System.Linq;

namespace AVS.CoreLib.Math.Bytes.Extensions
{
    public static class ByteArrayExtensions
    {
        public static string AsHexString(this byte[] array)
        {
            return Convert.ToHexString(array);
        }

        public static string AsArrayString(this byte[] array, string separator = ",")
        {
            return $"[{string.Join(separator, array)}]";
        }

        public static string AsBytesString(this byte[] array)
        {
            return $"[{string.Join(",", array)}]";
        }

        public static string AsBytesString(this IEnumerable<byte> bytes)
        {
            return $"[{string.Join(",", bytes)}]";
        }

        public static Dictionary<byte, int> ComputeFrequency(this byte[] bytes)
        {
            var dict = new Dictionary<byte, int>();
            foreach (var b in bytes)
            {
                if (!dict.ContainsKey(b))
                {
                    dict.Add(b, 0);
                }

                dict[b]++;
            }

            return dict;
        }

        public static int ComputeSum(this byte[] bytes)
        {
            int sum = 0;
            foreach (var b in bytes)
            {
                sum += b;
            }

            return sum;
        }

        public static int[] ToInt32Array(this byte[] bytes)
        {
            if (bytes.Length % 4 != 0)
                throw new ArgumentException();

            var span = new ReadOnlySpan<byte>(bytes);
            var arr = new int[bytes.Length / 4];
            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = BitConverter.ToInt32(span.Slice(i * 4, 4));
            }

            return arr;
        }

        public static uint[] ToUInt32Array(this byte[] bytes)
        {
            if (bytes.Length % 4 != 0)
                throw new ArgumentException();

            var span = new ReadOnlySpan<byte>(bytes);
            var arr = new uint[bytes.Length / 4];
            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = BitConverter.ToUInt32(span.Slice(i * 4, 4));
            }

            return arr;
        }

        public static short[] ToInt16Array(this byte[] bytes)
        {
            if (bytes.Length % 2 != 0)
                throw new ArgumentException();

            var span = new ReadOnlySpan<byte>(bytes);
            var arr = new short[bytes.Length / 2];
            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = BitConverter.ToInt16(span.Slice(i * 2, 2));
            }

            return arr;
        }

        public static ushort[] ToUInt16Array(this byte[] bytes)
        {
            if (bytes.Length % 2 != 0)
                throw new ArgumentException();

            var span = new ReadOnlySpan<byte>(bytes);
            var arr = new ushort[bytes.Length / 2];
            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = BitConverter.ToUInt16(span.Slice(i * 2, 2));
            }

            return arr;
        }

        public static int[] ToInt32ArrayFake(this byte[] bytes)
        {
            if (bytes.Length % 4 != 0)
                throw new ArgumentException();

            var span = new ReadOnlySpan<byte>(bytes);
            var arr = new int[bytes.Length / 4];
            for (var i = 0; i < arr.Length; i++)
            {
                if (bytes[i * 4 + 3] > 127)
                {
                    bytes[i * 4 + 3] -= 128;
                }

                arr[i] = BitConverter.ToInt32(span.Slice(i * 4, 4));
            }

            return arr;
        }

        public static IEnumerable<byte[]> Slice(this byte[] bytes, int chunkSize)
        {
            if (bytes == null || bytes.Length == 0)
                throw new ArgumentNullException(nameof(bytes));

            if (chunkSize <= 0)
                throw new ArgumentException("Chunk size must be positive.", nameof(chunkSize));
            for (var i = 0; i < bytes.Length; i += chunkSize)
            {
                var l = System.Math.Min(chunkSize, bytes.Length - i) + i;
                var chunk = bytes[i..l];
                yield return chunk;
            }
        }

        public static byte[] Difference(this byte[] bytes)
        {
            return Difference(bytes, BytesGenerator.AllBytes);
        }

        public static byte[] Difference(this byte[] bytes, byte[] fullSet)
        {
            return fullSet.Where(x => !bytes.Contains(x)).ToArray();
        }

        public static byte[] Repeat(this byte[] bytes, int n)
        {
            var list = new List<byte>(bytes.Length * n);

            for (var i = 0; i < n; i++)
            {
                list.AddRange(bytes);
            }

            return list.ToArray();
        }

        public static byte[] Shuffle(this byte[] bytes)
        {
            var list = new List<byte>(bytes);
            var rand = new Random();
            //rand.Next()
            for (var i = 0; i < bytes.Length; i++)
            {
                var r = rand.Next(i, bytes.Length);
                var b = list[i];
                list[i] = list[r];
                list[r] = b;
            }

            return list.ToArray();
        }

        public static byte[] OrderByChunks(this byte[] bytes, int chunkSize)
        {
            var n = bytes.Length / chunkSize;
            var list = new List<byte>(bytes.Length);
            for (var i = 0; i < n; i++)
            {
                list.AddRange(bytes.Skip(i * chunkSize).Take(chunkSize).OrderBy(x => x));
            }

            return list.ToArray();
        }
    }
}