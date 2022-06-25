using System;

namespace AVS.CoreLib.Math.MathUtils.Fractions
{
    /// <summary>
    /// Exception class for Fraction, derived from System.Exception
    /// </summary>
    public class FractionException : Exception
    {
        public FractionException() : base()
        { }

        public FractionException(string Message) : base(Message)
        { }

        public FractionException(string Message, Exception InnerException) : base(Message, InnerException)
        { }
    }
}