using System;
using System.Text;

namespace AVS.CoreLib.Extensions.Reflection
{
    public static class TypeExtensions
    {
        public static Type? FindGenericType(this Type type, string name)
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
            var name = type.Name;
            if (name.Contains('`'))
                name = name.Substring(0, name.IndexOf('`'));

            var sb = new StringBuilder(name);
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

        public static bool IsNullable(this Type type)
        {
            if (type.IsValueType)
            {
                return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
            }

            return true;
        }
    }
}