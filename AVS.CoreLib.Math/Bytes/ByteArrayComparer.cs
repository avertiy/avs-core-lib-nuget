using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AVS.CoreLib.Math.Bytes
{
    public class ByteArrayComparer : IEqualityComparer<byte[]>
    {
        public bool IgnoreOrder { get; set; }
        public bool Equals(byte[] x, byte[] y)
        {
            var hashset = new HashSet<byte>(x ?? Array.Empty<byte>());
            return hashset.SetEquals(y ?? Array.Empty<byte>());
        }

        public int GetHashCode(byte[] obj)
        {
            return new BigInteger(IgnoreOrder ? obj.OrderBy(x => x).ToArray() : obj).GetHashCode();
        }
    }
}