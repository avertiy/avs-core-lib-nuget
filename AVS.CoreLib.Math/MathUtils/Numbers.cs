using System;

namespace AVS.CoreLib.Math.MathUtils
{
    public static class Numbers
    {
        public static void PrintNumberTypes()
        {
            Console.WriteLine($"Int16 ({sizeof(short)}b): {short.MinValue} - {short.MaxValue}");
            Console.WriteLine($"Int32 ({sizeof(int)}b): {int.MinValue} - {int.MaxValue}");
            Console.WriteLine($"UInt32 ({sizeof(uint)}b): {uint.MinValue} - {uint.MaxValue}");
            Console.WriteLine($"Int64 ({sizeof(long)}b): {long.MinValue} - {long.MaxValue}");
            Console.WriteLine($"UInt64 ({sizeof(ulong)}b): {ulong.MinValue} - {ulong.MaxValue}");
            Console.WriteLine($"float ({sizeof(float)}b): {float.MinValue} - {float.MaxValue}  epsilon {float.Epsilon}");
            Console.WriteLine($"double ({sizeof(double)}b): {double.MinValue} - {double.MaxValue:N50}  epsilon {double.Epsilon}");
            Console.WriteLine($"decimal ({sizeof(decimal)}b): {decimal.MinValue} - {decimal.MaxValue}");
        }
    }


}