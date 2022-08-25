using AVS.Archivator.App.Common;
using AVS.Archivator.App.Extensions;
using AVS.CoreLib.Abstractions;
using AVS.CoreLib.PowerConsole;
using System;
using AVS.CoreLib.Math.Bytes.Extensions;
using AVS.CoreLib.Math.Geometry;

namespace AVS.Archivator.App
{
	public class LineEquationTestService : ITestService
	{
		public void Test()
		{

			//idea convert input bytes into integers, combine them into points and try to represent it as line equations
			PowerConsole.PrintHeader("Idea: bytes => integers => points => line equations");
			var bytes = Generator.GetRandomBytes(24);
			var integers = bytes.ToUInt32Array();
			//integers = new [] {10000, -20025, 30003,400400,505000,606000};
			PowerConsole.Print($"{bytes.AsArrayString()} => {integers.AsArrayString()}");
			try
			{
				var ab = new LineEquation(integers[0], integers[1], integers[2], integers[3]);
				var ac = new LineEquation(integers[0], integers[1], integers[4], integers[5]);
				var bc = new LineEquation(integers[2], integers[3], integers[4], integers[5]);
				PowerConsole.Print($"AB: {ab}; AC: {ac}; BC: {bc}; ");
				var y = ab.CalcY(integers[0]);
				PowerConsole.Print($"AB(x)= {ab.CalcY(integers[0]).Value}");
				PowerConsole.Print($"AC(x)= {ac.CalcY(integers[2]).Value}");
				PowerConsole.Print($"BC(x)= {bc.CalcY(integers[4]).Value}");
			}
			catch (Exception ex)
			{
				PowerConsole.WriteError(ex);
			}
		}
	}
}