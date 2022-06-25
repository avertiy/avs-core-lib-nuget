namespace AVS.CoreLib.Math.MathUtils.Sqrt
{
    public struct SqrtResult
    {
        public ulong Value;
        public ulong Rest;

        public SqrtResult(ulong value, ulong rest = 0)
        {
            Value = value;
            Rest = rest;
        }

        public int Size => ValueSize + RestSize;
        public int RestSize => Rest <= ushort.MaxValue ? 2 : Rest <= uint.MaxValue ? 4 : 8;
        public int ValueSize => Value <= ushort.MaxValue ? 2 : Value <= uint.MaxValue ? 4 : 8;

        public static implicit operator SqrtResult(ulong n) => new SqrtResult(n);
        public static implicit operator SqrtResult(uint n) => new SqrtResult(n);
        public static implicit operator SqrtResult(ushort n) => new SqrtResult(n);

        public override string ToString()
        {
            return $"Result [{Value};{Rest}]";
        }
    }
}