using System;
using System.Linq;
using AVS.CoreLib.Math.Extensions;

namespace AVS.CoreLib.Math.Bytes
{
	public static class XBitConverter
	{
		public static BitArray GetBits(ushort n)
		{
			var max = 88573;
			var pows = new ushort[] { 1, 3, 9, 27, 81, 243, 729, 2187, 6561, 19683, 59049 }; //3^10
			var bitArray = new BitArray(11);
			var l = bitArray.Length - 1;
			if (n > max)
				throw new ArgumentOutOfRangeException();
			if (n < pows[0])
				return bitArray;
			var bit = n >= pows.Last() ? pows.Length - 1 : pows.TakeWhile(x => n > x).Count();

			bitArray[l - bit] = 1;
			var rest = n - pows[bit];
			if (rest == 0)
				return bitArray;

			var i = bit - 1;
			while (rest > 0 && i >= 0)
			{
				if (rest >= pows[i])
				{
					rest = rest - pows[i];
					bitArray[l - i] = 1;
				}
				i--;
			}

			//bitArray.Rest = rest;
			return bitArray;
		}

		public static byte[] ToUInt64(byte[] bytes, Func<byte[], bool> testFn, Func<byte[], int, byte[]> shiftBytesFn)
		{
			var i = 0;
			while (bytes.Length > 0)
			{
				if (testFn(bytes))
				{
					return bytes;
				}
				bytes = shiftBytesFn(bytes, i++);
			}

			return bytes;
		}

        public static byte CombineIntoByte(int halfByte1, int halfByte2, bool checkOverflow = true)
        {
            //not sure which is faster but the second approach as for me more clear what we do
            //var high = halfByte1 << 4;
            //var res = high1 | halfByte2;

            var high = halfByte1 * 16;
            var low = halfByte2;
            var res = high + low;

            if (checkOverflow && res > byte.MaxValue)
            {
                throw new OverflowException($"Operation contains overflow");
            }

            var result = Convert.ToByte(res);
            return result;
        }
    }
}