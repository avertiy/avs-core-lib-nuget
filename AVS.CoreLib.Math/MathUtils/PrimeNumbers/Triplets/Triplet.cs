using System;
using AVS.CoreLib.Math.Extensions;
using AVS.CoreLib.Math.MathUtils.PrimeNumbers.Structs;

namespace AVS.CoreLib.Math.MathUtils.PrimeNumbers.Triplets
{
    public enum TripletType
    {
        TwinFirst,
        TwinLast
    }

    public class Triplet
    {
        public Prime P1 { get; }
        public Prime P2 { get; }
        public Prime P3 { get; }

        public int D1 => Convert.ToInt32(P1.N % 10);
        public int D2 => Convert.ToInt32(P2.N % 10);
        public int D3 => Convert.ToInt32(P3.N % 10);
        public int Sum => D1 + D2 + D3;

        public TripletType Type => P1.N + 2 == P2.N ? TripletType.TwinFirst : TripletType.TwinLast;

        public Triplet Prev;
        public Triplet Next;

        public int? PrevD1 => Prev?.D1;
        public int? Prev2D1 => Prev?.Prev?.D1;

        public int? NextD1 => Next?.D1;
        public int? Next2D1 => Next?.Next?.D1;

        private Triplet(Prime p1, Prime p2, Prime p3)
        {
            P1 = p1;
            P2 = p2;
            P3 = p3;
        }

        public static Triplet Create(Prime p1, Prime p2, Prime p3)
        {
            if (!IsValid(p1, p2, p3))
            {
                throw new ArgumentException("invalid triplet");
            }

            return new Triplet(p1, p2, p3);
        }

        private static bool IsValid(ulong n1, ulong n2, ulong n3)
        {
            return n1.IsPrimeTriplet(n2, n3);
        }


        public static implicit operator Prime(Triplet triplet)
        {
            return triplet.P1;
        }

        public override string ToString()
        {
            return $"({P1},{P2},{P3})";
        }
    }
}