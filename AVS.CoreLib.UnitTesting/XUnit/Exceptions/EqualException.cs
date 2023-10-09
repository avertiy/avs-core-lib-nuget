using Xunit.Sdk;

namespace AVS.CoreLib.UnitTesting.XUnit.Exceptions
{
    public class EqualException : AssertActualExpectedException
    {
        public EqualException(object expected, object actual, string userMassage)
            : base("Not " + expected, actual, "Assert.Equal() Failure\r\n" + userMassage, (string)null, (string)null)
        {
        }
    }
}