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

    public class NullException : XunitException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="T:Xunit.Sdk.NotNullException" /> class.
        /// </summary>
        public NullException(string userMessage)
            : base($"Assert.Null() Failure\r\n{userMessage}")
        {
        }
    }

    public class TrueException : XunitException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="T:Xunit.Sdk.NotNullException" /> class.
        /// </summary>
        public TrueException(string userMessage, bool condition)
            : base($"Assert.True({condition}) Failure\r\n{userMessage}")
        {
        }
    }

    public class RangeException : XunitException
    {
        /// <summary>
        /// Creates a new instance of the <see cref="T:Xunit.Sdk.NotNullException" /> class.
        /// </summary>
        public RangeException(string userMessage)
            : base($"Assert.WithinRange() Failure\r\n{userMessage}")
        {
        }
    }
}
