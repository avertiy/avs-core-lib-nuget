using System;

namespace AVS.CoreLib.Math.MathUtils
{
	public static class Checked
	{
		public static bool TryChecked<T>(Func<T> func, out T result)
		{
			try
			{
				checked
				{
					result = func();
					return false;
				}
			}
			catch (OverflowException)
			{
				result = default;
				return true;
			}
		}
	}
}