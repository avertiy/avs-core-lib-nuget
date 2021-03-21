namespace AVS.CoreLib.Trading.Structs
{
    public struct Aggregate
    {
        public decimal Avg;
        public decimal Min;
        public decimal Max;
    }

    public struct Aggregate<T>
    {
        public T Avg;
        public T Min;
        public T Max;
    }
}