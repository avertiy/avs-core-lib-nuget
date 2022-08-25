using AVS.CoreLib.Abstractions;
using AVS.CoreLib.PowerConsole;
using System;
using AVS.CoreLib.Math.MathUtils.Fractions;
using AVS.CoreLib.Math.MathUtils.Polinoms;

namespace AVS.Archivator.App
{
	public class PolynomTestService : ITestService
	{
		public void Test()
		{
			PowerConsole.PrintHeader("Polynom tests");
			Polynom.TryParse("x^3 + x = 10/27", out Polynom p);
			p = p.Optimize();
			Console.WriteLine(p.ToString());
			var t = p.TestRoot(new Fraction(1, 3));
			//p.TestRoot(new Fraction(-255, 131));
			var roots = p.FindRoots();
			Console.WriteLine($"roots: {string.Join(";", roots)}");
		}
	}
}