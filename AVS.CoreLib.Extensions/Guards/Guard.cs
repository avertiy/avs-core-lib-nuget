﻿using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Extensions.Stringify;

namespace AVS.CoreLib.Guards
{
    public static class Guard
    {
        #region Array guards
        public static void ArrayIndex<T>(T[] arr, int index, string name = "index")
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException($"{name} [{index}] must be positive number");

            if (index > arr.Length)
                throw new ArgumentOutOfRangeException($"{name} [{index}] must not exceed the array length {arr.Length}");
        }

        public static void ArrayLength<T>(T[] arr, int length, string name = "array length")
        {
            if (arr.Length != length)
                throw new ArgumentOutOfRangeException($"{name} must be equal {length}");
        }

        public static void ArrayLength<T>(IList<T> arr, int length, string name = "collection length")
        {
            if (arr.Count != length)
                throw new ArgumentOutOfRangeException($"{name} must be equal {length}");
        }

        public static void ArrayMinLength<T>(T[] arr, int minLength, string name = "array length")
        {
            if (arr.Length < minLength)
                throw new ArgumentOutOfRangeException($"{name} must be at least {minLength}");
        }

        public static void ArrayMinLength<T>(IList<T> arr, int minLength, string name = "collection length")
        {
            if (arr.Count < minLength)
                throw new ArgumentOutOfRangeException($"{name} must be at least {minLength}");
        }

        public static void ArrayMaxLength<T>(T[] arr, int maxLength, string name = "array length")
        {
            if (arr.Length > maxLength)
                throw new ArgumentOutOfRangeException($"{name} must be at least {maxLength}");
        }

        public static void ArrayMaxLength<T>(IList<T> arr, int maxLength, string name = "collection length")
        {
            if (arr.Count > maxLength)
                throw new ArgumentOutOfRangeException($"{name} must be at least {maxLength}");
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

        public static void AgainstNullOrEmpty(string param, string name = "argument")
        {
            if (string.IsNullOrEmpty(param))
                throw new ArgumentNullException($"{name} must be not null or empty");
        }

        public static void AgainstNullOrEmpty(string param, bool allowNull, string name = "argument")
        {
            if (string.IsNullOrEmpty(param) && !allowNull)
                throw new ArgumentNullException($"{name} must be not null or empty");
        }

        public static void AgainstNull(object param, string name = "argument")
        {
            if (param == null)
                throw new ArgumentNullException($"{name} must be not null");
        }

        public static void AgainstNull(object param, bool allowNull, string name = "argument")
        {
            if (param == null && !allowNull)
                throw new ArgumentNullException($"{name} must be not null");
        }

        public static void AgainstDateTimeMin(DateTime param, string name = "argument")
        {
            if (param == DateTime.MinValue)
                throw new ArgumentNullException($"{name} must be not a DateTime.MinValue");
        }

        public static void MustBeEqual(string str1, string str2, string message = null)
        {
            if (!str1.Equals(str2))
                throw new ArgumentException(message ?? $"'{str1}' expected to be equal '{str2}'");
        }

        public static void MustBeWithinRange(int value, int from, int to, bool inclusiveRange = true, string name = "argument")
        {
            if (inclusiveRange)
                if (value < from || value > to)
                    throw new ArgumentOutOfRangeException($"{name} is out of range [{from};{to}]");
            else
                if (value <= from || value >= to)
                    throw new ArgumentOutOfRangeException($"{name} is out of range ({from};{to})");
        }

        public static void MustBeWithinRange(double value, double from, double to, bool inclusiveRange = true, string name = "argument")
        {
            if (inclusiveRange)
                if (value < from || value > to)
                    throw new ArgumentOutOfRangeException($"{name} is out of range [{from};{to}]");
            else
                if (value <= from || value >= to)
                    throw new ArgumentOutOfRangeException($"{name} is out of range ({from};{to})");
        }

        public static void MustBeWithinRange(int value, (int from, int to) range, bool inclusiveRange = true, string name = "argument")
        {
            if (inclusiveRange)
                if (value < range.from || value > range.to)
                    throw new ArgumentOutOfRangeException($"{name} is out of range [{range.from};{range.to}]");
            else
                if (value <= range.from || value >= range.to)
                    throw new ArgumentOutOfRangeException($"{name} is out of range ({range.from};{range.to})");

        }

        public static void MustBeGreaterThan(int value, int number = 0, string name = "argument")
        {
            if (value <= number)
                throw new ArgumentException($"{name} must be greater than {value}");
        }

        public static void MustBeGreaterThan(double value, double number = 0, string name = "argument")
        {
            if (value <= number)
                throw new ArgumentException($"{name} must be greater than {value}");
        }

        public static void MustBeGreaterThan(decimal value, decimal number = 0, string name = "argument")
        {
            if (value <= number)
                throw new ArgumentException($"{name} must be greater than {value}");
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