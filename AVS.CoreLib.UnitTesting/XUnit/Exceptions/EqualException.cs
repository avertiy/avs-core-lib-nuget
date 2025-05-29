using Xunit.Sdk;

namespace AVS.CoreLib.UnitTesting.XUnit.Exceptions
{
    public class EqualException : Xunit.Sdk.XunitException
    {
        public EqualException(object? expected, object? actual, string? userMassage = null)
            : base("Assert.Equal() Failure\r\n" + userMassage ?? $"{actual} NOT EQUAL to {expected}", null)
        {
        }
    }
}