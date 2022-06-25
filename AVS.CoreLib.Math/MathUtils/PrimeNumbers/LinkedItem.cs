namespace AVS.CoreLib.Math.MathUtils.PrimeNumbers
{
    public class LinkedItem<T> where T : struct
    {
        public T Item { get; set; }
        public LinkedItem<T> Prev { get; set; }
        public LinkedItem<T> Next { get; set; }
        private LinkedItem(T item)
        {
            Item = item;
        }
    }
}