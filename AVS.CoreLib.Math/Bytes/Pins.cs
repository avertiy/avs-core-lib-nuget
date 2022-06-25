using System;
using System.Collections;
using System.Collections.Generic;
using AVS.CoreLib.Math.Extensions;

namespace AVS.CoreLib.Math.Bytes
{
	public class Pins : IEnumerable<int>
	{
		private readonly int[] _items;
		public readonly bool IsOrdered;
		public int Count => _items.Length;

		public int Counter { get; set; }

		public Pins(int n, bool isOrdered = false)
		{
			IsOrdered = isOrdered;
			_items = new int[n];
		}

		public Pins(int[] pins, bool isOrdered = false)
		{
			_items = pins;
			IsOrdered = isOrdered;
		}

		public int this[int i]
		{
			get => _items[i];
			set
			{
				if (i > 0 && IsOrdered && value < _items[i - 1])
				{
					throw new ArgumentException($"Pin[{i}-1]={_items[i - 1]} is greater than {value}");
				}

				Counter++;
				_items[i] = value;
			}
		}

		public T[] Pin<T>(T[] source)
		{
			var arr = new T[Count];
			for (var i = 0; i < Count; i++)
			{
				var ind = _items[i];
				arr[i] = source[ind];
			}
			return arr;
		}

		public override string ToString()
		{
			return $"{_items.AsArrayString()}";
		}

		public IEnumerator<int> GetEnumerator()
		{
			return ((IEnumerable<int>)_items).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public static implicit operator Pins(int[] pins)
		{
			return new Pins(pins);
		}

		public static implicit operator int[](Pins pins)
		{
			return pins._items;
		}
	}

	public static class PinExtensions
	{
		public static T[] Pin<T>(T[] source, params int[] pins)
		{
			var arr = new T[pins.Length];
			for (var i = 0; i < pins.Length; i++)
			{
				var ind = pins[i];
				arr[i] = source[ind];
			}
			return arr;
		}
	}
}