#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
#pragma warning disable CS1591 
namespace AVS.CoreLib.Abstractions.Rest
{
    public interface IParametersCollection : IDictionary<string, object>
    {
    }

    public static class ParametersCollectionExtensions
    {
        public static bool AddOptional(this IParametersCollection parameters, string key, string? value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            parameters.Add(key, value);
            return true;
        }

        public static bool AddOptional(this IParametersCollection parameters, string key, int value)
        {
            parameters.Add(key, value);
            return true;
        }

        public static void AddRequired(this IParametersCollection parameters, string key, string? value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException($"`{key}` value is mandatory");
            parameters.Add(key, value!);
        }

        public static bool AddOptionalEnum<T>(this IParametersCollection parameters, string key, T? value, Func<T, string> converter) where T : Enum
        {
            if (value == null)
                return false;

            var str = converter(value);

            if (string.IsNullOrEmpty(str))
                return false;

            parameters.Add(key, str);
            return true;
        }

        public static void AddRequired<T>(this IParametersCollection parameters, string key, T value) where T : struct
        {
            if (value.Equals(default))
                throw new ArgumentException($"`{key}` value is mandatory");
            parameters.Add(key, value);
        }

        public static void AddRequired(this IParametersCollection parameters, string key, int value)
        {
            if (value == 0)
                throw new ArgumentException($"`{key}` value is mandatory and must be not 0");
            parameters.Add(key, value);
        }

        public static bool AddOptionalLimit(this IDictionary<string, object> dictionary, string key, int limit, params int[] validValues)
        {
            if (limit == 0)
                return false;

            if (!validValues.Contains(limit))
                throw new ArgumentOutOfRangeException($"Limit `{limit}` must be one of [{string.Join(", ", validValues)}]");

            dictionary.Add(key, limit);
            return true;
        }
    }
}
#pragma warning restore CS1591