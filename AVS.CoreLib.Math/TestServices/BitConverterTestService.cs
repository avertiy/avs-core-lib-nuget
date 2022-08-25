using AVS.Archivator.App.Common;
using AVS.CoreLib.Abstractions;
using AVS.CoreLib.PowerConsole;
using System;
using AVS.CoreLib.Math.Bytes;

namespace AVS.Archivator.App
{
	public class BitConverterTestService : ITestService
	{
		public void Test()
		{

			//idea 1 bit code 3 or 4 instead of 2
			//code 3 
			//0x1000 = 8 (BIN) => 3^3+0 = 27 (TRI)
			//0x1001 = 9 (BIN) => 3^3+0+0+1 = 28 (TRI)
			//0x1010 = 10 (BIN) => 3^3+0+3+0 = 30 (TRI)
			//0x1011 = 11 (BIN) => 3^3+0+3+1 = 31 (TRI)
			//0x1100 = 12 (BIN) => 3^3+3^2+0+0 = 36 (TRI)
			//0x1101 = 13 (BIN) => 3^3+3^2+0+1 = 37 (TRI)
			//0x1110 = 14 (BIN) => 3^3+3^2+3+0 = 39 (TRI)
			//0x1111 = 15 (BIN) => 3^3+3^2+3+1 = 40 (TRI)
			//0x1111 1111 = 255 (BIN) => 3^6 + ..+1 = 729 +243 + 81 + 27 + 9 + 3 + 1 = max 1093 (TRI) within a byte instead of 256
			//0x11 1111 1111 = 2^10 = 1024 (BIN)
			//
			//extension codes: 0 1 3 4 9 10 12 13 27 28 30 31 36 37 39 40
			//0 => 0000
			//1    0001
			//...
			//40   1111

			//how to represent missing numbers? - rest will be written as ext within a reserved bits
			// 29 = 0001 1101 (BIN) => 01001+001 (TRI) => 01 1101
			// 35 = 0001 1101 (BIN) => 01011+100 (TRI) => 01 1101
			// 80 => 81-1 => 
			//
			PowerConsole.PrintHeader("Idea: bits converter with base = 3");
			var numbers = new ushort[] { UInt16.MaxValue, 1, 9, 2, 27, 31, 40, 48, 64, 68, 80, 81, 255, 256, 512, 1024, 2048, 5555, 12500, 20000, 25500, 32800 };
			var b1 = BitArray.Parse("0x1111");
			var b2 = BitArray.Parse("0x11111");
			var b3 = BitArray.Parse("0x111111");
			PowerConsole.Print($"{b1} => {b1.ToUInt32(3)}");
			PowerConsole.Print($"{b2} => {b2.ToUInt32(3)}");
			PowerConsole.Print($"{b3} => {b3.ToUInt32(3)}");
			foreach (var number in numbers)
			{
				var bits = XBitConverter.GetBits(number);
				PowerConsole.Print($"{number} => {bits} => {bits.ToUInt32(3)}");
			}
		}
	}
}