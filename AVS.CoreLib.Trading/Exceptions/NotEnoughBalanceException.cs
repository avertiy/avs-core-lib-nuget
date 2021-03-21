using System;

namespace AVS.CoreLib.Trading.Exceptions
{
    public class NotEnoughBalanceException : Exception
    {
        public NotEnoughBalanceException(string message) : base(message)
        {
        }
    }
}
