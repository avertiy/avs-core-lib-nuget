using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Stringify;

namespace AVS.CoreLib.Guards
{
    public static class Guard
    {
        #region Array guards
        /// <summary>
        /// check array index, it should be in range [0,arr.Length] 
        /// </summary>
        public static void ArrayIndex<T>(T[] arr, int index, string? message = null)
        {
            if (index < 0 || index >= arr.Length)
                throw new ArgumentOutOfRangeException(message ?? $"[{index}] should be within range [0; {arr.Length-1}]");
        }

        public static void ArrayLength<T>(T[] arr, int length, string? message = null)
        {
            if (arr.Length != length)
                throw new ArgumentOutOfRangeException(message ?? $"{length} element(s) expected");
        }

        public static void ArrayLength<T>(IList<T> arr, int length, string? message = null)
        {
            if (arr.Count != length)
                throw new ArgumentOutOfRangeException(message ?? $"{length} element(s) expected");
        }

        public static void ArrayMinLength<T>(T[] arr, int minLength, string? message = null)
        {
            if (arr.Length < minLength)
                throw new ArgumentOutOfRangeException(message?? $"at least {minLength} element(s) required");
        }

        public static void ArrayMinLength<T>(IList<T> arr, int minLength, string? message = null)
        {
            if (arr.Count < minLength)
                throw new ArgumentOutOfRangeException(message ?? $"At least {minLength} element(s) required");
        }

        public static void ArrayMaxLength<T>(T[] arr, int maxLength, string? message = null)
        {
            if (arr.Length > maxLength)
                throw new ArgumentOutOfRangeException(message ?? $"Max {maxLength} element(s) allowed");
        }

        public static void ArrayMaxLength<T>(IList<T> arr, int maxLength, string? message = null)
        {
            if (arr.Count > maxLength)
                throw new ArgumentOutOfRangeException(message ?? $"Max {maxLength} element(s) allowed");
        }

        #endregion

        #region IDictionary

        public static void MustContainKey<TKey, TValue>(IDictionary<TKey, TValue> dict, TKey key, string name = "dictionary")
        {
            if (!dict.ContainsKey(key))
                throw new ArgumentException($"{name} must contain key {key}");
        }

        /// <summary>
        /// validates dictionary contains only specified (valid/supported) keys 
        /// </summary>
        public static void ValidKeys<TKey, TValue>(IDictionary<TKey, TValue> dict, TKey[] validKeys, string name = "dictionary")
        {
            foreach (var key in dict.Keys)
                if (!validKeys.Contains(key))
                    throw new ArgumentException($"{name} contains invalid key: {key}");
        }

        /// <summary>
        /// validates dictionary contains only specified (supported) keys 
        /// </summary>
        public static void SupportedKeys<TKey, TValue>(IDictionary<TKey, TValue> dict, TKey[] supportedKeys, string name = "dictionary")
        {
            foreach (var key in dict.Keys)
                if (!supportedKeys.Contains(key))
                    throw new ArgumentException($"{name} contains not supported key: {key}");
        }

        public static void MustContainKeys<TKey, TValue>(IDictionary<TKey, TValue> dict, params TKey[] keys)
        {
            foreach (var key in keys)
                if (!dict.ContainsKey(key))
                    throw new ArgumentException($"Must contain key {key}");
        }
        #endregion

        #region IList

        public static void ListMustHaveAtLeast<T>(IList<T> list, int itemsCount, string name = "list")
        {
            if (list == null)
                throw new ArgumentNullException($"{name} must be not null");

            if (list.Count < itemsCount)
                throw new ArgumentOutOfRangeException($"{name} must have at least #{itemsCount} items");
        }

        public static void ListIndex<T>(IList<T> list, int index, string name = "index")
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException($"{name} [{index}] must be positive number");

            if (index > list.Count)
                throw new ArgumentOutOfRangeException($"{name} [{index}] must not exceed {list.Count}");
        }

        #endregion

        public static void AgainstNullOrEmpty(string? param, string name = "argument")
        {
            if (string.IsNullOrEmpty(param))
                throw new ArgumentNullException($"{name} must be not null or empty");
        }

        public static void AgainstNullOrEmpty(string? param, bool allowNull, string name = "argument")
        {
            if (string.IsNullOrEmpty(param) && !allowNull)
                throw new ArgumentNullException($"{name} must be not null or empty");
        }

        public static void AgainstNull(object? param, string name = "argument")
        {
            if (param == null)
                throw new ArgumentNullException($"{name} must be not null");
        }

        public static void AgainstNull(object? param, bool allowNull, string name = "argument")
        {
            if (param == null && !allowNull)
                throw new ArgumentNullException($"{name} must be not null");
        }

