#nullable enable
using System;
using System.Collections.Generic;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.Guards;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace AVS.CoreLib.REST.Models
{
    public class ParametersCollection : Dictionary<string, object>, IParametersCollection
    {
        public bool AddOptional<T>(string key, T? value)
        {
            if (value == null)
                return false;

            this.Add(key, value);
            return true;
        }

        public bool AddOptional(string key, string? value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            this.Add(key, value);
            return true;
        }
        
        public bool AddOptionalEnum<T>(string key, T? value, Func<T, string> converter) where T :Enum
        {
            if (value == null)
                return false;

            var str = converter(value);

            if (string.IsNullOrEmpty(str))
                return false;

            this.Add(key, str);
            return true;
        }

        public void AddRequired<T>(string key, T value) where T : struct
        {
            Guard.Against.Default(value);
            this.Add(key, value);
        }

        public void AddRequired(string key, string? value)
        {
            Guard.Against.NullOrEmpty(value, $"`{key}` value is mandatory");
            this.Add(key, value!);
        }

        public void AddRequired(string key, int value)
        {
            Guard.Against.Zero(value, $"`{key}` value is mandatory");
            this.Add(key, value);
        }
    }
}

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member