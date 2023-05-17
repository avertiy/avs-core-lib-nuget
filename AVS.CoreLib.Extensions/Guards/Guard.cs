using System;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.Guards
{
    public class Guard : IMustBeGuardClause, IAgainstGuardClause, IMustGuardClause, IArrayGuardClause, IDictionaryGuardClause
    {
        private Guard() { }

        private static readonly Guard _guard = new Guard();
        /// <summary>
        /// An entry point to a set of Guard Clauses.
        /// </summary>
        public static IAgainstGuardClause Against { get; set; } = _guard;

        //public static IMustGuardClause Must { get; set; } = _guard;
        public static IMustBeGuardClause MustBe { get; set; } = _guard;
        public static IArrayGuardClause Array { get; set; } = _guard;
        /// <summary>
        /// for now this is just for a convenience but later might separate out guard list extensions
        /// </summary>
        public static IArrayGuardClause List { get; set; } = _guard;
        public static IDictionaryGuardClause Dictionary { get; set; } = _guard;



        #region Obosolete methods

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


        [Obsolete("use Guard.MustBe.GreaterThan(..)")]
        public static void MustBeGreaterThan(int value, int number = 0, string name = "argument")
        {
            if (value <= number)
                throw new ArgumentException($"{name} must be greater than {number}");
        }

        [Obsolete("use Guard.MustBe.GreaterThan(..)")]
        public static void MustBeGreaterThan(double value, double number = 0, string name = "argument")
        {
            if (value <= number)
                throw new ArgumentException($"{name} must be greater than {number}");
        }

        [Obsolete("use Guard.MustBe.GreaterThan(..)")]
        public static void MustBeGreaterThan(decimal value, decimal number = 0, string name = "argument")
        {
            if (value <= number)
                throw new ArgumentException($"{name} must be greater than {number}");
        }

        [Obsolete("use Guard.MustBe.GreaterThanOrEqual(..)")]
        public static void MustBeGreaterThanOrEqual(int value, int number = 0, string name = "argument")
        {
            if (value < number)
                throw new ArgumentException($"{name} must be greater or equal to {number}");
        }

        [Obsolete("use Guard.MustBe.GreaterThanOrEqual(..)")]
        public static void MustBeGreaterThanOrEqual(double value, double number = 0, string name = "argument")
        {
            if (value < number)
                throw new ArgumentException($"{name} must be greater or equal to {number}");
        }

        [Obsolete("use Guard.MustBe.GreaterThanOrEqual(..)")]
        public static void MustBeGreaterThanOrEqual(decimal value, decimal number = 0, string name = "argument")
        {
            if (value < number)
                throw new ArgumentException($"{name} must be greater or equal to {number}");
        }


        [Obsolete("use Guard.MustBe.Equal(..)")]
        public static void MustBeEqual(string? str1, string? str2, string? message = null)
        {
            if (str1 == null || str2 == null || !str1.Equals(str2))
                throw new ArgumentException(message ?? $"'{str1}' expected to be equal '{str2}'");
        }

        [Obsolete("use Guard.Must.BeEqual(..)")]
        public static void MustBeEqual(double value, double valueToCompare, double tolerance = 0.001, string name = "argument")
        {
            if (!value.IsEqual(valueToCompare, tolerance))
                throw new ArgumentException($"{name} must be equal to {valueToCompare}");
        }

        [Obsolete("use Guard.Must.BeEqual(..)")]
        public static void MustBeEqual(decimal value, decimal valueToCompare, decimal tolerance = 0.001m, string name = "argument")
        {
            if (!value.IsEqual(valueToCompare, tolerance))
                throw new ArgumentException($"{name} must be equal to {valueToCompare}");
        }
        [Obsolete("use Guard.MustBe.WithinRange(..)")]
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

        [Obsolete("use Guard.MustBe.WithinRange(..)")]
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

        [Obsolete("use Guard.MustBe.WithinRange(..)")]
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


        [Obsolete("Use Guard.Against.NullOrEmpty(..)")]
        public static void AgainstNullOrEmpty(string? param, string name = "argument")
        {
            if (string.IsNullOrEmpty(param))
                throw new ArgumentNullException($"{name} must be not null or empty");
        }

        [Obsolete("Use Guard.Against.NullOrEmpty(..)")]
        public static void AgainstNullOrEmpty(string? param, bool allowNull, string name = "argument")
        {
            if (string.IsNullOrEmpty(param) && !allowNull)
                throw new ArgumentNullException($"{name} must be not null or empty");
        }

        [Obsolete("Use Guard.Against.Null(..)")]
        public static void AgainstNull(object? arg, string? message = null)
        {
            if (arg == null)
                throw new ArgumentNullException(message ?? $"must be not null");
        }
        [Obsolete("Use Guard.Against.Null(..)")]
        public static void AgainstNull(object? param, bool allowNull, string name = "argument")
        {
            if (param == null && !allowNull)
                throw new ArgumentNullException($"{name} must be not null");
        }

        [Obsolete("Use Guard.Against.DateTimeMin(..)")]
        public static void AgainstDateTimeMin(DateTime param, string name = "argument")
        {
            if (param == DateTime.MinValue)
                throw new ArgumentNullException($"{name} must be not a DateTime.MinValue");
        } 
        #endregion
    }
}
