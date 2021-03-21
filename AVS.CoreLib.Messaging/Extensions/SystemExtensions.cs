using System;
using System.Text;

namespace AVS.CoreLib.Extensions
{
    public static class SystemExtensions
    {
        public static string ToStringNotation(this Type type)
        {
            if (!type.IsGenericType)
                return type.Name;

            var args = type.GetGenericArguments();
            var sb = new StringBuilder(type.Name);
            sb.Append("<");
            foreach (var typeArgument in args)
            {
                sb.Append(typeArgument.ToStringNotation());
                sb.Append(",");
            }

            sb.Length--;
            sb.Append(">");
            return sb.ToString();
        }
    }
}
