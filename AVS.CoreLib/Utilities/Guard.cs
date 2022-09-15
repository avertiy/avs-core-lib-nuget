using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Abstractions;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Structs;

namespace AVS.CoreLib.Utilities
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

        public static void AgainstNullOrEmpty(string param, string name = "argument")
        {
            if (string.IsNullOrEmpty(param))
            {
                throw new ArgumentNullException($"{name} must be not null or empty");
            }
        }

        public static void AgainstNullOrEmpty(string param, bool allowNull, string name = "argument")
        {
            if (string.IsNullOrEmpty(param) && !allowNull)
            {
                throw new ArgumentNullException($"{name} must be not null or empty");
            }
        }

        public static void AgainstNull(object param, string name = "argument")
        {
            if (param == null)
            {
                throw new ArgumentNullException($"{name} must be not null");
            }
        }

        public static void AgainstNull(object param, bool allowNull, string name = "argument")
        {
            if (param == null && !allowNull)
            {
                throw new ArgumentNullException($"{name} must be not null");
            }
        }

        public static void AgainstDateTimeMin(DateTime param, string name = "argument")
        {
            if (param == DateTime.MinValue)
            {
                throw new ArgumentNullException($"{name} must be not a DateTime.MinValue");
            }
        }

        public static void MustBeEqual(string str1, string str2)
        {
            if (!str1.Equals(str2))
                throw new ArgumentException($"'{str1}' expected to be equal '{str2}'");
        }

        public static void MustHaveValue(IHasValue obj)
        {
            if (obj == null || !obj.HasValue)
                throw new ArgumentNullException(nameof(obj));
        }

        public static void MustBeWithinRange(int value, Range<int> range)
        {
            if (!range.Contains(value))
                throw new ArgumentOutOfRangeException($"{nameof(value)} is out of range {range}");
        }

        public static void MustBeWithinRange(int value, (int from, int to) range)
        {
            if (value < range.from || value > range.to)
                throw new ArgumentOutOfRangeException($"{nameof(value)} is out of range {range}");
        }

        public static void MustBeGreaterThan(int value, int number = 0)
        {
            if (value <= number)
                throw new ArgumentException($"{nameof(value)} must be greater than {value}");
        }

        public static void MustBeGreaterThan(double value, double number = 0)
        {
            if (value <= number)
                throw new ArgumentException($"{nameof(value)} must be greater than {value}");
        }

        public static void MustBeGreaterThan(decimal value, decimal number = 0)
        {
            if (value <= number)
                throw new ArgumentException($"{nameof(value)} must be greater than {value}");
        }

        public static string MustBeOneOf(string value, params string[] values)
        {
            if (values.Contains(value))
                return value;

            throw new ArgumentOutOfRangeException($"{nameof(value)} `{value}` is not one of allowed values: {values.AsString()}");
        }

        public static T MustBeOneOf<T>(T value, params T[] values)
        {
            if (values.Contains(value))
                return value;

            throw new ArgumentOutOfRangeException($"{nameof(value)} `{value}` is not one of allowed values: {string.Join(", ", values)}");
        }
    }
}
