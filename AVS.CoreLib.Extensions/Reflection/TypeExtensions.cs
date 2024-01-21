using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using AVS.CoreLib.Extensions.Attributes;

namespace AVS.CoreLib.Extensions.Reflection
{
    public static class TypeExtensions
    {
        /// <summary>
        /// reflect object properties and its values to dictionary
        /// </summary>
        /// <param name="type">Type to reflect properties</param>
        /// <param name="obj">obj</param>
        /// <param name="predicate">optional predicate to filter props</param>
        /// <returns>dictionary (keys - property names; values - property values)</returns>
        public static Dictionary<string, object> Reflect(this Type type, object obj, Func<string, object, bool>? predicate = null)
        {
            var dict = new Dictionary<string, object>();

            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var propertyInfo in props)
            {
                if (!propertyInfo.CanRead)
                    continue;

                var attrImp = propertyInfo.GetCustomAttribute<ImportantAttribute>();

                var attrIgnore = propertyInfo.GetCustomAttribute<IgnoreAttribute>();
                if (attrImp == null && attrIgnore != null)
                    continue;

                var value = propertyInfo.GetValue(obj);

                if(attrImp == null && predicate != null && !predicate.Invoke(propertyInfo.Name, value))
                    continue;

                dict.Add(propertyInfo.Name, value);
            }

            return dict;
        }

        public static string GetDisplayName(this PropertyInfo propertyInfo)
        {
            var attr = propertyInfo.GetCustomAttribute<DisplayNameAttribute>();
            if (attr != null)
            {
                return attr.DisplayName;
            }

            return propertyInfo.Name;
        }

        /// <summary>
        /// reflect object properties and its type names to dictionary
        /// </summary>
        public static Dictionary<string, string> ReflectProperties(this Type type, object obj)
        {
            var dict = new Dictionary<string, string>();

            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var propertyInfo in props)
            {
                var key = propertyInfo.Name;
                var value = propertyInfo.PropertyType.GetReadableName();
                dict.Add(key, value);
            }

            return dict;
        }

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

        public static string GetTypeName(this object obj, int? maxLength = null, bool fullNamePreferably = false)
        {
            var type = obj.GetType();
            var name = type.Name;
            if (fullNamePreferably && type.FullName?.Length < 80)
                name = type.FullName;

            if (!type.IsGenericType)
                return name;

            var args = type.GetGenericArguments();
            var limitArgs = maxLength == null ? 4 : (int?)null;
            return ToStringNotation(type.Name, args, limitArgs, maxLength);
        }

        public static string GetReadableName(this Type type, int? maxLength = null, bool fullNamePreferably = false)
        {
            var name = type.Name;
            if (fullNamePreferably && type.FullName?.Length < 80)
                name = type.FullName;

            if (!type.IsGenericType)
                return name;

            var args = type.GetGenericArguments();
            var limitArgs = maxLength == null ? 4 : (int?)null;
            return ToStringNotation(type.Name, args, limitArgs, maxLength);
        }

        private static string ToStringNotation(string typeName, Type[] args, int? limitArgs, int? maxLength = null)
        {
            typeName = typeName.TrimStart('<', '>');
            if (typeName.Contains('`'))
                typeName = typeName.Substring(0, typeName.IndexOf('`'));

            var sb = new StringBuilder(typeName);
            sb.Append("<");
            for (var i = 0; i < args.Length; i++)
            {
                if (limitArgs.HasValue && i == limitArgs)
                {
                    sb.Append("...,");
                    break;
                }

                var typeArgument = args[i];
                sb.Append(typeArgument.GetReadableName());

                if (maxLength.HasValue && sb.Length > maxLength - 4)
                {
                    sb.Append("...,");
                    break;
                }

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

        /// <summary>
        /// Determines if type is simple
        /// simply type is either  primitive, string, decimal or any of those wrapped in Nullable
        /// </summary>
        public static bool IsSimpleType(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                // nullable type, check if the nested type is simple.
                return IsSimpleType(type.GetGenericArguments()[0]);
            return type.IsPrimitive
                   || type.IsEnum
                   || type == typeof(string)
                   || type == typeof(decimal);
        }
    }
}