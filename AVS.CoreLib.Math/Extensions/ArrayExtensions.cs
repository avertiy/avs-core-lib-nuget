namespace AVS.CoreLib.Math.Extensions
{
	public static class ArrayExtensions
	{
		public static int ComputeSum(this int[] numbers)
		{
			int sum = 0;
			checked
			{
				foreach (var n in numbers)
				{
					sum += n;
				}
			}
			return sum;
		}

		public static string AsArrayString(this int[] arr, string separator=",")
		{
			return $"[{string.Join(separator, arr)}]";
		}
	}
}