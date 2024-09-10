using System;
using System.Diagnostics.CodeAnalysis;
using AVS.CoreLib.Abstractions.Responses;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.UnitTesting.XUnit.Exceptions;
using Xunit.Sdk;
using EqualException = AVS.CoreLib.UnitTesting.XUnit.Exceptions.EqualException;
using FalseException = AVS.CoreLib.UnitTesting.XUnit.Exceptions.FalseException;
using NotEqualException = AVS.CoreLib.UnitTesting.XUnit.Exceptions.NotEqualException;
using NotNullException = AVS.CoreLib.UnitTesting.XUnit.Exceptions.NotNullException;
using NullException = AVS.CoreLib.UnitTesting.XUnit.Exceptions.NullException;
using TrueException = AVS.CoreLib.UnitTesting.XUnit.Exceptions.TrueException;

#nullable enable
namespace AVS.CoreLib.UnitTesting.xUnit
{
    public partial class Assert : Xunit.Assert
    {
        public static void Success(IResponse response, string userMessage)
        {
            if (!response.Success)
                throw new TrueException($"\r\n {userMessage}\r\n Error: {response.Error}", response.Success);
        }

        public static void Null([MaybeNull] object? obj, string userMessage = "Expected null value")
        {
            if (obj != null)
                throw new NullException(userMessage);
        }

        public static void NotNull([NotNull] object? obj, string userMessage = "Expected not null value")
        {
            if (obj == null)
                throw new NotNullException(userMessage);
        }

        public static new void True(bool condition, string userMessage)
        {
            if (!condition)
                throw new TrueException(userMessage, condition);
        }

        public static new void False(bool condition, string userMessage)
        {
            if (condition)
                throw new FalseException(userMessage, condition);
        }

        public static void WithinRange(decimal actual, (decimal from, decimal to) range, string userMessage)
        {
            if (actual <= range.from || actual >= range.to)
                throw new RangeException(userMessage);
        }

        public static void WithinInclRange(decimal actual, (decimal from, decimal to) range, string userMessage)
        {
            if (actual < range.from || actual > range.to)
                throw new RangeException(userMessage);
        }

        #region Equal overloads
        public static void Equal(string expected, string actual, string userMessage,
            bool ignoreCase = true,
            bool ignoreLineEndingDifferences = true,
            bool ignoreWhiteSpaceDifferences = true)
        {
            int index1 = -1;
            int index2 = -1;
            int num1 = 0;
            int num2 = 0;
            if (expected == null && actual == null)
                return;

            if (expected == actual)
                return;

            if (actual != null && expected != null)
            {
                index1 = 0;
                index2 = 0;
                num1 = expected.Length;
                num2 = actual.Length;
                while (index1 < num1 && index2 < num2)
                {
                    char upperInvariant1 = expected[index1];
                    char upperInvariant2 = actual[index2];
                    if (ignoreLineEndingDifferences && IsLineEnding(upperInvariant1) && IsLineEnding(upperInvariant2))
                    {
                        index1 = SkipLineEnding(expected, index1);
                        index2 = SkipLineEnding(actual, index2);
                    }
                    else if (ignoreWhiteSpaceDifferences && IsWhiteSpace(upperInvariant1) && IsWhiteSpace(upperInvariant2))
                    {
                        index1 = SkipWhitespace(expected, index1);
                        index2 = SkipWhitespace(actual, index2);
                    }
                    else
                    {
                        if (ignoreCase)
                        {
                            upperInvariant1 = char.ToUpperInvariant(upperInvariant1);
                            upperInvariant2 = char.ToUpperInvariant(upperInvariant2);
                        }
                        if ((int)upperInvariant1 == (int)upperInvariant2)
                        {
                            ++index1;
                            ++index2;
                        }
                        else
                            break;
                    }
                }
            }

            if (index1 < num1 || index2 < num2)
                throw new StringEqualException(expected, actual, userMessage, index1, index2);
        }

        public static void Equal(decimal expected, decimal actual, string? userMessage = null)
        {
            if (expected != actual)
                throw new EqualException(expected, actual, userMessage);
        }

        public static void Equal(decimal expected, decimal actual, decimal tolerance, string? userMessage = null)
        {
            var diff = expected - actual;
            if (diff.Abs() > tolerance)
                throw new EqualException(expected, actual, userMessage);
        }

        public static void Equal(double expected, double actual, double tolerance, string? userMessage = null)
        {
            if (Math.Abs(expected - actual) > tolerance)
                throw new EqualException(expected, actual, userMessage);
        }

        public static void Equal(int expected, int actual, string? userMessage = null)
        {
            if (expected != actual)
                throw new EqualException(expected, actual, userMessage);
        }

        public static void Equal<T>(T expected, T actual, string userMessage)
        {
            try
            {
                Equal<T>(expected, actual);
            }
            catch
            {
                throw new EqualException(expected!, actual!, userMessage);
            }
        }
        #endregion

        #region NotEqual overloads
        public static void NotEqual(decimal expected, decimal actual, string userMessage)
        {
            if (expected == actual)
                throw new NotEqualException(expected, actual, userMessage);
        }

        public static void NotEqual(double expected, double actual, string userMessage)
        {
            if (expected == actual)
                throw new NotEqualException(expected, actual, userMessage);
        }

        public static void NotEqual(int expected, int actual, string userMessage)
        {
            if (expected == actual)
                throw new NotEqualException(expected, actual, userMessage);
        }

        public static void NotEqual<T>(T expected, T actual, string userMessage)
        {
            try
            {
                NotEqual(expected, actual);
            }
            catch
            {
                throw new NotEqualException(expected, actual, userMessage);
            }
        }
        #endregion

        public static new void Same(object? expected, object? actual)
        {
            if (!object.ReferenceEquals(expected, actual))
                throw new SameException(expected, actual);
        }

        private static bool IsLineEnding(char c)
        {
            if (c != '\r')
                return c == '\n';
            return true;
        }

        private static bool IsWhiteSpace(char c)
        {
            if (c != ' ')
                return c == '\t';
            return true;
        }

        private static int SkipLineEnding(string value, int index)
        {
            if (value[index] == '\r')
                ++index;
            if (index < value.Length && value[index] == '\n')
                ++index;
            return index;
        }

        private static int SkipWhitespace(string value, int index)
        {
            for (; index < value.Length; ++index)
            {
                switch (value[index])
                {
                    case '\t':
                    case ' ':
                        continue;
                    default:
                        return index;
                }
            }
            return index;
        }
    }
}