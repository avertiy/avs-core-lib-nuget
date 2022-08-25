using AVS.CoreLib.Abstractions;
using AVS.CoreLib.Math.MathUtils.Fractions;
using AVS.CoreLib.Math.MathUtils.Sqrt;
using AVS.CoreLib.PowerConsole;

namespace AVS.Archivator.App
{
	public class SqrtTestService : ITestService
	{
		public void Test()
		{
			PowerConsole.PrintHeader("Sqrt tests");
			var f = new Fraction(9, 1);
			var sqrt = f.Sqrt();
			PowerConsole.Print($"sqrt({f}) => {sqrt}");

			f = new Fraction(9, 2);
			sqrt = f.Sqrt();
			PowerConsole.Print($"sqrt({f}) => {sqrt}");

			f = new Fraction(1, 9);
			sqrt = f.Sqrt();
			PowerConsole.Print($"sqrt({f}) => {sqrt}");

			f = new Fraction(2, 9);
			sqrt = f.Sqrt();
			PowerConsole.Print($"sqrt({f}) => {sqrt}");

			f = new Fraction(short.MaxValue + 1, 2);
			sqrt = f.Sqrt();
			PowerConsole.Print($"sqrt({f}) => {sqrt}");

			f = new Fraction(int.MaxValue + 1L, 2);
			sqrt = f.Sqrt();
			PowerConsole.Print($"sqrt({f}) => {sqrt}");

			PowerConsole.PrintHeader("Sqrt expression tests");
			f = new Fraction(2 * 9 * 16);
			var expr = SqrtExpression.Create(f);
			PowerConsole.Print($"sqrt({f}) => {expr}");
			f = new Fraction(-2 * 9 * 16);
			expr = SqrtExpression.Create(f);
			PowerConsole.Print($"sqrt({f}) => {expr}");

			f = new Fraction((long)121211 * 212115);
			expr = SqrtExpression.Create(f);
			PowerConsole.Print($"sqrt({f}) => {expr}");
		}
	}
}