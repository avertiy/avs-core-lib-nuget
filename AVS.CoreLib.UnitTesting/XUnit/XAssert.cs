using AVS.CoreLib.Abstractions.Responses;
using AVS.CoreLib.UnitTesting.XUnit.Exceptions;
using Xunit;

namespace AVS.CoreLib.UnitTesting.xUnit
{
    public static class XAssert
    {
        public static void Success(IResponse response, string userMessage)
        {
            Assert.True(response.Success, $"\r\n {userMessage}\r\n Error: {response.Error}");
        }

        public static void NotNull(object obj, string userMessage)
        {
            if (obj == null)
                throw new NotNullException(userMessage);
        }

        public static void Equal(string expected, string actual, string userMessage,
            bool ignoreCase = true,
            bool ignoreLineEndingDifferences = true,
            bool ignoreWhiteSpaceDifferences = true)
        {
            int index1 = -1;
            int index2 = -1;
            int num1 = 0;
            int num2 = 0;
            if (expected == null)
            {
                if (actual == null)
                    return;
            }
            else if (actual != null)
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
                throw new EqualException(expected, actual, userMessage, index1, index2);
        }

        public static void Equal<T>(T expected, T actual, string userMessage)
        {
            try
            {
                Assert.Equal<T>(expected, actual);
            }
            catch
            {
                throw new EqualException(expected, actual, userMessage);
            }
        }

        public static void NotEqual<T>(T expected, T actual, string userMessage)
        {
            try
            {
                Assert.NotEqual(expected, actual);
            }
            catch
            {
                throw new NotEqualException(expected, actual, userMessage);
            }
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