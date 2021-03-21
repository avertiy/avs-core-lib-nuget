using System.Globalization;

namespace AVS.CoreLib.REST.Extensions
{
    public static class Extensions
    {
        public static string ToStringHex(this byte[] value)
        {
            var output = string.Empty;
            for (var i = 0; i < value.Length; i++)
            {
                output += value[i].ToString("x2", CultureInfo.InvariantCulture);
            }
            return (output);
        }
    }
}