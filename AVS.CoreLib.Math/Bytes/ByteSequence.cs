using System.Collections;
using System.Collections.Generic;
using AVS.CoreLib.Math.Bytes.Extensions;

namespace AVS.CoreLib.Math.Bytes
{
	public class ByteSequence : IEnumerable<byte>
	{
		public List<byte> Bytes { get; set; } = new List<byte>();
		public ByteSequence()
		{
		}

		public ByteSequence(params byte[] bytes)
		{
			Bytes.AddRange(bytes);
		}

		public void Insert(int index, byte @byte)
		{
			Bytes.Insert(index, @byte);
		}

		public void Add(params byte[] bytes)
		{
			Bytes.AddRange(bytes);
		}

		public void Insert(int index, ByteSequence sequence)
		{
			Bytes.InsertRange(index, sequence.Bytes);
		}

		public override string ToString()
		{
			return $"{Bytes.AsBytesString()}";
		}

		public IEnumerator<byte> GetEnumerator()
		{
			return Bytes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public byte[] ToArray() => Bytes.ToArray();
	}
}