        public static void AgainstDateTimeMin(DateTime param, string name = "argument")
        {
            if (param == DateTime.MinValue)
                throw new ArgumentNullException($"{name} must be not a DateTime.MinValue");
        }

        public static void MustBeEqual(string? str1, string? str2, string? message = null)
        {
            if (str1 == null || str2 == null || !str1.Equals(str2))
                throw new ArgumentException(message ?? $"'{str1}' expected to be equal '{str2}'");
        }

        public static void MustBeEqual(double value, double valueToCompare, double tolerance = 0.001, string name = "argument")
        {
            if (!value.IsEqual(valueToCompare, tolerance))
                throw new ArgumentException($"{name} must be equal to {valueToCompare}");
        }

        public static void MustBeEqual(decimal value, decimal valueToCompare, decimal tolerance = 0.001m, string name = "argument")
        {
            if (!value.IsEqual(valueToCompare, tolerance))
                throw new ArgumentException($"{name} must be equal to {valueToCompare}");
        }

        public static void MustBeWithinRange(int value, int from, int to, bool inclusiveRange = true,
            string name = "argument")
        {
            if (inclusiveRange)
            {
                if (value < from || value > to)
                    throw new ArgumentOutOfRangeException($"{name} is out of range [{from};{to}]");
            }
            else
            {
                if (value <= from || value >= to)
                    throw new ArgumentOutOfRangeException($"{name} is out of range ({from};{to})");
            }
        }

        public static void MustBeWithinRange(double value, double from, double to, bool inclusiveRange = true, string name = "argument")
        {
            if (inclusiveRange)
            {
                if (value < from || value > to)
                    throw new ArgumentOutOfRangeException($"{name} is out of range [{from};{to}]");

            }
            else
            {
                if (value <= from || value >= to)
                    throw new ArgumentOutOfRangeException($"{name} is out of range ({from};{to})");
            }
        }

        public static void MustBeWithinRange(int value, (int from, int to) range, bool inclusiveRange = true, string name = "argument")
        {
            if (inclusiveRange)
            {
                if (value < range.from || value > range.to)
                    throw new ArgumentOutOfRangeException($"{name} is out of range [{range.from};{range.to}]");
            }
            else
            {
                if (value <= range.from || value >= range.to)
                    throw new ArgumentOutOfRangeException($"{name} is out of range ({range.from};{range.to})");
            }
        }

        #region MustBePositive
        public static void MustBePositive(int value, string name = "argument")
        {
            if (value <= 0)
                throw new ArgumentException($"{name} must be positive number (value:{value})");
        }

        public static void MustBePositive(decimal value, string name = "argument")
        {
            if (value <= 0)
                throw new ArgumentException($"{name} must be positive number (value:{value})");
        }

        public static void MustBePositive(double value, string name = "argument")
        {
            if (value <= 0)
                throw new ArgumentException($"{name} must be positive number (value:{value})");
        } 
        #endregion

        public static void MustBeGreaterThan(int value, int number = 0, string name = "argument")
        {
            if (value <= number)
                throw new ArgumentException($"{name} must be greater than {number}");
        }

        public static void MustBeGreaterThan(double value, double number = 0, string name = "argument")
        {
            if (value <= number)
                throw new ArgumentException($"{name} must be greater than {number}");
        }

        public static void MustBeGreaterThan(decimal value, decimal number = 0, string name = "argument")
        {
            if (value <= number)
                throw new ArgumentException($"{name} must be greater than {number}");
        }

        public static void MustBeGreaterThanOrEqual(int value, int number = 0, string name = "argument")
        {
            if (value < number)
                throw new ArgumentException($"{name} must be greater or equal to {number}");
        }

        public static void MustBeGreaterThanOrEqual(double value, double number = 0, string name = "argument")
        {
            if (value < number)
                throw new ArgumentException($"{name} must be greater or equal to {number}");
        }

        public static void MustBeGreaterThanOrEqual(decimal value, decimal number = 0, string name = "argument")
        {
            if (value < number)
                throw new ArgumentException($"{name} must be greater or equal to {number}");
        }

        public static string MustBeOneOf(string value, params string[] values)
        {
            if (values.Contains(value))
                return value;

            throw new ArgumentOutOfRangeException($"{nameof(value)} `{value}` is not one of allowed values: {values.Stringify()}");
        }

        public static T MustBeOneOf<T>(T value, params T[] values)
        {
            if (values.Contains(value))
                return value;

            throw new ArgumentOutOfRangeException($"{nameof(value)} `{value}` is not one of allowed values: {string.Join(", ", values)}");
        }
    }
}
