using System;
using System.Text;

namespace AVS.CoreLib.Extensions
{
    public static class SystemExtensions
    {
        public static string AsString(this string[] arr, string separator = ", ")
        {
            return string.Join(separator, arr);
        }

        public static Type FindGenericType(this Type type, string name)
        {
            if (type.IsGenericType)
            {
                if (type.Name.StartsWith(name))
                    return type;
                if (type.BaseType != null)
                    return type.BaseType.FindGenericType(name);
            }
            return null;
        }

        public static string ToStringNotation(this Type type)
        {
            if (!type.IsGenericType)
                return type.Name;

            var args = type.GetGenericArguments();
            StringBuilder sb = new StringBuilder(type.Name);
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
