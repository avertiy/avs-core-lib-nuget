using System;

namespace AVS.CoreLib.Extensions
{
    public static class TupleExtensions
    {
        public static decimal Smallest(this (decimal, decimal) tuple)
        {
            return tuple.Item1 <= tuple.Item2 ? tuple.Item1 : tuple.Item2;
        }

        public static decimal Smallest(this (decimal, decimal, decimal) tuple)
        {
            if (tuple.Item1 <= tuple.Item2 && tuple.Item1 <= tuple.Item3)
                return tuple.Item1;
            if (tuple.Item2 <= tuple.Item1 && tuple.Item2 <= tuple.Item3)
                return tuple.Item2;
            return tuple.Item3;
        }

        public static int Smallest(this (int, int) tuple)
        {
            return tuple.Item1 <= tuple.Item2 ? tuple.Item1 : tuple.Item2;
        }

        public static int Smallest(this (int, int, int) tuple)
        {
            if (tuple.Item1 <= tuple.Item2 && tuple.Item1 <= tuple.Item3)
                return tuple.Item1;
            if (tuple.Item2 <= tuple.Item1 && tuple.Item2 <= tuple.Item3)
                return tuple.Item2;
            return tuple.Item3;
        }

        public static decimal Greatest(this (decimal, decimal) tuple)
        {
            return tuple.Item1 < tuple.Item2 ? tuple.Item2 : tuple.Item1;
        }

        public static int Greatest(this (int, int) tuple)
        {
            return tuple.Item1 < tuple.Item2 ? tuple.Item2 : tuple.Item1;
        }

        
    }
}
