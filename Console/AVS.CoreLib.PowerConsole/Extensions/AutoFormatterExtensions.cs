using AVS.CoreLib.Extensions.AutoFormatters;

namespace AVS.CoreLib.PowerConsole.Extensions
{
    public static class AutoFormatterExtensions
    {
        public static IAutoFormatter AddOptionsFormatter(this IAutoFormatter formatter)
        {
            formatter.TryAddFormatterByKeyword<object>("options", x => x.ToJsonString());
            return formatter;
        }
    }
}