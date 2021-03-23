using Xunit.Sdk;

namespace AVS.CoreLib.UnitTesting.XUnit.Exceptions
{
    /// <summary>Exception thrown when an object is unexpectedly null.</summary>
    public class NotNullException : XunitException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="T:Xunit.Sdk.NotNullException" /> class.
        /// </summary>
        public NotNullException(string userMessage)
            : base($"Assert.NotNull() Failure\r\n{userMessage}")
        {
        }
    }
}
