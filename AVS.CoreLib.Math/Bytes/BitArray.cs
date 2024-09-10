using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AVS.CoreLib.Math.Bytes
{
    public class BitArray
    {
        public int[] Bits { get; private set; }
        public int Length
        {
            get => Bits.Length;
            set
            {
                if (value < Bits.Length)
                {
                    Bits = Bits.Take(value).ToArray();
                }
                else if (value > Bits.Length)
                {
                    throw new ArgumentOutOfRangeException($"BitArray length could not be extended.");
                }
            }
        }

        public int BytesCount => Length / 8 + (Length % 8 == 0 ? 0 : 1);

        public BitArray(int size)
        {
            Bits = new int[size];
        }

        public BitArray(int[] bits)
        {
            Bits = bits;
        }

        public int this[int i]
        {
            get => Bits[i];
            set
            {
                if (value != 1 && value != 0)
                    throw new InvalidOperationException();

                Bits[i] = value;
            }
        }

        public void SetBit(int index, bool condition)
        {
            this[index] = condition ? 1 : 0;
        }

        public void ResetBits(int from = 0)
        {
            for (var i = from; i < Bits.Length; i++)
            {
                Bits[i] = 0;
            }
        }

        public override string ToString()
        {
            return $"0x{string.Join("", Bits)}";
        }

        public static implicit operator BitArray(string str)
        {
            return BitArray.Parse(str);
        }

        public static implicit operator BitArray(int[] arr)
        {
            return arr.ToBitArray(true);
        }

        public static BitArray Parse(string str)
        {
            if (!Regex.IsMatch(str, "(0x)?[01]+"))
            {
                throw new ArgumentException(nameof(str));
            }

            var input = str.Replace("0x", "");
            var bitArray = new BitArray(input.Length);
            var i = 0;
            foreach (var c in input.ToCharArray())
            {
                if (c == '1')
                    bitArray[i] = 1;
                i++;
            }

            return bitArray;
        }

        public static BitArray FromBytes(params byte[] bytes)
        {
            return bytes.ToBitArray();
        }

        public static BitArray FromInt(int value, int size)
        {
            if (value < 0)
                throw new ArgumentException($"The value {value} must be positive number");

            var arr = new BitArray(size);

            //37 into 4 bits => 18%1 9%0 4%1 2%0 | 1%0 %1 => [100101] => [..0101]

            //12: 12/2 = 6%0 => 3%0 => 1 % 1 => %1 => [1100]
            //5:  5/2 =  2%1 => 1:0 => %1 => [..101]
            var i = 1;
            while (value > 0 && i <= size)
            {
                arr[^i++] = value % 2;
                value = value / 2;
            }

            return arr;
        }

        public BitArray Concat(BitArray other)
        {
            var arr = new BitArray(this.Length + other.Length);
            for (var i = 0; i < this.Length; i++)
            {
                arr[i] = this[i];
            }

            for (var i = Length; i < arr.Length; i++)
            {
                arr[i] = other[i - Length];
            }

            return arr;
        }
    }

    public static class BitArrayExtensions
    {
        public static byte[] ToByteArray(this BitArray arr)
        {
            var bytes = new List<byte>(arr.Length / 8 + 1);
            byte b = 0;
            var i = 0;
            var n = 0;
            while (true)
            {
                if (i > 0 && i % 8 == 0)
                {
                    bytes.Add(b);
                    n = 0;
                    b = 0;
                }

                if (i == arr.Length)
                {
                    if (i % 8 > 0)
                    {
                        bytes.Add(b);
                    }
                    break;
                }

                var bit = arr[i];
                if (bit == 1)
                {
                    b += (byte)Pow(2, n);
                }
                i++;
                n++;
            }

            var result = bytes.ToArray();
            if (result.Length != arr.BytesCount)
            {
                throw new Exception("something wrong with bitarray");
            }

            return result;
        }

        public static BitArray ToBitArray(this byte[] bytes)
        {
            var arr = new BitArray(bytes.Length * 8);
            for (int i = 0; i < arr.Length; ++i)
            {
                int bit = ((bytes[i / 8] >> (i % 8)) & 0x01);
                arr[i] = bit;
            }

            return arr;
        }

        public static BitArray ToBitArray(this int[] bits, bool forceCheckBits = true)
        {
            if (!forceCheckBits)
                return new BitArray(bits);
            var bitArray = new BitArray(bits.Length);
            for (var i = 0; i < bits.Length; i++)
            {
                var bit = bits[i];
                bitArray[i] = bit == 0 ? 0 : 1;
            }

            return bitArray;
        }

        public static uint ToUInt32(this BitArray arr, int @base = 2)
        {
            uint res = 0;
            var n = arr.Length - 1;
            for (var i = 0; i < arr.Bits.Length; i++)
            {
                var bit = arr.Bits[i];
                if (bit == 1)
                {
                    res += Pow(@base, n - i);
                }

            }

            return res;
        }

        private static uint Pow(int n, int pow)
        {
            if (pow == 0)
                return 1;
            return (uint)n * Pow(n, pow - 1);
        }
    }
}