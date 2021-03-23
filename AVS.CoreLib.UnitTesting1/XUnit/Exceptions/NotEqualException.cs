using Xunit.Sdk;

namespace AVS.CoreLib.UnitTesting.XUnit.Exceptions
{
    public class NotEqualException : AssertActualExpectedException
    {
        public NotEqualException(object expected, object actual, string userMassage)
            : base((object)("Not " + expected), actual, "Assert.NotEqual() Failure\r\n" + userMassage, (string)null, (string)null)
        {
        }
    }
}