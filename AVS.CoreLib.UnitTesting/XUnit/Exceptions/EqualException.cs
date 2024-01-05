using Xunit.Sdk;

namespace AVS.CoreLib.UnitTesting.XUnit.Exceptions
{
    public class EqualException : AssertActualExpectedException
    {
        public EqualException(object? expected, object? actual, string? userMassage = null)
            : base(expected, actual, 
                  "Assert.Equal() Failure\r\n" + userMassage ?? $"{actual} NOT EQUAL to {expected}")
        {
        }
    }
}