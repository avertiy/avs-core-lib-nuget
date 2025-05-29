using Xunit.Sdk;

namespace AVS.CoreLib.UnitTesting.XUnit.Exceptions
{
    public class NotEqualException : Xunit.Sdk.XunitException
    {
        public NotEqualException(object expected, object actual, string userMassage)
            : base($"Assert.NotEqual() Failure {expected} != {actual} \r\n" + userMassage)
        {
        }
    }
}