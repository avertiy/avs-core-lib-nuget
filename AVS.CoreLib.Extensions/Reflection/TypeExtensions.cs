using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        
        public static string GetReadableName(this Type type, bool fullNamePreferably = false)
        {
            var name = type.Name;
            if (fullNamePreferably && type.FullName.Length < 80)
                name = type.FullName;

            if (!type.IsGenericType)
                return name;

            var args = type.GetGenericArguments();
            return ToStringNotation(type.Name, args);
        }

        /// <summary>
        /// For generics return a type name in a human friendly format e.g. List&lt;MyClass&gt;
        /// </summary>
        /// <param name="type"></param>
        [Obsolete("use GetReadableName()")]
        public static string ToStringNotation(this Type type)
        {
            if (!type.IsGenericType)
                return type.Name;

            var args = type.GetGenericArguments();
            return ToStringNotation(type.Name, args);
        }

        private static string ToStringNotation(string typeName, Type[] args)
        {
            typeName = typeName.TrimStart('<', '>');
            if (typeName.Contains('`'))
                typeName = typeName.Substring(0, typeName.IndexOf('`'));

            var sb = new StringBuilder(typeName);
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