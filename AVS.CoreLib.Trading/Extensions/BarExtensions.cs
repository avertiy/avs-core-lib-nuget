using AVS.CoreLib.Trading.Abstractions;

namespace AVS.CoreLib.Trading.Extensions
{
    public static class BarExtensions
    {
        public static string ToString(this IBar bar, string format)
        {
            return format switch
            {
                "G" => $"{bar.Time:G}",
                "g" => $"{bar.Time:g}",
                "T" => $"{bar.Time:T}",
                "t" => $"{bar.Time:t}",
                _ => ((IOhlc)bar).ToString()
            };
        }
    }
}