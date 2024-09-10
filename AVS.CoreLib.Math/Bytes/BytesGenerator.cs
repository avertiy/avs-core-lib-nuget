using System;
using System.Collections.Generic;
using System.Linq;

namespace AVS.CoreLib.Math.Bytes
{
    public static class BytesGenerator
    {
        private static byte[] _allBytes;

        public static byte[] AllBytes
        {
            get
            {
                if (_allBytes == null)
                {
                    _allBytes = new byte[256];
                    for (var i = 1; i <= 255; i++)
                    {
                        _allBytes[i] = (byte)i;
                    }
                }

                return _allBytes;
            }
        }

        public static byte[] GetFirstBytes(int count)
        {
            if (count < 1 || count > 256)
                throw new ArgumentOutOfRangeException($"{nameof(count)} is out of range [1;256]");
            var bytes = new byte[count];
            ;
            for (var i = 0; i < count; i++)
            {
                bytes[i] = (byte)i;
            }

            return bytes;
        }

        public static IEnumerable<byte[]> GenerateUniqueByteSequences(int count, int sum, bool ignoreOrder)
        {
            var sequences = GenerateUniqueByteSequences(count, sum);
            return sequences.Distinct(new ByteArrayComparer() { IgnoreOrder = ignoreOrder });
        }

        public static IEnumerable<byte[]> GenerateUniqueByteSequences(int count, int sum)
        {
            var max = 255;
            if (sum > count * max)
            {
                yield return Array.Empty<byte>();
            }
            else if (count <= 1)
            {
                yield return new[] { (byte)sum };
            }
            else
            {
                for (var i = 0; i <= max && i <= sum; i++)
                {
                    var count2 = count - 1;
                    var sum2 = sum - i;

                    // check whether i is ok max be first element
                    if (count2 * max >= sum2)
                    {
                        foreach (var subSequence in GenerateSequencesInternal(count2, sum2, i + 1, max))
                        {
                            if (subSequence.Length == 0)
                                continue;

                            if (subSequence.Any(x => x == i))
                                continue;

                            var sequence = new List<byte>(count) { (byte)i };
                            sequence.AddRange(subSequence);
                            var arr = sequence.ToArray();
                            yield return arr;
                        }
                    }
                }
            }
        }

        private static IEnumerable<byte[]> GenerateSequencesInternal(int count, int sum, int from, int max)
        {
            if (count == 0 || sum > count * max)
            {
                yield return Array.Empty<byte>();
            }
            else if (count == 1)
            {
                yield return new[] { (byte)sum };
            }
            else
            {
                var count2 = count - 1;
                if (count2 == 1)
                {
                    for (var i = from; i <= max && i <= sum; i++)
                    {
                        var sum2 = sum - i;
                        if (sum2 <= max && sum2 != i)
                        {
                            yield return new[] { (byte)i, (byte)sum2 };
                        }
                    }
                }

                for (var i = from; i <= max && i <= sum; i++)
                {
                    var sum2 = sum - i;
                    foreach (var subSequence in GenerateSequencesInternal(count2, sum2, i + 1, max))
                    {
                        if (subSequence.Length == 0)
                            continue;

                        if (subSequence.Any(x => x == i))
                            continue;

                        var sequence = new List<byte>(count) { (byte)i };
                        sequence.AddRange(subSequence);
                        var arr = sequence.ToArray();
                        yield return arr;
                    }
                }
            }
        }
    }
}